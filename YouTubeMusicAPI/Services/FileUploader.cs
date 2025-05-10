using Renci.SshNet;

namespace YouTubeMusicAPI.Services
{
    public class FileUploader
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _privateKeyPath;
        private readonly string _remoteDirectory;

        public FileUploader(string host, string username, string privateKeyPath, string remoteDirectory)
        {
            _host = host;
            _username = username;
            _privateKeyPath = privateKeyPath;
            _remoteDirectory = remoteDirectory;
        }

        public async Task UploadFileAsync(string localFilePath)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Local file not found.", localFilePath);
            }

            using var privateKey = new PrivateKeyFile(_privateKeyPath);
            using var client = new SftpClient(_host, _username, new[] { privateKey });

            try
            {
                client.Connect();

                if (!client.Exists(_remoteDirectory))
                {
                    Console.WriteLine($"Error: Remote directory '{_remoteDirectory}' does not exist on server '{_host}'.");
                    throw new DirectoryNotFoundException($"Remote directory '{_remoteDirectory}' does not exist.");
                }

                using var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                var remoteFilePath = Path.Combine(_remoteDirectory, Path.GetFileName(localFilePath));
                remoteFilePath = remoteFilePath.Replace("\\", "/"); // Ensure correct path format for SFTP
                await Task.Run(() => client.UploadFile(fileStream, remoteFilePath));

                Console.WriteLine($"File '{localFilePath}' uploaded to '{remoteFilePath}' on server '{_host}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void UploadFile(string localFilePath)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Local file not found.", localFilePath);
            }

            using var privateKey = new PrivateKeyFile(_privateKeyPath);
            using var client = new SftpClient(_host, _username, new[] { privateKey });

            try
            {
                client.Connect();

                using var fileStream = new FileStream(localFilePath, FileMode.Open);
                var remoteFilePath = Path.Combine(_remoteDirectory, Path.GetFileName(localFilePath));
                client.UploadFile(fileStream, remoteFilePath);

                Console.WriteLine($"File '{localFilePath}' uploaded to '{remoteFilePath}' on server '{_host}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
            finally
            {
                client.Disconnect();
            }
        }
    }
}
