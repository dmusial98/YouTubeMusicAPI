using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services
{
    public class FFMpegConnector
    {
        public static string FFMpegPath {  get; set; }

        public void ConvertToMp3(string inputFilePath, string outputFilePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\Dawid\Desktop\ffmpeg-7.0.2-essentials_build\bin\ffmpeg.exe",
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
