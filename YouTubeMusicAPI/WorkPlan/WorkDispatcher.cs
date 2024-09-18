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
            {
                List<PlaylistWorkList> _list = new List<PlaylistWorkList>();
                foreach (var playlist in validationResults.settingsValidationPlaylistResults)
                {
                    _list.Add(new PlaylistWorkList(
                         playlistName: playlist.PlaylistReadSettings.name,
                         false,
                         false,
                         false,
                         false,
                         false
                     ));
                }

                return new WorkList(_list.ToArray());
            }

            List<PlaylistWorkList> list = new List<PlaylistWorkList>();

            foreach (var playlist in validationResults.settingsValidationPlaylistResults)
            {
                bool saveUrlsInFile = true;
                bool downloadMusicFromUrlFile = true;
                bool downloadMusicFromApi = true;
                bool saveBadUrlsDuringDownloadInFile = true;
                bool dislikeForBadUrls = true;

                //ifologia
                if (!playlist.PlaylistReadSettings.urls.saveUrlsInFile ||
                    playlist.wasIncorrectPlaylistPath ||
                    playlist.wasIncorrectPlaylistName ||
                    playlist.wasIncorrectUrlFileToSave)
                    saveUrlsInFile = false;
                if (!playlist.PlaylistReadSettings.download.downloadMusicFromUrlFile ||
                    playlist.wasIncorrectFFmpegPath ||
                    playlist.wasIncorrectPlaylistPath ||
                    //playlist.wasIncorrectPlaylistName ||
                    playlist.wasIncorrectUrlFileToDownload ||
                    playlist.wasIncorrectErrorsNumberForUrl ||
                    playlist.wasIncorrectMaximumLengthInSeconds)
                    downloadMusicFromUrlFile = false;
                if (!playlist.PlaylistReadSettings.download.downloadMusicFromApi ||
                    playlist.wasIncorrectFFmpegPath ||
                    playlist.wasIncorrectPlaylistPath ||
                    playlist.wasIncorrectPlaylistName ||
                    playlist.wasIncorrectErrorsNumberForUrl ||
                    playlist.wasIncorrectMaximumLengthInSeconds)
                    downloadMusicFromApi = false;
                if ((!downloadMusicFromApi && !downloadMusicFromUrlFile) || 
                    playlist.wasIncorrectBadUrlsFileNameToSave)
                    saveBadUrlsDuringDownloadInFile = false;
                if (!playlist.PlaylistReadSettings.dislikeForBadUrls.dislikeForBadUrls ||
                    playlist.wasIncorrectPlaylistPath ||
                    playlist.wasIncorrectBadUrlsFileNameToDislike)
                    dislikeForBadUrls = false;

                //dodaj do listy workElement
                list.Add(new(playlist.PlaylistReadSettings.name,
                    saveUrlsInFile,
                    downloadMusicFromUrlFile,
                    downloadMusicFromApi,
                    saveBadUrlsDuringDownloadInFile,
                    dislikeForBadUrls));
            }

            return new WorkList(list.ToArray());
        }
    }
}
