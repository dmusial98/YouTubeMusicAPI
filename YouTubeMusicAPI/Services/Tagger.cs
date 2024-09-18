using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
	public class Tagger : ITagger
	{
		public void DoTagsInFile(Video video, string author, string mp3FilePath)
		{
			var tagFile = TagLib.File.Create(mp3FilePath);
			tagFile.Tag.Performers = new[] { author };
			tagFile.Tag.Title = video.Title;
			tagFile.Save();
		}
	}
}
