using System;
using System.Collections.Generic;
using System.Linq;
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
			//read settings
			Settings settings = await settingsReader.ReadSettingsAsync();
			if (settings != null)
			{
				var validationSettingsResults = await validator.ValidateSettingsAsync(settings);
				Logger.LogErrorsInSettings(validationSettingsResults);
				var workPlan = workDispatcher.PlanWork(validationSettingsResults);

				if (workPlan != null && workPlan.playlistWorkList.Length > 0)
				{
					foreach (var playlist in workPlan.playlistWorkList)
					{
						string[] urlsFromPlaylistYTApi = null;
						string[] urlsFromUrlFile = null;

						if (playlist.SaveUrlsInFile || playlist.DownloadMusicFromApi)
						{
							var playlistId = await ytApiCommunicator.GetPlaylistIdAsync(playlist.PlaylistName);
							urlsFromPlaylistYTApi = await ytApiCommunicator.GetUrlsFromPlaylistAsync(playlistId);
						}

						if (playlist.SaveUrlsInFile && urlsFromPlaylistYTApi != null)
							await urlFileReader.SaveUrlsInFileAsync(playlist.UrlFileNameToSave, urlsFromPlaylistYTApi);

						if (playlist.DownloadMusicFromApi && urlsFromPlaylistYTApi != null)
							//ustawic parametry dla downloadera jeszcze
							await musicDownloader.DownloadAudiosAsMp3Async(urlsFromPlaylistYTApi);

						if (playlist.DownloadMusicFromUrlFile)
						{
							urlsFromUrlFile = await urlFileReader.ReadUrlsFromFileAsync(playlist.UrlFileNameToRead);
							if (urlsFromUrlFile != null)
								await musicDownloader.DownloadAudiosAsMp3Async(urlsFromUrlFile);
						}

						//SaveBadUrlsDuringDownlaodToFile

						//DislikeForBadUrls
					}
				}
			}
			else
				Logger.LogLeakOfSettings();
		}
	}
}
