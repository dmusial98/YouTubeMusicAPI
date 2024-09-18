using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public Dictionary<string, int> errorsNumbersDictionary { get; set; } = new();
		public string DirectoryPath { get; set; }
		public string FFMPEGPath { get; set; }
		IFFmpegConnector _ffmpegConnector;
		ITagger _tagger;


		public async Task<string> DownloadAudioAsMp3Async(string videoUrl)
		{
			try
			{
				var youtube = new YoutubeClient();
				var video = await youtube.Videos.GetAsync(videoUrl);

				if (video.Duration.Value.TotalSeconds > 600)
					return $"{video.Author} - {video.Title} haven't been downloaded with duration {video.Duration.Value.TotalMinutes} minutes";

				string author = RemoveTopicAndPrecedingChars(video.Author.ToString());
				string filename = RemoveInvalidPathChars($"{author} - {video.Title}");

				string tempFilePath = Path.Combine(DirectoryPath, $"{filename}.webm"); // Tymczasowy plik audio
				string mp3FilePath = Path.Combine(DirectoryPath, $"{filename}.mp3"); // Docelowy plik MP3

				if (!File.Exists(mp3FilePath))
				{
					var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
					var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

					// Pobieranie strumienia audio
					using (var inputStream = await youtube.Videos.Streams.GetAsync(audioStreamInfo))
					using (var outputStream = File.Create(tempFilePath))
					{
						await inputStream.CopyToAsync(outputStream);
					}

					// Konwersja do MP3 za pomocą FFmpeg
					_ffmpegConnector.ConvertToMp3(tempFilePath, mp3FilePath);

					// Usunięcie tymczasowego pliku
					File.Delete(tempFilePath);
				}

				_tagger.DoTagsInFile(video, author, mp3FilePath);

				return mp3FilePath;
			}
			catch (Exception e)
			{
				if (errorsNumbersDictionary.TryGetValue(videoUrl, out int errorsNumberInt))
					errorsNumbersDictionary[videoUrl] = ++errorsNumberInt;
				else
				{
					errorsNumbersDictionary.Add(videoUrl, 1);
				}
				return $"Exception -> {e.Message} {videoUrl}";
			}
		}



		string RemoveTopicAndPrecedingChars(string input)
		{
			string toRemove = "Topic";
			int index = input.IndexOf(toRemove);

			if (index >= 3) // Sprawdź, czy istnieje wystarczająco dużo znaków do usunięcia
			{
				return input.Remove(index - 3, toRemove.Length + 3);
			}

			return input; // Jeśli "Topic" nie istnieje lub nie ma wystarczająco dużo znaków, zwróć oryginalny string
		}

		static string RemoveInvalidPathChars(string path)
		{
			char[] invalidChars = "<>:\"/\\|?*".ToCharArray();
			return string.Concat(path.Where(c => !invalidChars.Contains(c)));
		}
	}
}

