using YoutubeExplode.Videos;

namespace YouTubeMusicAPI.Services.Interfaces
{
	public interface ITagger
	{
		public void DoTagsInFile(Video video, string author, string mp3FilePath);
	}
}
