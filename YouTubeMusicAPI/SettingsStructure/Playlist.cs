using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.SettingsStructure
{
    public class Playlist
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string UrlsFileName { get; set; }
        public string BadUrlsFileName { get; set; }
        public bool CreateDirectoryForPlaylistFile { get; set; }
        public bool GetUrls { get; set; }
        public bool DownloadMusicFromUrlFile { get; set;}
        public bool DownloadMusicFromApi { get; set; }
        public bool SaveUrlsInFile { get; set; }
        public bool SaveBadUrlsInFile { get; set; }
        public bool DislikeForBadUrls { get; set; }
        public int ErrorNumbersForUrls { get; set; }
        public int MaximumLengthInSeconds { get; set; }
    }
}
