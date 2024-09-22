namespace YouTubeMusicAPI.Services.Interfaces
{
    public interface IFileChecker
    {
        bool CheckIfFileExists(string pathToClientSecretFile);
        bool CheckPath(string path);
    }
}