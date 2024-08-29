using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using NAudio.Lame;
using NAudio.Wave;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTubeMusicAPI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
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

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "YouTubeAPI"
            });

            var request = youtubeService.Playlists.List("snippet,contentDetails");
            request.Mine = true;
            var response = request.Execute();

            var playlistId = response.Items.Where(p => p.Snippet.Title == "Disco").First().Id;

            //var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, playlistId);
            var videoUrls = await GetVideoUrlsFromPlaylist(youtubeService, "LL"); //ulubione utwory

            int counter = 1;

            foreach (var url in videoUrls)
            {
                string mp3Path = "";

                do
                {
                    mp3Path = await DownloadAudioAsMp3(url);
                    Console.WriteLine($"{counter} MP3 file saved to {mp3Path}");
                } while (mp3Path.StartsWith("Exception"));

                counter++;
            }
        }

        static async Task<List<string>> GetVideoUrlsFromPlaylist(YouTubeService youtubeService, string playlistId)
        {
            var videoUrls = new List<string>();
            string nextPageToken = null;

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

        static async Task<string> DownloadAudioAsMp3(string videoUrl)
        {
            try
            {
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(videoUrl);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                string tempFilePath = Path.Combine(@"Z:\MP3", $"{video.Author} - {video.Title}.webm"); // Tymczasowy plik audio
                string mp3FilePath = Path.Combine(@"Z:\MP3", $"{video.Author} - {video.Title}.mp3"); // Docelowy plik MP3

                if (!File.Exists(mp3FilePath))
                {
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
                return mp3FilePath;
            }
            catch (Exception e)
            {
                return $"Exception - {e.Message}";
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
    }
}
