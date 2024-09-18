using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
    internal class SettingsValidator : ISettingsValidator
    {
        IYTApiCommunicator _ytCommunicator;
        //TODO: konstruktor z wsztrzykiwaniem

        public bool CheckIfFileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public bool CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            if (Directory.Exists(path))
                return true;
            else
                return false;
        }

        public async Task<bool> CheckPlaylistNameAsync(string playlistName)
        {
            if (string.IsNullOrEmpty(playlistName))
                return false;

            var result = await _ytCommunicator.GetPlaylistIdAsync(playlistName);

            if (result == null)
                return false;
            else
                return true;
        }
    }
}
