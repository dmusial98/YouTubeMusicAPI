namespace YouTubeMusicAPI.SettingsStructure
{
    public class SettingsValidationPlaylistResults
    {
        public PlaylistSettings PlaylistReadSettings { get; }

        public bool wasIncorrectPlaylistPath { get; set; } = false;
        public bool wasIncorrectPlaylistName { get; set; } = false;
        //public bool wasIncorrectFFmpegPath { get; set; } = false;
        public bool wasIncorrectUrlFileToDownload { get; set; } = false;
        public bool wasIncorrectPathToClientSecretFile { get; set; } = false;
        public bool wasIncorrectUrlFileToSave { get; set; } = false;
        //public bool wasIncorrectBadUrlsFileNameToSave { get; set; } = false;
        //public bool wasIncorrectErrorsNumberForUrl { get; set; } = false;
        //public bool wasIncorrectMaximumLengthInSeconds { get; set; } = false;
        //public bool wasIncorrectBadUrlsFileNameToDislike { get; set; } = false;

        public SettingsValidationPlaylistResults(PlaylistSettings playlistReadSettings)
        {
            PlaylistReadSettings = playlistReadSettings;
        }
    }
}