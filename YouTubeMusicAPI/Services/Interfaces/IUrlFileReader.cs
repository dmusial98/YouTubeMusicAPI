using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services.Interfaces
{
    public interface IUrlFileReader
    {
        public Task<string[]> ReadUrlsFromFileAsync(string path);
    }
}
