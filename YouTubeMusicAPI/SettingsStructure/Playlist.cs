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
        public bool GetUrls { get; set; }
        public bool DownloadMusic { get; set;}
        public bool SaveUrls { get; set; }
        public bool SaveBadUrls { get; set; }
        public bool DislikeForBadUrls { get; set; }
        public int ErrorNumbersForUrls { get; set; }
    }
}
