using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
	public class WorkDispatcher : IWorkDispatcher
	{
		public WorkList PlanWork(SettingsValidationResults validationResults)
		{
			if (validationResults.wasIncorrectPathToClientSecretFile)
				return GenerateWorkListForInvalidSettings(validationResults);

			return GenerateWorkListForValidSettings(validationResults);
		}

		private WorkList GenerateWorkListForInvalidSettings(SettingsValidationResults validationResults)
		{
			var list = validationResults.settingsValidationPlaylistResults
				.Select(playlist => new PlaylistWorkList(
					new PlaylistSettings()
					{
						name = null,
						path = null,
						urls = new Urls(),
						download = new(),
					},
					false,
					false,
					false,
					false,
					false,
					false))
				.ToList();

			return new WorkList(list.ToArray());
		}

		private WorkList GenerateWorkListForValidSettings(SettingsValidationResults validationResults)
		{
			var list = new List<PlaylistWorkList>();

			foreach (var playlist in validationResults.settingsValidationPlaylistResults)
			{
				if (playlist.PlaylistReadSettings == null)
					continue;

				bool saveUrlsInFile = ShouldSaveUrlsInFile(playlist);
				bool downloadMusicFromUrlFile = ShouldDownloadMusicFromUrlFile(playlist);
				bool downloadMusicFromApi = ShouldDownloadMusicFromApi(playlist);
				bool readDifferencesFile = ShouldReadDifferencesFile(playlist);
				
				bool willBeDownload = downloadMusicFromApi || downloadMusicFromUrlFile;
				
				bool renameFiles = willBeDownload && playlist.PlaylistReadSettings.renameFiles;
				bool sendToServer = willBeDownload && playlist.PlaylistReadSettings.sendToServer;

				list.Add(new PlaylistWorkList(
					playlist.PlaylistReadSettings,
					saveUrlsInFile,
					downloadMusicFromUrlFile,
					downloadMusicFromApi,
					readDifferencesFile,
					renameFiles,
					sendToServer));
			}

			return new WorkList(list.ToArray());
		}

		private bool ShouldReadDifferencesFile(SettingsValidationPlaylistResults playlist)
		{
			return playlist!.PlaylistReadSettings!.download!.downloadMusicWithDifferencesFile &&
				   !playlist.wasIncorrectDifferencesFileName &&
				   !playlist.wasIncorrectPlaylistPath &&
				   !playlist.wasIncorrectPlaylistName;
		}

		private bool ShouldSaveUrlsInFile(SettingsValidationPlaylistResults playlist)
		{
			return playlist!.PlaylistReadSettings!.urls!.saveUrlsInFile &&
				   !playlist.wasIncorrectPlaylistPath &&
				   !playlist.wasIncorrectPlaylistName &&
				   !playlist.wasIncorrectUrlFileToSave;
		}

		private bool ShouldDownloadMusicFromUrlFile(SettingsValidationPlaylistResults playlist)
		{
			return playlist.PlaylistReadSettings.download!.downloadMusicFromUrlFile &&
				   !playlist.wasIncorrectPlaylistPath &&
				   !playlist.wasIncorrectUrlFileToDownload;
		}

		private bool ShouldDownloadMusicFromApi(SettingsValidationPlaylistResults playlist)
		{
			return playlist.PlaylistReadSettings.download!.downloadMusicFromApi &&
				   !playlist.wasIncorrectPlaylistPath &&
				   !playlist.wasIncorrectPlaylistName;
		}
	}
}
