﻿using System;
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
	}
}
