namespace YouTubeMusicAPI.SettingsStructure
{
    public class Download
    {
        public bool downloadMusicFromUrlFile { get; set; } = false;
        public bool downloadMusicFromApi {  get; set; } = false;
        //public bool saveBadUrlsDuringDownloadInFile { get; set; } = false;
        //public int errorNumbersForUrl { get; set; } = 15;
        //public int maximumLengthInSeconds { get; set; } = 600;
        public string urlsFileName { get; set; } = String.Empty;
        //public string badUrlsFileName { get; set; } = String.Empty;
        //public string ffmpegPath { get; set; } = String.Empty;
    }
}