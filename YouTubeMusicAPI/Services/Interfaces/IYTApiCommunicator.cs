using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	internal interface IYTApiCommunicator
	{
		public Task RemoveLikeForVideosAsync(string[] urls);
		public Task<string?> GetPlaylistIdAsync(string playlistName);
		public Task<string[]> GetUrlsFromPlaylistAsync(string playlistId);
	}
}
