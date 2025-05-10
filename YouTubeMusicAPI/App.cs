using System.Reflection;
using YouTubeMusicAPI.Services;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;
using YouTubeMusicAPI.WorkPlan;

namespace YouTubeMusicAPI
{
	public class App
	{
		private readonly ISettingsReader settingsReader;
		private readonly ISettingsValidator validator;
		private readonly IWorkDispatcher workDispatcher;
		private readonly IYTApiCommunicator ytApiCommunicator;
		private readonly IUrlFileReaderWriter urlFileReader;
		private readonly IMusicDownloader musicDownloader;
		private readonly IFilesRenamer filesRenamer;
		private readonly FileUploader fileUploader;


		public App(ISettingsReader settingsReader,
			ISettingsValidator validator,
			IWorkDispatcher workDispatcher,
			IYTApiCommunicator ytApiCommunicator,
			IUrlFileReaderWriter urlFileReader,
			IMusicDownloader musicDownloader,
			IFilesRenamer filesRenamer)
		{
			this.settingsReader = settingsReader;
			this.validator = validator;
			this.workDispatcher = workDispatcher;
			this.ytApiCommunicator = ytApiCommunicator;
			this.urlFileReader = urlFileReader;
			this.musicDownloader = musicDownloader;
			this.filesRenamer = filesRenamer;

			var settings = settingsReader.ReadSettingsAsync().Result;
			var serverConfig = settings.serverConfig;

			if (serverConfig == null || string.IsNullOrEmpty(serverConfig.host) || string.IsNullOrEmpty(serverConfig.username) || string.IsNullOrEmpty(serverConfig.privateKeyPath) || string.IsNullOrEmpty(serverConfig.remoteDirectory))
				throw new InvalidOperationException("Server configuration is incomplete or missing.");
			
			this.fileUploader = new FileUploader(serverConfig.host, serverConfig.username, serverConfig.privateKeyPath, serverConfig.remoteDirectory);
		}

		public async Task Run()
		{
			var settings = await LoadSettings();
			if (settings == null)
			{
				Logger.LogLeakOfSettings();
				EndOfWork();
				return;
			}

			var validationResults = await ValidateSettings(settings);

			if (validationResults == null)
				throw new ArgumentNullException(nameof(validationResults), "Validation results cannot be null.");

			if (CheckIfWasError(validationResults))
			{
				Logger.LogErrorsInSettings(validationResults);
				EndOfWork();
				return;
			}

			var workPlan = workDispatcher.PlanWork(validationResults);
			if (workPlan != null && workPlan.playlistWorkList.Length > 0)
				await ProcessWorkPlan(workPlan);

			EndOfWork();
		}

		private async Task<Settings> LoadSettings() =>
			await settingsReader.ReadSettingsAsync();


		private async Task<SettingsValidationResults> ValidateSettings(Settings settings) =>
			await validator.ValidateSettingsAsync(settings);

		private bool CheckIfWasError(SettingsValidationResults validationResults)
		{
			return validationResults.settingsValidationPlaylistResults.Any(a => a.GetType()
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Where(p => p.PropertyType == typeof(bool))
			.Select(p => (bool)p.GetValue(a))
			.Any(value => value));
		}

		private async Task ProcessWorkPlan(WorkList workPlan)
		{
			foreach (var playlist in workPlan.playlistWorkList)
				await ProcessPlaylist(playlist);
		}

