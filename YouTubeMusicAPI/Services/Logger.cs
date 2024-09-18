using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services
{
	public class Logger
	{
		public static void LogLeakOfSettings()
		{
			Console.WriteLine("Cannot read settings file -> settings.json");
		}
	}
}
