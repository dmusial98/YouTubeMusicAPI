using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.WorkPlan
{
	public class PlaylistWorkList
	{
		public string PlaylistName { get; }
		public string PlaylistPath { get; }
		public string UrlFileNameToSave { get; }
		public string UrlFileNameToRead { get; }
		public string DifferenciesFileToRead { get; }
		public bool SaveUrlsInFile { get; }
		public bool DownloadMusicFromUrlFile { get; }
		public bool DownloadMusicFromApi { get; }
		public bool ReadDifferenciesFile { get; set; }
		public bool RenameFiles { get; }
		public bool SendToServer { get; }


		public PlaylistWorkList(PlaylistSettings playlistSettings, bool saveUrlsInFile, bool downloadMusicFromUrlFile, bool downloadMusicFromApi, bool readDifferencesFile, bool renameFiles, bool sendToServer)
		{
			PlaylistName = playlistSettings.name ?? "";
			PlaylistPath = playlistSettings.path ?? "";
			UrlFileNameToSave = playlistSettings.urls?.urlsFileName ?? "";
			UrlFileNameToRead = playlistSettings.download?.urlsFileNameToDownload ?? "";
			DifferenciesFileToRead = playlistSettings.download?.UrlsFileNameToReadDownloadedSongs ?? "";
			SaveUrlsInFile = saveUrlsInFile;
			DownloadMusicFromUrlFile = downloadMusicFromUrlFile;
			DownloadMusicFromApi = downloadMusicFromApi;
			ReadDifferenciesFile = readDifferencesFile;
			RenameFiles = renameFiles;
			SendToServer = sendToServer;
		}
	}
}