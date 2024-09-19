using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
    public class UrlFileReader : IUrlFileReader
    {
        public async Task<string[]> ReadUrlsFromFileAsync(string path)
        {
            string[] urls = await File.ReadAllLinesAsync(path);
            return urls;
        }
    }
}
