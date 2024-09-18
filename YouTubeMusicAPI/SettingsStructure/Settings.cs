using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.SettingsStructure
{
	public class Settings
	{
		public string? pathToClientSecretFile { get; set; }
		public PlaylistSettings[] playlists { get; set; }
	}
}
