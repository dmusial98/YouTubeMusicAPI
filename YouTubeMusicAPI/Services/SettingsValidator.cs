using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services
{
	public class SettingsValidator : ISettingsValidator
	{
		private readonly IYTApiCommunicator _ytCommunicator;
		private readonly IFileChecker _fileChecker;

		public SettingsValidator(IYTApiCommunicator ytCommunicator, IFileChecker fileChecker)
		{
			_ytCommunicator = ytCommunicator;
			_fileChecker = fileChecker;
		}

		public async Task<SettingsValidationResults> ValidateSettingsAsync(Settings settings)
		{
			if (string.IsNullOrEmpty(settings.pathToClientSecretFile))
				settings.pathToClientSecretFile = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");

			SettingsValidationResults results = new(settings.pathToClientSecretFile);

			_ytCommunicator.credentialsFileName = settings.pathToClientSecretFile;

			if (!_fileChecker.CheckIfFileExists(settings.pathToClientSecretFile))
			{
				Logger.LogInvalidPathToClientSecretFile(settings.pathToClientSecretFile);
				results.wasIncorrectPathToClientSecretFile = true;
			}

			var validationTasks = settings.playlists.Select(async playlist =>
			{
				var playlistResult = new SettingsValidationPlaylistResults(playlist);
				results.settingsValidationPlaylistResults.Add(playlistResult);

				if (playlist.name != "LL" && !await CheckPlaylistNameAsync(playlist.name))
				{
					playlistResult.wasIncorrectPlaylistName = true;
				}

				if (string.IsNullOrEmpty(playlist.path))
					playlist.path = Directory.GetCurrentDirectory();

				if (!_fileChecker.CheckPath(playlist.path))
					playlistResult.wasIncorrectPlaylistPath = true;

				ValidatePlaylistUrls(playlist, playlistResult);
				ValidatePlaylistDownloadSettings(playlist, playlistResult);
			});

			await Task.WhenAll(validationTasks);

			return results;
		}

		private void ValidatePlaylistUrls(PlaylistSettings playlist, SettingsValidationPlaylistResults playlistResult)
		{
			if (playlist.urls != null && playlist.urls.saveUrlsInFile && string.IsNullOrEmpty(playlist.urls.urlsFileName))
			{
				playlistResult.wasIncorrectUrlFileToSave = true;
			}
		}

		private void ValidatePlaylistDownloadSettings(PlaylistSettings playlist, SettingsValidationPlaylistResults playlistResult)
		{
			if (playlist.download != null)
			{
				if (playlist.download.downloadMusicFromUrlFile &&
					!_fileChecker.CheckIfFileExists(Path.Combine(playlist.path, playlist.download.urlsFileNameToDownload)))
					playlistResult.wasIncorrectUrlFileToDownload = true;
				
				if(playlist.download.downloadMusicWithDifferencesFile && !_fileChecker.CheckIfFileExists(Path.Combine(playlist.path, playlist.download.UrlsFileNameToReadDownloadedSongs)))
					playlistResult.wasIncorrectDifferencesFileName = true;
			}
		}

		private async Task<bool> CheckPlaylistNameAsync(string playlistName)
		{
			if (string.IsNullOrEmpty(playlistName))
				return false;

			var result = await _ytCommunicator.GetPlaylistIdAsync(playlistName);
			return result != null;
		}
	}
}
