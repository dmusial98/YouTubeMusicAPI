using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
	public class FFmpegConnector : IFFmpegConnector
	{
		public string FFmpegPath { get; set; }

		public void ConvertToMp3(string inputFilePath, string outputFilePath)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = FFmpegPath,
				Arguments = $"-i \"{inputFilePath}\" -q:a 0 \"{outputFilePath}\"", // Konwertowanie z najwyższą jakością
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (var process = Process.Start(processStartInfo))
			{
				process.WaitForExit();
			}
		}

	}
}
