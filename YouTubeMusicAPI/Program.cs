using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System.Diagnostics;
using TagLib.Id3v2;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeMusicAPI.Services;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI
{
	internal class Program
	{
		static YouTubeService youtubeService;
		static Dictionary<string, int> errorsNumbersDictionary = new();

		const int ErrorsNumberConst = 15;

		//static async Task Main(string[] args)
		//{
		//    var videoUrls = await File.ReadAllLinesAsync(@"Z:\MP3\badUrls.txt");

		//    try
		//    {
		//        UserCredential credential;
		//        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
		//        {
		//            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
		//                GoogleClientSecrets.Load(stream).Secrets,
		//                new[] { YouTubeService.Scope.YoutubeForceSsl },
		//                "user",
		//                CancellationToken.None,
		//                new Google.Apis.Util.Store.FileDataStore("YouTubeAPI")
		//            );
		//        }

		//        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
		//        {
		//            HttpClientInitializer = credential,
		//            ApplicationName = "YouTubeAPI"
		//        });

		//        int counter = 1;
		//        foreach(var video in videoUrls )
		//        {
		//            var videoId = video.Remove(0, 32);

		//            try
		//            {
		//                var request = youtubeService.Videos.Rate(videoId, VideosResource.RateRequest.RatingEnum.None);
		//                await request.ExecuteAsync();
		//                Console.WriteLine($"{counter++} Polubienie zostało usunięte.");
		//            }
		//            catch (Exception ex)
		//            {
		//                Console.WriteLine("Wystąpił błąd podczas usuwania polubienia: " + ex.Message);
		//            }
		//        }

		//    }
		//    catch (AggregateException ex)
		//    {
		//        foreach (var e in ex.InnerExceptions)
		//        {
		//            Console.WriteLine("Błąd podczas autoryzacji: " + e.Message);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        Console.WriteLine("Nieoczekiwany błąd: " + ex.Message);
		//    }
		//}

		static ISettingsReader settingsReader;
		static ISettingsValidator validator;
		static bool wasIncorrectPlaylistPath = false;
		static bool wasIncorrectPlaylistName = false;
		static bool wasIncorrectFFmpegPath = false;
		static bool wasIncorrectUrlFileToDownload = false;
		private static bool wasIncorrectPathToClientSecretFile = false;
		private static bool wasIncorrectUrlFileToSave = false;
		private static bool wasIncorrectBadUrlsFileNameToSave = false;
		private static bool wasIncorrectErrorsNumberForUrl = false;
		private static bool wasIncorrectMaximumLengthInSeconds = false;
        private static bool wasIncorrectBadUrlsFileNameToDislike = false;

        static async Task Main(string[] args)
		{
			//DependencyInjection


			//read settings
			Settings settings = await settingsReader.ReadSettingsAsync();
			if (settings != null)
			{
				if (String.IsNullOrEmpty(settings.pathToClientSecretFile))
					settings.pathToClientSecretFile = Directory.GetCurrentDirectory();

				if (validator.CheckIfFileExists(settings.pathToClientSecretFile))
				{
					Logger.LogInvalidPathToClientSecretFile(settings.pathToClientSecretFile);
					wasIncorrectPathToClientSecretFile = true;
				}

				foreach (var playlist in settings.playlists)
				{
					if (!(await validator.CheckPlaylistNameAsync(playlist.name)))
					{
						Logger.LogInvalidNameOfPlaylist(playlist.name);
						wasIncorrectPlaylistName = true;
					}

					if (String.IsNullOrEmpty(playlist.path))
						playlist.path = Directory.GetCurrentDirectory();

					if (!validator.CheckPath(playlist.path))
					{
						Logger.LogInvalidPathInSettings(playlist.path);
						wasIncorrectPlaylistPath = true;
					}

					if (playlist.urls != null && playlist.urls.saveUrlsInFile && String.IsNullOrEmpty(playlist.urls.urlsFileName))
					{
						Logger.LogLeakOfUrlFileNameToSave(playlist.urls.urlsFileName);
						wasIncorrectUrlFileToSave = true;
					}

					if (playlist.download != null)
					{
						if ((playlist.download.downloadMusicFromApi || playlist.download.downloadMusicFromUrlFile) &&
							!validator.CheckIfFileExists(playlist.download.ffmpegPath))
						{
							Logger.LogInvalidPathToFFmpeg(playlist.download.ffmpegPath);
							wasIncorrectFFmpegPath = true;
						}
						if (playlist.download.downloadMusicFromUrlFile &&
							!validator.CheckIfFileExists(Path.Combine(playlist.path, playlist.download.urlsFileName)))
						{
							Logger.LogInvalidUrlFileName(Path.Combine(playlist.path, playlist.download.urlsFileName));
							wasIncorrectUrlFileToDownload = true;
						}
						if (playlist.download.saveBadUrlsDuringDownloadInFile && String.IsNullOrEmpty(playlist.download.badUrlsFileName))
						{
							Logger.LogIncorrectBadUrlsFileNameToSave(playlist.download.badUrlsFileName);
							wasIncorrectBadUrlsFileNameToSave = true;
						}
						if (playlist.download.errorNumbersForUrl < 0)
						{
							Logger.LogIncorrectNumberOfErrorsForUrl(playlist.download.errorNumbersForUrl);
							wasIncorrectErrorsNumberForUrl = true;
						}
						if(playlist.download.maximumLengthInSeconds <= 0)
						{
							Logger.LogIncorrectMaximumLengthInSeconds(playlist.download.maximumLengthInSeconds);
							wasIncorrectMaximumLengthInSeconds = true;
						}
					}

					if (playlist.dislikeForBadUrls != null && playlist.dislikeForBadUrls.dislikeForBadUrls 
						&& !validator.CheckIfFileExists(Path.Combine(playlist.path, playlist.dislikeForBadUrls.badUrlsFileName)))
					{
						Logger.LogIncorrectBadUrlsFileNameToDislike(Path.Combine(playlist.path, playlist.dislikeForBadUrls.badUrlsFileName));
						wasIncorrectBadUrlsFileNameToDislike = true;
					}
				}
			}
			else
				Logger.LogLeakOfSettings();





			////////////////////////////////
			YTApiCommunication ytApiCommunication = new YTApiCommunication();


			UserCredential credential;
			using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
			{
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
						GoogleClientSecrets.Load(stream).Secrets,
						//new[] { YouTubeService.Scope.YoutubeReadonly },
						new[] { YouTubeService.Scope.YoutubeForceSsl },
						"dmusial98@gmail.com",
						CancellationToken.None,
						new Google.Apis.Util.Store.FileDataStore("YouTubeAPI")
				).Result;
			}

			youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "YouTubeAPI"
			});

			//var request = youtubeService.Playlists.List("snippet,contentDetails");
			//request.Mine = true;
			//var response = request.Execute();

			//var playlistId = response.Items.Where(p => p.Snippet.Title == "Disco").First().Id;
			////var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, playlistId);
			var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, "LL"); //ulubione utwory
			Console.WriteLine("Songs URL downloaded from playlist");
			await File.WriteAllLinesAsync(@"Z:\MP3\allSongs.txt", videoUrls);

			List<string> invalidVideosUrls = new();
			int counter = 1;
			foreach (var url in videoUrls)
			{
				string mp3Path = "";
				bool isException = false;
				do
				{
					mp3Path = await DownloadAudioAsMp3(url);
					isException = mp3Path.StartsWith("Exception");

					if (isException)
						Console.WriteLine($"{counter} {mp3Path}");
					else
						Console.WriteLine($"{counter++} MP3 file saved to {mp3Path}");
				} while (isException && errorsNumbersDictionary[url] < ErrorsNumberConst);
			}

			foreach (var url in errorsNumbersDictionary)
				invalidVideosUrls.Add(url.Key);

			await File.WriteAllLinesAsync(@"Z:\MP3\badUrls.txt", invalidVideosUrls);

		}

		static async Task<List<string>> GetVideoUrlsFromPlaylist(YouTubeService youtubeService, string playlistId)
		{
			var videoUrls = new List<string>();
			string nextPageToken = null;
			var counter = 1;
			do
			{
				var playlistItemsRequest = youtubeService.PlaylistItems.List("snippet");
				playlistItemsRequest.PlaylistId = playlistId;
				playlistItemsRequest.MaxResults = 50;
				playlistItemsRequest.PageToken = nextPageToken;

				var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();

				foreach (var playlistItem in playlistItemsResponse.Items)
				{
					string videoId = playlistItem.Snippet.ResourceId.VideoId;

					string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
					videoUrls.Add(videoUrl);
				}

				nextPageToken = playlistItemsResponse.NextPageToken;
			}
			while (nextPageToken != null);

			return videoUrls;
		}

		private static async Task<bool> CheckIfVideoIsValid(string videoId)
		{
			try
			{
				var videoRequest = youtubeService.Videos.List("snippet,status");
				videoRequest.Id = videoId;
				var response = await videoRequest.ExecuteAsync();

				return response.Items.Count > 0 && response.Items[0].Status.PrivacyStatus != "private";

			}
			catch (Exception ex)
			{
				return false;
			}
		}

		static async Task<string> DownloadAudioAsMp3(string videoUrl)
		{
			try
			{
				var youtube = new YoutubeClient();
				var video = await youtube.Videos.GetAsync(videoUrl);

				if (video.Duration.Value.TotalSeconds > 600)
					return $"{video.Author} - {video.Title} haven't been downloaded with duration {video.Duration.Value.TotalMinutes} minutes";

				string author = RemoveTopicAndPrecedingChars(video.Author.ToString());
				string filename = RemoveInvalidPathChars($"{author} - {video.Title}");

				string tempFilePath = Path.Combine(@"Z:\MP3", $"{filename}.webm"); // Tymczasowy plik audio
				string mp3FilePath = Path.Combine(@"Z:\MP3", $"{filename}.mp3"); // Docelowy plik MP3

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
					ConvertToMp3(tempFilePath, mp3FilePath);

					// Usunięcie tymczasowego pliku
					File.Delete(tempFilePath);
				}

				var tagFile = TagLib.File.Create(mp3FilePath);
				tagFile.Tag.Performers = new[] { author };
				tagFile.Tag.Title = video.Title;
				tagFile.Save();

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

		static void ConvertToMp3(string inputFilePath, string outputFilePath)
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

		static string RemoveInvalidPathChars(string path)
		{
			char[] invalidChars = "<>:\"/\\|?*".ToCharArray();
			return string.Concat(path.Where(c => !invalidChars.Contains(c)));
		}

		static string RemoveTopicAndPrecedingChars(string input)
		{
			string toRemove = "Topic";
			int index = input.IndexOf(toRemove);

			if (index >= 3) // Sprawdź, czy istnieje wystarczająco dużo znaków do usunięcia
			{
				return input.Remove(index - 3, toRemove.Length + 3);
			}

			return input; // Jeśli "Topic" nie istnieje lub nie ma wystarczająco dużo znaków, zwróć oryginalny string
		}
	}
}
