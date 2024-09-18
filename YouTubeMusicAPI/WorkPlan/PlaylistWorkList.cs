namespace YouTubeMusicAPI.WorkPlan
{
    public class PlaylistWorkList
    {
        public string PlaylistName { get; }
        public bool SaveUrlsInFile { get; }
        public bool DownloadMusicFromUrlFile { get; }
        public bool DownloadMusicFromApi { get; }
        public bool SaveBadUrlsDuringDownloadInFile { get; }
        public bool DislikeForBadUrls { get; }

        public PlaylistWorkList(string playlistName, 
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
        }
    }
}