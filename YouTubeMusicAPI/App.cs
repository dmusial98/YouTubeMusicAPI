using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;
using YouTubeMusicAPI.WorkPlan;

namespace YouTubeMusicAPI
{
	public class App
	{
		//TODO: obsluga pobierania muzyki na podstawie urlow, których nie ma w pliku z pobranymi utworami - podac defaultowa nazwe
		//TODO: dodac pobieranie muzyki na podstawie yt-dlp
		//TODO: usunac ffmpeg
		//TODO dodac funkcjonalnosc usuwania id utworu

		private readonly ISettingsReader settingsReader;
		private readonly ISettingsValidator validator;
		private readonly IWorkDispatcher workDispatcher;
		private readonly IYTApiCommunicator ytApiCommunicator;
		private readonly IUrlFileReaderWriter urlFileReader;
		private readonly IMusicDownloader musicDownloader;

		public App(ISettingsReader settingsReader,
			ISettingsValidator validator,
			IWorkDispatcher workDispatcher,
			IYTApiCommunicator ytApiCommunicator,
			IUrlFileReaderWriter urlFileReader,
			IMusicDownloader musicDownloader)
		{
			this.settingsReader = settingsReader;
			this.validator = validator;
			this.workDispatcher = workDispatcher;
			this.ytApiCommunicator = ytApiCommunicator;
			this.urlFileReader = urlFileReader;
			this.musicDownloader = musicDownloader;
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

			if (validationResults == null || CheckIfWasError(validationResults))
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
			{
				await ProcessPlaylist(playlist);
			}
		}

		private async Task ProcessPlaylist(PlaylistWorkList playlist)
		{
			string[] urlsFromPlaylistYTApi = null;

			if (playlist.SaveUrlsInFile || playlist.DownloadMusicFromApi)
			{
				var playlistId = playlist.PlaylistName == "LL"
					? "LL"
					: await ytApiCommunicator.GetPlaylistIdAsync(playlist.PlaylistName);

				urlsFromPlaylistYTApi = await ytApiCommunicator.GetUrlsFromPlaylistAsync(playlistId);
			}

			await SaveUrlsIfRequired(playlist, urlsFromPlaylistYTApi);
			await DownloadMusicIfRequired(playlist, urlsFromPlaylistYTApi);
			await DownloadMusicFromUrlFileIfRequired(playlist);
			//await SaveBadUrlsIfRequired(playlist);
		}

		private async Task SaveUrlsIfRequired(PlaylistWorkList playlist, string[] urlsFromPlaylistYTApi)
		{
			if (playlist.SaveUrlsInFile && urlsFromPlaylistYTApi != null)
			{
				await urlFileReader.SaveUrlsInFileAsync(
					Path.Combine(playlist.PlaylistPath, playlist.UrlFileNameToSave),
					urlsFromPlaylistYTApi);
			}
		}

		private async Task DownloadMusicIfRequired(PlaylistWorkList playlist, string[] urlsFromPlaylistYTApi)
		{
			if (playlist.DownloadMusicFromApi && urlsFromPlaylistYTApi != null)
			{
				musicDownloader.DirectoryPath = playlist.PlaylistPath;
				//musicDownloader.FFmpegPath = playlist.FFmpegPath;
				//musicDownloader.errorsNumberForSingleSong = playlist.ErrorsNumberForSingleSong;
				await musicDownloader.DownloadAudiosAsMp3Async(urlsFromPlaylistYTApi);
			}
		}

		private async Task DownloadMusicFromUrlFileIfRequired(PlaylistWorkList playlist)
		{
			if (playlist.DownloadMusicFromUrlFile)
			{
				var urlsFromUrlFile = await urlFileReader.ReadUrlsFromFileAsync(
					Path.Combine(playlist.PlaylistPath, playlist.UrlFileNameToRead));
				if (urlsFromUrlFile != null)
				{
					await musicDownloader.DownloadAudiosAsMp3Async(urlsFromUrlFile);
				}
			}
		}

		//private async Task SaveBadUrlsIfRequired(PlaylistWorkList playlist)
		//{
		//	if (playlist.SaveBadUrlsDuringDownloadInFile)
		//	{
		//		var badUrls = musicDownloader.ErrorsNumbersDictionary
		//			.Where(badUrl => badUrl.Value >= playlist.ErrorsNumberForSingleSong)
		//			.Select(badUrl => badUrl.Key)
		//			.ToList();

		//		await urlFileReader.SaveUrlsInFileAsync(
		//			Path.Combine(playlist.PlaylistPath, playlist.BadUrlsFileNameToWrite),
		//			badUrls.ToArray());
		//	}
		//}

		private static void EndOfWork()
		{
			Logger.LogEndOfWork();
			Console.ReadLine();
		}
	}
}
