using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeMusicAPI.Services
{
	public static class Logger
	{
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
    }
}
