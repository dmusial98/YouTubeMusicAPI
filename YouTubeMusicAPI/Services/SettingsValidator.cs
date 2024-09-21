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
	internal class SettingsValidator : ISettingsValidator
	{
		IYTApiCommunicator _ytCommunicator;

		public SettingsValidator(IYTApiCommunicator ytCommunicator)
		{
			_ytCommunicator = ytCommunicator;
        }

		public async Task<SettingsValidationResults> ValidateSettingsAsync(Settings settings)
		{
			if (String.IsNullOrEmpty(settings.pathToClientSecretFile))
				settings.pathToClientSecretFile = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");

            SettingsValidationResults results = new(settings.pathToClientSecretFile);

			_ytCommunicator.credentialsFileName = settings.pathToClientSecretFile;

            if (!CheckIfFileExists(settings.pathToClientSecretFile))
			{
				Logger.LogInvalidPathToClientSecretFile(settings.pathToClientSecretFile);
				results.wasIncorrectPathToClientSecretFile = true;
			}

			foreach (var playlist in settings.playlists)
			{
				results.settingsValidationPlaylistResults.Add(new SettingsValidationPlaylistResults(playlist));

				if (playlist.name != "LL")
				{
					if (!(await CheckPlaylistNameAsync(playlist.name)))
						results.settingsValidationPlaylistResults.Last().wasIncorrectPlaylistName = true;
				}

				if (String.IsNullOrEmpty(playlist.path))
					playlist.path = Directory.GetCurrentDirectory();

				if (!CheckPath(playlist.path))
					results.settingsValidationPlaylistResults.Last().wasIncorrectPlaylistPath = true;

				if (playlist.urls != null && playlist.urls.saveUrlsInFile && String.IsNullOrEmpty(playlist.urls.urlsFileName))
					results.settingsValidationPlaylistResults.Last().wasIncorrectUrlFileToSave = true;

				if (playlist.download != null)
				{
					if ((playlist.download.downloadMusicFromApi || playlist.download.downloadMusicFromUrlFile) &&
							!CheckIfFileExists(playlist.download.ffmpegPath))
						results.settingsValidationPlaylistResults.Last().wasIncorrectFFmpegPath = true;

					if (playlist.download.downloadMusicFromUrlFile &&
							!CheckIfFileExists(Path.Combine(playlist.path, playlist.download.urlsFileName)))
						results.settingsValidationPlaylistResults.Last().wasIncorrectUrlFileToDownload = true;

					if (playlist.download.saveBadUrlsDuringDownloadInFile && String.IsNullOrEmpty(playlist.download.badUrlsFileName))
						results.settingsValidationPlaylistResults.Last().wasIncorrectBadUrlsFileNameToSave = true;

					if (playlist.download.errorNumbersForUrl < 0)
						results.settingsValidationPlaylistResults.Last().wasIncorrectErrorsNumberForUrl = true;

					if (playlist.download.maximumLengthInSeconds <= 0)
						results.settingsValidationPlaylistResults.Last().wasIncorrectMaximumLengthInSeconds = true;
				}

				if (playlist.dislikeForBadUrls != null && playlist.dislikeForBadUrls.dislikeForBadUrls
						&& !CheckIfFileExists(Path.Combine(playlist.path, playlist.dislikeForBadUrls.badUrlsFileName)))
					results.settingsValidationPlaylistResults.Last().wasIncorrectBadUrlsFileNameToDislike = true;
			}

			return results;
		}

		private bool CheckIfFileExists(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;
			if (File.Exists(path))
				return true;
			else
				return false;
		}

		private bool CheckPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;
			if (Directory.Exists(path))
				return true;
			else
				return false;
		}

		private async Task<bool> CheckPlaylistNameAsync(string playlistName)
		{
			if (string.IsNullOrEmpty(playlistName))
				return false;

			var result = await _ytCommunicator.GetPlaylistIdAsync(playlistName);

			if (result == null)
				return false;
			else
				return true;
		}


	}
}
