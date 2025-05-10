namespace YouTubeMusicAPI.SettingsStructure
{
    public class Download
    {
        public bool downloadMusicFromUrlFile { get; set; } = false;
        public bool downloadMusicFromApi {  get; set; } = false;
        public bool downloadMusicWithDifferencesFile { get; set; } = false;
        public string urlsFileNameToDownload { get; set; } = String.Empty;
        public string UrlsFileNameToReadDownloadedSongs {  get; set; } = String.Empty;
    }
}