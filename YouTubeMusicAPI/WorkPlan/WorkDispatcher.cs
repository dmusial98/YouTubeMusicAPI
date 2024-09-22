using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        dislikeForBadUrls = new()
                    },
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
                bool saveUrlsInFile = ShouldSaveUrlsInFile(playlist);
                bool downloadMusicFromUrlFile = ShouldDownloadMusicFromUrlFile(playlist);
                bool downloadMusicFromApi = ShouldDownloadMusicFromApi(playlist);
                bool saveBadUrlsDuringDownloadInFile = ShouldSaveBadUrlsDuringDownload(playlist, downloadMusicFromApi, downloadMusicFromUrlFile);
                bool dislikeForBadUrls = ShouldDislikeForBadUrls(playlist);

                list.Add(new PlaylistWorkList(playlist.PlaylistReadSettings,
                    saveUrlsInFile,
                    downloadMusicFromUrlFile,
                    downloadMusicFromApi,
                    saveBadUrlsDuringDownloadInFile,
                    dislikeForBadUrls));
            }

            return new WorkList(list.ToArray());
        }

        private bool ShouldSaveUrlsInFile(SettingsValidationPlaylistResults playlist)
        {
            return playlist.PlaylistReadSettings.urls.saveUrlsInFile &&
                   !playlist.wasIncorrectPlaylistPath &&
                   !playlist.wasIncorrectPlaylistName &&
                   !playlist.wasIncorrectUrlFileToSave;
        }

        private bool ShouldDownloadMusicFromUrlFile(SettingsValidationPlaylistResults playlist)
        {
            return playlist.PlaylistReadSettings.download.downloadMusicFromUrlFile &&
                   !playlist.wasIncorrectFFmpegPath &&
                   !playlist.wasIncorrectPlaylistPath &&
                   !playlist.wasIncorrectUrlFileToDownload &&
                   !playlist.wasIncorrectErrorsNumberForUrl &&
                   !playlist.wasIncorrectMaximumLengthInSeconds;
        }

        private bool ShouldDownloadMusicFromApi(SettingsValidationPlaylistResults playlist)
        {
            return playlist.PlaylistReadSettings.download.downloadMusicFromApi &&
                   !playlist.wasIncorrectFFmpegPath &&
                   !playlist.wasIncorrectPlaylistPath &&
                   !playlist.wasIncorrectPlaylistName &&
                   !playlist.wasIncorrectErrorsNumberForUrl &&
                   !playlist.wasIncorrectMaximumLengthInSeconds;
        }

        private bool ShouldSaveBadUrlsDuringDownload(SettingsValidationPlaylistResults playlist, bool downloadMusicFromApi, bool downloadMusicFromUrlFile)
        {
            return playlist.PlaylistReadSettings.download.saveBadUrlsDuringDownloadInFile &&
                   (downloadMusicFromApi || downloadMusicFromUrlFile) &&
                   !playlist.wasIncorrectBadUrlsFileNameToSave;
        }

        private bool ShouldDislikeForBadUrls(SettingsValidationPlaylistResults playlist)
        {
            return playlist.PlaylistReadSettings.dislikeForBadUrls.dislikeForBadUrls &&
                   !playlist.wasIncorrectPlaylistPath &&
                   !playlist.wasIncorrectBadUrlsFileNameToDislike;
        }
    }
}
