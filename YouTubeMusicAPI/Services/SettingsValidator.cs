using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
	internal class SettingsValidator : ISettingsValidator
	{
		IYTApiCommunicator _ytCommunicator;

		public (bool, string) CheckFFmpegFilePath(string path)
		{
			if (File.Exists(path))
				return (true, "");
			else
				return (false, $"Invalid path to FFmpeg exe file: {path}");
		}

		public (bool, string) CheckIfUrlFileExists(string path)
		{
			if (File.Exists(path))
				return (true, "");
			else
				return (false, $"File with urls for playlist: {path} doesn't exist");
		}

		public (bool, string) CheckPath(string path)
		{
			if (Directory.Exists(path))
				return (true, "");
			else
				return (false, $"Path {path} doesn't exist");
		}

		public (bool, string) CheckPlaylistName(string playlistName)
		{
			var result = _ytCommunicator.GetPlaylistIdAsync(playlistName);

			if (result == null)
				return (false, $"Playlist {playlistName} doesn't exist");
			else
				return (true, "");
		}
	}
}
