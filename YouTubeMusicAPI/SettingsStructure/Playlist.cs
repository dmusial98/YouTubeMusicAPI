using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.SettingsStructure
{
	public class Playlist
	{
		public string? Name { get; set; }
		public string? Path { get; set; }
		public string? UrlsFileName { get; set; }
		public string? BadUrlsFileName { get; set; }
		public string FFmpegPath { get; set; }
		public bool CreateDirectoryForPlaylistFile { get; set; } = false;
		public bool GetUrls { get; set; } = false;
		public bool DownloadMusicFromUrlFile { get; set; } = false;
		public bool DownloadMusicFromApi { get; set; } = false;
		public bool SaveUrlsInFile { get; set; } = false;
		public bool SaveBadUrlsInFile { get; set; } = false;
		public bool DislikeForBadUrls { get; set; } = false;
		public int ErrorNumbersForUrls { get; set; } = 15;
		public int MaximumLengthInSeconds { get; set; } = 600;
	}
}
