using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
	public class PlaylistWorkList
	{
		public string PlaylistName { get; }
		public string PlaylistPath { get; }
		public string UrlFileNameToSave { get; }
		public string UrlFileNameToRead { get; }
		public string DifferenciesFileToRead { get; }
		//public string BadUrlsFileNameToWrite { get; }
		//public string FFmpegPath { get; }
		//public int ErrorsNumberForSingleSong { get; }
		public bool SaveUrlsInFile { get; }
		public bool DownloadMusicFromUrlFile { get; }
		public bool DownloadMusicFromApi { get; }
		public bool ReadDifferenciesFile { get; set; }
		public bool RenameFiles { get; }

		//public bool SaveBadUrlsDuringDownloadInFile { get; }
		//public bool DislikeForBadUrls { get; }

		public PlaylistWorkList(PlaylistSettings playlistSettings, bool saveUrlsInFile, bool downloadMusicFromUrlFile, bool downloadMusicFromApi, bool readDifferencesFile, bool renameFiles /*, bool saveBadUrlsDuringDownloadInFile, bool dislikeForBadUrls*/)
		{
			PlaylistName = playlistSettings.name ?? "";
			PlaylistPath = playlistSettings.path ?? "";
			UrlFileNameToSave = playlistSettings.urls?.urlsFileName ?? "";
			UrlFileNameToRead = playlistSettings.download?.urlsFileNameToDownload ?? "";
			DifferenciesFileToRead = playlistSettings.download?.UrlsFileNameToReadDownloadedSongs ?? "";
			//BadUrlsFileNameToWrite = playlistSettings.download?.badUrlsFileName ?? "";
			//FFmpegPath = playlistSettings.download?.ffmpegPath ?? "";
			//ErrorsNumberForSingleSong = playlistSettings.download?.errorNumbersForUrl ?? 0;
			SaveUrlsInFile = saveUrlsInFile;
			DownloadMusicFromUrlFile = downloadMusicFromUrlFile;
			DownloadMusicFromApi = downloadMusicFromApi;
			ReadDifferenciesFile = readDifferencesFile;
			RenameFiles = renameFiles;
			//SaveBadUrlsDuringDownloadInFile = saveBadUrlsDuringDownloadInFile;
			//DislikeForBadUrls = dislikeForBadUrls;
		}
	}
}