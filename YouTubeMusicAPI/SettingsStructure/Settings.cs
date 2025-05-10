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
		public PlaylistSettings[]? playlists { get; set; }
		public ServerConfig? serverConfig { get; set; }
	}

	public class ServerConfig
	{
		public string? host { get; set; }
		public string? username { get; set; }
		public string? privateKeyPath { get; set; }
		public string? remoteDirectory { get; set; }
	}
}
