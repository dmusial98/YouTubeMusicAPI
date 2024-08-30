using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System.Diagnostics;
using TagLib.Id3v2;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTubeMusicAPI
{
    internal class Program
    {
        static YouTubeService youtubeService;

        static async Task Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //DownloadAudioAsMp3(@"https://www.youtube.com/watch?v=zpW8j9CSU24");

            //////////////////////////////////////////////////////////////////////////////////////
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "dmusial98@gmail.com",
                    CancellationToken.None,
                    new FileDataStore("YouTubeAPI")
                ).Result;
            }

            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "YouTubeAPI"
            });

            var request = youtubeService.Playlists.List("snippet,contentDetails");
            request.Mine = true;
            var response = request.Execute();

            //var playlistId = response.Items.Where(p => p.Snippet.Title == "Disco").First().Id;
            ////var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, playlistId);
            var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, "LL"); //ulubione utwory
            Console.WriteLine("Songs URL downloaded from playlist");
            await File.WriteAllLinesAsync(@"Z:\MP3\allSongs.txt", videoUrls);

            //var videoUrls = await File.ReadAllLinesAsync(@"Z:\MP3\allSongs.txt");

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
                } while (isException);
            }
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

                    if(await CheckIfVideoIsValid(videoId))
                    {
                        string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                        videoUrls.Add(videoUrl);

                        Console.WriteLine($"{counter++} Url checked");
                    }
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
