namespace YouTubeMusicAPI.SettingsStructure
{
    public class SettingsValidationResults
    {
        public string pathToClientSecretFile { get; }
        public bool wasIncorrectPathToClientSecretFile { get; set; } = false;
        public List<SettingsValidationPlaylistResults> settingsValidationPlaylistResults { get; } = new();

        public SettingsValidationResults(string pathToClientSecretFile)
        {
            this.pathToClientSecretFile = pathToClientSecretFile;
        }
    }
}