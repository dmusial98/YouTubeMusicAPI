using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	internal interface ISettingsValidator
	{
		public (bool, string) CheckPlaylistName(string playlistName);
		public (bool, string) CheckPath(string path);
		public (bool, string) CheckIfUrlFileExists(string path);
		public (bool, string) CheckFFmpegFilePath(string path);


	}
}
