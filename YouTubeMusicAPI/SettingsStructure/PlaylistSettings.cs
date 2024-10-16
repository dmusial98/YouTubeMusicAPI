using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.SettingsStructure
{
	public class PlaylistSettings
	{
		public string? name { get; set; }
		public string? path { get; set; }
        public Urls? urls { get; set; }
		public Download? download { get; set; }
		//public DislikeForBadUrls? dislikeForBadUrls { get; set; }
	}
}
