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
							 string.Empty,
							 string.Empty,
							 string.Empty,
							 string.Empty,
							 string.Empty,
							 0,
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

				string UrlFileNameToSave = "";
				string UrlFileNameToRead = "";
				string BadUrlsFileNameToWrite = "";

				if (!playlist.PlaylistReadSettings.urls.saveUrlsInFile ||
						playlist.wasIncorrectPlaylistPath ||
						playlist.wasIncorrectPlaylistName ||
						playlist.wasIncorrectUrlFileToSave)
					saveUrlsInFile = false;
				else
                    UrlFileNameToSave = playlist.PlaylistReadSettings.urls.urlsFileName;

                if (!playlist.PlaylistReadSettings.download.downloadMusicFromUrlFile ||
						playlist.wasIncorrectFFmpegPath ||
						playlist.wasIncorrectPlaylistPath ||
						//playlist.wasIncorrectPlaylistName ||
						playlist.wasIncorrectUrlFileToDownload ||
						playlist.wasIncorrectErrorsNumberForUrl ||
						playlist.wasIncorrectMaximumLengthInSeconds)
					downloadMusicFromUrlFile = false;
				else
                    UrlFileNameToRead = playlist.PlaylistReadSettings.download.urlsFileName;

                if (!playlist.PlaylistReadSettings.download.downloadMusicFromApi ||
						playlist.wasIncorrectFFmpegPath ||
						playlist.wasIncorrectPlaylistPath ||
						playlist.wasIncorrectPlaylistName ||
						playlist.wasIncorrectErrorsNumberForUrl ||
						playlist.wasIncorrectMaximumLengthInSeconds)
					downloadMusicFromApi = false;

				if (!playlist.PlaylistReadSettings.download.saveBadUrlsDuringDownloadInFile ||
					(!downloadMusicFromApi && !downloadMusicFromUrlFile) ||
						playlist.wasIncorrectBadUrlsFileNameToSave)
					saveBadUrlsDuringDownloadInFile = false;
				else
                    BadUrlsFileNameToWrite = playlist.PlaylistReadSettings.download.badUrlsFileName;

                if (!playlist.PlaylistReadSettings.dislikeForBadUrls.dislikeForBadUrls ||
						playlist.wasIncorrectPlaylistPath ||
						playlist.wasIncorrectBadUrlsFileNameToDislike)
					dislikeForBadUrls = false;

				list.Add(new(playlist.PlaylistReadSettings.name,
					playlist.PlaylistReadSettings.path,
					UrlFileNameToSave,
					UrlFileNameToRead,
					BadUrlsFileNameToWrite,
					playlist.PlaylistReadSettings.download.ffmpegPath,
					playlist.PlaylistReadSettings.download.errorNumbersForUrl,
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