		private async Task ProcessPlaylist(PlaylistWorkList playlist)
		{
			string[]? urlsFromPlaylistYTApi = null;

			if (playlist.SaveUrlsInFile || playlist.DownloadMusicFromApi)
			{
				var playlistId = playlist.PlaylistName == "LL"
					? "LL" : await ytApiCommunicator.GetPlaylistIdAsync(playlist.PlaylistName);

				if (string.IsNullOrEmpty(playlistId))
					throw new ArgumentException("Playlist ID cannot be null or empty.", nameof(playlistId));

				urlsFromPlaylistYTApi = await ytApiCommunicator.GetUrlsFromPlaylistAsync(playlistId);
			}

			if (urlsFromPlaylistYTApi != null)
				await SaveUrlsIfRequiredAsync(playlist, urlsFromPlaylistYTApi);
			string[] UrlsOfDownlaodedMusic = await ReadDifferenciesFileIfRequiredAsync(playlist);
			if (urlsFromPlaylistYTApi != null)
				await DownloadMusicIfRequiredAsync(playlist, urlsFromPlaylistYTApi, UrlsOfDownlaodedMusic);
			await DownloadMusicFromUrlFileIfRequiredAsync(playlist, UrlsOfDownlaodedMusic);
			RenameFilesIfRequired(playlist);
			if (playlist.SendToServer)
				await UploadFilesAsync(playlist.PlaylistPath);
		}

		private void RenameFilesIfRequired(PlaylistWorkList playlist)
		{
			if (playlist.RenameFiles)
				filesRenamer.RenameFiles(playlist.PlaylistPath);
		}

		private async Task UploadFilesAsync(string directoryPath)
		{
			foreach (var file in Directory.GetFiles(directoryPath, "*.mp3"))
				await fileUploader.UploadFileAsync(file);
		}

		private async Task SaveUrlsIfRequiredAsync(PlaylistWorkList playlist, string[] urlsFromPlaylistYTApi)
		{
			if (playlist.SaveUrlsInFile && urlsFromPlaylistYTApi != null)
			{
				await urlFileReader.SaveUrlsInFileAsync(
					Path.Combine(playlist.PlaylistPath, playlist.UrlFileNameToSave),
					urlsFromPlaylistYTApi);
			}
		}

		private async Task<string[]> ReadDifferenciesFileIfRequiredAsync(PlaylistWorkList playlist)
		{
			if (playlist.ReadDifferenciesFile)
				return await urlFileReader.ReadUrlsFromFileAsync(Path.Combine(playlist.PlaylistPath, playlist.DifferenciesFileToRead));
			
			return Array.Empty<string>();
		}

		private async Task DownloadMusicIfRequiredAsync(PlaylistWorkList playlist, string[] urlsFromPlaylistYTApi, string[] differenciesUrls)
		{
			string[] urlsToDownload = GetUrlsToDownload(urlsFromPlaylistYTApi, differenciesUrls);

			if (playlist.DownloadMusicFromApi && urlsFromPlaylistYTApi != null)
			{
				musicDownloader.DirectoryPath = playlist.PlaylistPath;
				await musicDownloader.DownloadAudiosAsMp3Async(urlsToDownload.ToArray());
			}
		}

		private async Task DownloadMusicFromUrlFileIfRequiredAsync(PlaylistWorkList playlist, string[] differenciesUrls)
		{
			string[] urlsFromUrlFile = [];

			if (playlist.DownloadMusicFromUrlFile)
				urlsFromUrlFile = await urlFileReader.ReadUrlsFromFileAsync(
					Path.Combine(playlist.PlaylistPath, playlist.UrlFileNameToRead));
					
			string[] urlsToDownload = GetUrlsToDownload(urlsFromUrlFile, differenciesUrls);
			musicDownloader.DirectoryPath = playlist.PlaylistPath;
			await musicDownloader.DownloadAudiosAsMp3Async(urlsToDownload);
		}

		private static string[] GetUrlsToDownload(string[] urlsFromPlaylistYTApi, string[] differenciesUrls)
		{
			List<string> urlsToDownload = urlsFromPlaylistYTApi.ToList();

			if (differenciesUrls != null && differenciesUrls.Length > 0)
				foreach (string url in differenciesUrls)
					urlsToDownload.Remove(url);
			return urlsToDownload.ToArray();
		}

		private static void EndOfWork()
		{
			Logger.LogEndOfWork();
			Console.ReadLine();
		}
	}
}
