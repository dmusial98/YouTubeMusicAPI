using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	public interface IMusicDownloader
	{
		public Task<string> DownloadAudioAsMp3Async(string videoUrl);
	}
}
