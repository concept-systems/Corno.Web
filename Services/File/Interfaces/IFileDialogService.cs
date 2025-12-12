using Corno.Services.Base.Interfaces;

namespace Corno.Services.File.Interfaces;

public interface IFileDialogService : IService
{
    string GetFolder();
    string GetCsvFile();
    string GetExcelFile();
    string GetImageFile();
    string SaveCsvFile();
    string SaveExcelFile();
    string GetDataFile(string title, string filter);
}