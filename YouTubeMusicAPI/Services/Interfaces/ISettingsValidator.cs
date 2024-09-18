using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	internal interface ISettingsValidator
	{
		public Task<bool> CheckPlaylistNameAsync(string playlistName);
		public bool CheckPath(string path);
		public bool CheckIfFileExists(string path);


	}
}
