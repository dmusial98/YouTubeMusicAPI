using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	public interface IMusicDownloader
	{
		//public Task DownloadSingleAudioAsMp3Async(string videoUrl);
		public Task DownloadAudiosAsMp3Async(string[] urls);
        public string DirectoryPath { get; set; }
        public string FFmpegPath { get; set; }
        public int errorsNumberForSingleSong { get; set; }
        public Dictionary<string, int> ErrorsNumbersDictionary { get; set; }
    }
}
