using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeMusicAPI.Services.Interfaces;
using YouTubeMusicAPI.SettingsStructure;

namespace YouTubeMusicAPI.Services
{
    public class SettingsValidator : ISettingsValidator
    {
        private readonly IYTApiCommunicator _ytCommunicator;
        private readonly IFileChecker _fileChecker; // Nowa klasa pomocnicza

        public SettingsValidator(IYTApiCommunicator ytCommunicator, IFileChecker fileChecker)
        {
            _ytCommunicator = ytCommunicator;
            _fileChecker = fileChecker;
        }

        public async Task<SettingsValidationResults> ValidateSettingsAsync(Settings settings)
        {
            if (string.IsNullOrEmpty(settings.pathToClientSecretFile))
                settings.pathToClientSecretFile = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");

            SettingsValidationResults results = new(settings.pathToClientSecretFile);

            _ytCommunicator.credentialsFileName = settings.pathToClientSecretFile;

            if (!_fileChecker.CheckIfFileExists(settings.pathToClientSecretFile))
            {
                Logger.LogInvalidPathToClientSecretFile(settings.pathToClientSecretFile);
                results.wasIncorrectPathToClientSecretFile = true;
            }

            var validationTasks = settings.playlists.Select(async playlist =>
            {
                var playlistResult = new SettingsValidationPlaylistResults(playlist);
                results.settingsValidationPlaylistResults.Add(playlistResult);

                if (playlist.name != "LL" && !(await CheckPlaylistNameAsync(playlist.name)))
                {
                    playlistResult.wasIncorrectPlaylistName = true;
                }

                if (string.IsNullOrEmpty(playlist.path))
                    playlist.path = Directory.GetCurrentDirectory();

                if (!_fileChecker.CheckPath(playlist.path))
                    playlistResult.wasIncorrectPlaylistPath = true;

                ValidatePlaylistUrls(playlist, playlistResult);
                ValidatePlaylistDownloadSettings(playlist, playlistResult);
            });

            await Task.WhenAll(validationTasks);

            return results;
        }

        private void ValidatePlaylistUrls(PlaylistSettings playlist, SettingsValidationPlaylistResults playlistResult)
        {
            if (playlist.urls != null && playlist.urls.saveUrlsInFile && string.IsNullOrEmpty(playlist.urls.urlsFileName))
            {
                playlistResult.wasIncorrectUrlFileToSave = true;
            }
        }

        private void ValidatePlaylistDownloadSettings(PlaylistSettings playlist, SettingsValidationPlaylistResults playlistResult)
        {
            if (playlist.download != null)
            {
                if ((playlist.download.downloadMusicFromApi || playlist.download.downloadMusicFromUrlFile) &&
                    !_fileChecker.CheckIfFileExists(playlist.download.ffmpegPath))
                {
                    playlistResult.wasIncorrectFFmpegPath = true;
                }

                if (playlist.download.downloadMusicFromUrlFile &&
                    !_fileChecker.CheckIfFileExists(Path.Combine(playlist.path, playlist.download.urlsFileName)))
                {
                    playlistResult.wasIncorrectUrlFileToDownload = true;
                }

                if (playlist.download.saveBadUrlsDuringDownloadInFile && string.IsNullOrEmpty(playlist.download.badUrlsFileName))
                {
                    playlistResult.wasIncorrectBadUrlsFileNameToSave = true;
                }

                if (playlist.download.errorNumbersForUrl < 0)
                    playlistResult.wasIncorrectErrorsNumberForUrl = true;

                if (playlist.download.maximumLengthInSeconds <= 0)
                    playlistResult.wasIncorrectMaximumLengthInSeconds = true;
            }
        }

        private async Task<bool> CheckPlaylistNameAsync(string playlistName)
        {
            if (string.IsNullOrEmpty(playlistName))
                return false;

            var result = await _ytCommunicator.GetPlaylistIdAsync(playlistName);
            return result != null;
        }
    }


