using System.IO;
using System.Linq;
using Corno.Web.Services.File.Interfaces;

namespace Corno.Web.Services.File;

public class FileService : IFileService
{
    #region -- Methods --

    public void DeleteFile(string filePath)
    {
        System.IO.File.Delete(filePath);
    }

    public void MoveFile(string oldFilePath, string newFilePath)
    {
        if (System.IO.File.Exists(newFilePath))
            DeleteFile(newFilePath);

        System.IO.File.Move(oldFilePath, newFilePath);
    }

    public void MoveFileToFolder(string filePath, string newDirectory)
    {
        // Move file to processed folder
        if (!Directory.Exists(newDirectory))
            Directory.CreateDirectory(newDirectory);

        var newFilePath = newDirectory + Path.GetFileName(filePath);
        if (System.IO.File.Exists(newFilePath))
            System.IO.File.Delete(newFilePath);
        System.IO.File.Move(filePath, newFilePath);
    }

    public void MoveFileToImportedFolder(string filePath)
    {
        // Move file to processed folder
        var oldDirectory = Path.GetDirectoryName(filePath);
        var newDirectory = oldDirectory + @"\Processed\";
        if (!Directory.Exists(newDirectory))
            Directory.CreateDirectory(newDirectory);

        var newFilePath = newDirectory + Path.GetFileName(filePath);
        if (System.IO.File.Exists(newFilePath))
            System.IO.File.Delete(newFilePath);
        System.IO.File.Move(filePath, newFilePath);
    }

    public void DeleteFilesOtherThan(string directory, string extension)
    {
        // Check if directory has pending files. If yes, the process them
        var files = Directory.GetFiles(directory ?? string.Empty)
            .Where(f => !f.EndsWith(extension));
        foreach (var file in files)
            System.IO.File.Delete(file);
    }

    #endregion
}