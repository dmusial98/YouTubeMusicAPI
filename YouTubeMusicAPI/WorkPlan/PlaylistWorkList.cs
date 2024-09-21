namespace YouTubeMusicAPI.WorkPlan
{
	public class PlaylistWorkList
	{
		public string PlaylistName { get; }
		public string PlaylistPath { get; }
		public string UrlFileNameToSave { get; set; }
		public string UrlFileNameToRead { get; set; }
		public string BadUrlsFileNameToWrite { get; set; }
        public string FFmpegPath { get; set; }
        public int ErrorsNumberForSingleSong { get; set; }
        public bool SaveUrlsInFile { get; }
		public bool DownloadMusicFromUrlFile { get; }
		public bool DownloadMusicFromApi { get; }
		public bool SaveBadUrlsDuringDownloadInFile { get; }
		public bool DislikeForBadUrls { get; }

		public PlaylistWorkList(string playlistName,
                string playlistPath,
                string urlFileNameToSave,
                string urlFileNameToRead,
                string badUrlsFileNameToWrite,
                string ffmpegPath,
                int errorsNumberForSingleSong,
                bool saveUrlsInFile,
                bool downloadMusicFromUrlFile,
                bool downloadMusicFromApi,
                bool saveBadUrlsDuringDownloadInFile,
                bool dislikeForBadUrls)
        {
            PlaylistName = playlistName;
            SaveUrlsInFile = saveUrlsInFile;
            DownloadMusicFromUrlFile = downloadMusicFromUrlFile;
            DownloadMusicFromApi = downloadMusicFromApi;
            SaveBadUrlsDuringDownloadInFile = saveBadUrlsDuringDownloadInFile;
            DislikeForBadUrls = dislikeForBadUrls;
            UrlFileNameToSave = urlFileNameToSave;
            UrlFileNameToRead = urlFileNameToRead;
            BadUrlsFileNameToWrite = badUrlsFileNameToWrite;
            PlaylistPath = playlistPath;
            FFmpegPath = ffmpegPath;
            ErrorsNumberForSingleSong = errorsNumberForSingleSong;
        }
    }
}