    //internal class SettingsValidator : ISettingsValidator
    //{
    //    IYTApiCommunicator _ytCommunicator;

    //    public SettingsValidator(IYTApiCommunicator ytCommunicator)
    //    {
    //        _ytCommunicator = ytCommunicator;
    //    }

    //    public async Task<SettingsValidationResults> ValidateSettingsAsync(Settings settings)
    //    {
    //        if (string.IsNullOrEmpty(settings.pathToClientSecretFile))
    //            settings.pathToClientSecretFile = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");

    //        SettingsValidationResults results = new(settings.pathToClientSecretFile);

    //        _ytCommunicator.credentialsFileName = settings.pathToClientSecretFile;

    //        if (!CheckIfFileExists(settings.pathToClientSecretFile))
    //        {
    //            Logger.LogInvalidPathToClientSecretFile(settings.pathToClientSecretFile);
    //            results.wasIncorrectPathToClientSecretFile = true;
    //        }

    //        foreach (var playlist in settings.playlists)
    //        {
    //            results.settingsValidationPlaylistResults.Add(new SettingsValidationPlaylistResults(playlist));

    //            if (playlist.name != "LL")
    //            {
    //                if (!(await CheckPlaylistNameAsync(playlist.name)))
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectPlaylistName = true;
    //            }

    //            if (string.IsNullOrEmpty(playlist.path))
    //                playlist.path = Directory.GetCurrentDirectory();

    //            if (!CheckPath(playlist.path))
    //                results.settingsValidationPlaylistResults.Last().wasIncorrectPlaylistPath = true;

    //            if (playlist.urls != null && playlist.urls.saveUrlsInFile && string.IsNullOrEmpty(playlist.urls.urlsFileName))
    //                results.settingsValidationPlaylistResults.Last().wasIncorrectUrlFileToSave = true;

    //            if (playlist.download != null)
    //            {
    //                if ((playlist.download.downloadMusicFromApi || playlist.download.downloadMusicFromUrlFile) &&
    //                        !CheckIfFileExists(playlist.download.ffmpegPath))
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectFFmpegPath = true;

    //                if (playlist.download.downloadMusicFromUrlFile &&
    //                        !CheckIfFileExists(Path.Combine(playlist.path, playlist.download.urlsFileName)))
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectUrlFileToDownload = true;

    //                if (playlist.download.saveBadUrlsDuringDownloadInFile && string.IsNullOrEmpty(playlist.download.badUrlsFileName))
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectBadUrlsFileNameToSave = true;

    //                if (playlist.download.errorNumbersForUrl < 0)
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectErrorsNumberForUrl = true;

    //                if (playlist.download.maximumLengthInSeconds <= 0)
    //                    results.settingsValidationPlaylistResults.Last().wasIncorrectMaximumLengthInSeconds = true;
    //            }

    //            if (playlist.dislikeForBadUrls != null && playlist.dislikeForBadUrls.dislikeForBadUrls
    //                    && !CheckIfFileExists(Path.Combine(playlist.path, playlist.dislikeForBadUrls.badUrlsFileName)))
    //                results.settingsValidationPlaylistResults.Last().wasIncorrectBadUrlsFileNameToDislike = true;
    //        }

    //        return results;
    //    }

    //    private bool CheckIfFileExists(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return false;
    //        if (File.Exists(path))
    //            return true;
    //        else
    //            return false;
    //    }

    //    private bool CheckPath(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return false;
    //        if (Directory.Exists(path))
    //            return true;
    //        else
    //            return false;
    //    }

    //    private async Task<bool> CheckPlaylistNameAsync(string playlistName)
    //    {
    //        if (string.IsNullOrEmpty(playlistName))
    //            return false;

    //        var result = await _ytCommunicator.GetPlaylistIdAsync(playlistName);

    //        if (result == null)
    //            return false;
    //        else
    //            return true;
    //    }


    //}
}
