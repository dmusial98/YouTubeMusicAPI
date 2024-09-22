using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
    public class FileChecker : IFileChecker
    {
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
    }
}
