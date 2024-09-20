using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
	public interface IYTApiCommunicator
	{
        public string credentialsFileName { get; set; }
        public Task RemoveLikeForVideosAsync(string[] urls);
		public Task<string?> GetPlaylistIdAsync(string playlistName);
		public Task<string[]> GetUrlsFromPlaylistAsync(string playlistId);
	}
}
