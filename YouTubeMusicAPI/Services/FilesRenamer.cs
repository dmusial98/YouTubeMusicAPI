using System.IO;
using YouTubeMusicAPI.Services.Interfaces;

namespace YouTubeMusicAPI.Services;

public class FilesRenamer : IFilesRenamer
{
    public void RenameFiles(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath, "*.mp3");

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            if (fileName.StartsWith("NA - "))
            {
                var newFileName = fileName.Substring(5); // Remove "NA - "
                var newFilePath = Path.Combine(directoryPath, newFileName);
                File.Move(file, newFilePath);
            }
        }
    }
}