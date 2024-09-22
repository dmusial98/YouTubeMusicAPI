using AngleSharp.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Collections.Specialized;
using System.Text;
using TagLib;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services
{
    public class YTApiCommunicator : IYTApiCommunicator
    {
        public string credentialsFileName { get; set; }
        UserCredential? _credential;
        YouTubeService? _youtubeService;

        private async Task SetUp()
        {
            if (_credential == null || _youtubeService == null)
            {
                using (var stream = new FileStream(credentialsFileName, FileMode.Open, FileAccess.Read))
                {
                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            new[] { YouTubeService.Scope.YoutubeForceSsl },
                            "user",
                            CancellationToken.None,
                            new Google.Apis.Util.Store.FileDataStore("YouTubeAPI")
                    );
                }

                _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = "YouTubeAPI"
                });
            }
        }

        public async Task RemoveLikeForVideosAsync(string[] urls)
        {
            await SetUp();

            int counter = 1;
            foreach (var video in urls)
            {
                var videoId = video.Remove(0, 32);

                try
                {
                    var request = _youtubeService.Videos.Rate(videoId, VideosResource.RateRequest.RatingEnum.None);
                    await request.ExecuteAsync();
                    Console.WriteLine($"{counter++} Polubienie zosta³o usuniête.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wyst¹pi³ b³¹d podczas usuwania polubienia: " + ex.Message);
                }
            }

        }

        public async Task<string?> GetPlaylistIdAsync(string playlistName)
        {
            await SetUp();

            var request = _youtubeService.Playlists.List("snippet,contentDetails");
            request.Mine = true;
            request.MaxResults = 100;
            var response = request.Execute();
            
            return response.Items.FirstOrDefault(p => p.Snippet.Title == playlistName)?.Id;
        }

        public async Task<string[]> GetUrlsFromPlaylistAsync(string playlistId)
        {
            await SetUp();

            var videoUrls = new List<string>();
            string nextPageToken = null;
            do
            {
                var playlistItemsRequest = _youtubeService.PlaylistItems.List("snippet");
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

            return [.. videoUrls];
        }
    }
}