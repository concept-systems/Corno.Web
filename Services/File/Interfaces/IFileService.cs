using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.File.Interfaces;

public interface IFileService : IService
{
    #region -- Methods --

    void DeleteFile(string filePath);
    void MoveFile(string oldFilePath, string newFilePath);
    void MoveFileToFolder(string filePath, string newDirectory);
    void MoveFileToImportedFolder(string filePath);
    void DeleteFilesOtherThan(string directory, string extension);

    #endregion
}