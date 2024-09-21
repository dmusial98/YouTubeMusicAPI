using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services
{
	public static class Logger
	{
		public static void LogErrorsInSettings(SettingsValidationResults validationResults)
		{
			if (validationResults != null)
			{
				if (validationResults.wasIncorrectPathToClientSecretFile)
					LogInvalidPathToClientSecretFile(validationResults.pathToClientSecretFile);

				foreach (var playlist in validationResults.settingsValidationPlaylistResults)
				{
					if (playlist.wasIncorrectPlaylistName)
						LogInvalidNameOfPlaylist(playlist.PlaylistReadSettings.name);

					if (playlist.wasIncorrectPlaylistPath)
						LogInvalidPathInSettings(playlist.PlaylistReadSettings.path);

					if (playlist.wasIncorrectUrlFileToSave)
						LogLeakOfUrlFileNameToSave(playlist.PlaylistReadSettings.urls.urlsFileName);

					if (playlist.wasIncorrectFFmpegPath)
						LogInvalidPathToFFmpeg(playlist.PlaylistReadSettings.download.ffmpegPath);

					if (playlist.wasIncorrectUrlFileToDownload)
						LogInvalidUrlFileName(Path.Combine(playlist.PlaylistReadSettings.path, playlist.PlaylistReadSettings.download.urlsFileName));
					
					if (playlist.wasIncorrectBadUrlsFileNameToSave)
						LogIncorrectBadUrlsFileNameToSave(playlist.PlaylistReadSettings.download.badUrlsFileName);
					
					if (playlist.wasIncorrectErrorsNumberForUrl)
						LogIncorrectNumberOfErrorsForUrl(playlist.PlaylistReadSettings.download.errorNumbersForUrl);
					
					if (playlist.wasIncorrectMaximumLengthInSeconds)
						LogIncorrectMaximumLengthInSeconds(playlist.PlaylistReadSettings.download.maximumLengthInSeconds);
				
					if (playlist.wasIncorrectBadUrlsFileNameToDislike)
						LogIncorrectBadUrlsFileNameToDislike(Path.Combine(playlist.PlaylistReadSettings.path, playlist.PlaylistReadSettings.dislikeForBadUrls.badUrlsFileName));
				}
			}
		}

		public static void LogLeakOfSettings() =>
			Console.WriteLine("Cannot read settings file -> settings.json");

		public static void LogInvalidNameOfPlaylist(string playlistName) =>
			Console.WriteLine($"Invalid name of playlist: {playlistName}");

		public static void LogInvalidPathInSettings(string path) =>
			Console.WriteLine($"Invalid path in settings.json file: {path}");

		public static void LogInvalidUrlFileName(string urlFileName) =>
			Console.WriteLine($"Invalid url file name: {urlFileName}");

		public static void LogInvalidPathToFFmpeg(string ffmpegPath) =>
			Console.WriteLine($"Invalid path to FFmpeg: {ffmpegPath}");

		public static void LogInvalidPathToClientSecretFile(string pathToClientSecretFile) =>
			Console.WriteLine($"Invalid path to client secret file: {pathToClientSecretFile}");

		public static void LogLeakOfUrlFileNameToSave(string urlsFileName) =>
			Console.WriteLine($"Leak of url file to save: {urlsFileName}");

		public static void LogIncorrectBadUrlsFileNameToSave(string badUrlsFileName) =>
			Console.WriteLine($"Incorrect bad urls file name to save: {badUrlsFileName}");

		public static void LogIncorrectNumberOfErrorsForUrl(int errorNumbersForUrl) =>
			Console.WriteLine($"Incorrect number of errors for url: {errorNumbersForUrl}");

		public static void LogIncorrectMaximumLengthInSeconds(int maximumLengthInSeconds) =>
			Console.WriteLine($"Incorrect maximum length for one song in seconds: {maximumLengthInSeconds}");

		public static void LogIncorrectBadUrlsFileNameToDislike(string badUrlsFileName) =>
			Console.WriteLine($"Incorrect bad urls file name to dislike: {badUrlsFileName}");

		public static void LogTooLongSong(string title, double duration) =>
			Console.WriteLine($"{title} haven't been downloaded with duration {duration} minutes\"");

		public static void LogDownloadedSong(int counter, string mp3FilePath) =>
			Console.WriteLine($"{counter}. {mp3FilePath} downloaded");

		public static void LogExceptionDuringDownload(string videoUrl, Exception e) =>
			Console.WriteLine($"{videoUrl} -> {e.Message}");

		public static void LogStartDownloadingSongs(int songsCount) =>
			Console.WriteLine($"Start downloading {songsCount} songs");

		public static void LogEndOfWork() =>
			Console.WriteLine("Program ends work, type Enter key to end");
    }
}
