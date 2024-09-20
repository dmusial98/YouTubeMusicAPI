using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
	public class UrlFileReaderWriter : IUrlFileReaderWriter
	{
		public async Task<string[]> ReadUrlsFromFileAsync(string path)
		{
			string[] urls = await File.ReadAllLinesAsync(path);
			return urls;
		}

		public async Task SaveUrlsInFileAsync(string filename, string[] urls)
		{
			await File.WriteAllLinesAsync(filename, urls);
		}
	}
}
