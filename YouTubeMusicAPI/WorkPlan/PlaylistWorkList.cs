using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
	public class PlaylistWorkList
	{
		public string PlaylistName { get; }
		public string PlaylistPath { get; }
		public string UrlFileNameToSave { get; }
		public string UrlFileNameToRead { get; }
		//public string BadUrlsFileNameToWrite { get; }
		//public string FFmpegPath { get; }
		//public int ErrorsNumberForSingleSong { get; }
		public bool SaveUrlsInFile { get; }
		public bool DownloadMusicFromUrlFile { get; }
		public bool DownloadMusicFromApi { get; }
		//public bool SaveBadUrlsDuringDownloadInFile { get; }
		//public bool DislikeForBadUrls { get; }

		public PlaylistWorkList(PlaylistSettings playlistSettings, bool saveUrlsInFile, bool downloadMusicFromUrlFile, bool downloadMusicFromApi/*, bool saveBadUrlsDuringDownloadInFile, bool dislikeForBadUrls*/)
		{
			PlaylistName = playlistSettings.name ?? "";
			PlaylistPath = playlistSettings.path ?? "";
			UrlFileNameToSave = playlistSettings.urls?.urlsFileName ?? "";
			UrlFileNameToRead = playlistSettings.download?.urlsFileName ?? "";
			//BadUrlsFileNameToWrite = playlistSettings.download?.badUrlsFileName ?? "";
			//FFmpegPath = playlistSettings.download?.ffmpegPath ?? "";
			//ErrorsNumberForSingleSong = playlistSettings.download?.errorNumbersForUrl ?? 0;
			SaveUrlsInFile = saveUrlsInFile;
			DownloadMusicFromUrlFile = downloadMusicFromUrlFile;
			DownloadMusicFromApi = downloadMusicFromApi;
			//SaveBadUrlsDuringDownloadInFile = saveBadUrlsDuringDownloadInFile;
			//DislikeForBadUrls = dislikeForBadUrls;
		}
	}
}