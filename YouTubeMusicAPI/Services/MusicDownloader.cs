using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
	public class MusicDownloader : IMusicDownloader
	{
		string ytDlpPath = "yt-dlp";
		string arguments = "-x --audio-format mp3 --audio-quality 0 --embed-thumbnail --add-metadata --max-duration 600 --output \"%(artist)s - %(title)s.%(ext)s\" ";
		string _directoryPath = string.Empty;

		public string DirectoryPath {
			get => _directoryPath;
			set 
			{ 
				arguments = $"-x --audio-format mp3 --audio-quality 0 --embed-thumbnail --add-metadata --output \"{RemoveInvalidPathChars(value)}{RemoveInvalidPathChars("%(artist)s - %(title)s.%(ext)s\"")} "; 
				_directoryPath = value;
			}
		}

		public MusicDownloader()
		{

		}

		public async Task DownloadAudiosAsMp3Async(string[] urls)
		{
			Logger.LogStartDownloadingSongs(urls.Length);

			foreach (var url in urls)
				DownloadSingleAudioAsMp3Async(url);
		}

		private void DownloadSingleAudioAsMp3Async(string videoUrl)
		{

			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.FileName = ytDlpPath;
			processInfo.Arguments = arguments + videoUrl;

			// Przekierowanie wyjścia, aby móc odczytać wynik działania yt-dlp
			processInfo.RedirectStandardOutput = true;
			processInfo.RedirectStandardError = true;
			processInfo.UseShellExecute = false;
			processInfo.CreateNoWindow = true;

			try
			{
				// Uruchamiamy proces
				using (Process process = Process.Start(processInfo))
				{
					// Odczytujemy i wyświetlamy wyjście
					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();

					// Oczekujemy na zakończenie procesu
					process.WaitForExit();

					// Wyświetlamy wynik działania
					Console.WriteLine("Output: " + output);
					Console.WriteLine("Error: " + error);
				}
			}
			catch (Exception ex)
			{
				// Obsługa błędów
				Console.WriteLine("Wystąpił błąd: " + ex.Message);
			}
		}

		static string RemoveInvalidPathChars(string path)
		{
			char[] invalidChars = "<>:\"/\\|?*".ToCharArray();
			return string.Concat(path.Where(c => !invalidChars.Contains(c)));
		}
	}
}

