using System.Windows.Forms;
using Corno.Services.File.Interfaces;

namespace Corno.Services.File;

public class FileDialogService : IFileDialogService
{
    #region -- Methods --
    public string GetFolder()
    {
        var folderDlg = new FolderBrowserDialog { ShowNewFolderButton = true };
        return folderDlg.ShowDialog() == DialogResult.OK ? folderDlg.SelectedPath : string.Empty;
        //var folderDlg = new RadOpenFolderDialog();
        //return folderDlg.ShowDialog() == DialogResult.OK ? folderDlg.FileName : string.Empty;
    }


    public string GetCsvFile()
    {
        var fileDialog = new OpenFileDialog
        {
            Title = @"Select CSV File",
            //InitialDirectory = @"C:\",
            Filter = @"CSV files (*.csv)|*.csv",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return fileDialog.ShowDialog() != DialogResult.OK ? null : fileDialog.FileName;
    }

    public string GetExcelFile()
    {
        var fileDialog = new OpenFileDialog
        {
            Title = @"Select Excel File",
            //InitialDirectory = @"C:\",
            Filter = @"Excel Files|*.xls;*.xlsx;*.xlsm",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return fileDialog.ShowDialog() != DialogResult.OK ? null : fileDialog.FileName;
    }

    public string GetImageFile()
    {
        var fileDialog = new OpenFileDialog
        {
            Title = @"Select Image File",
            Filter = @"Image Files|*.jpg;*.jpeg;*.png;*.tiff",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return fileDialog.ShowDialog() != DialogResult.OK ? null : fileDialog.FileName;
    }

    public string SaveCsvFile()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = @"Select / Enter CSV File",
            Filter = @"CSV files (*.csv)|*.csv",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return saveFileDialog.ShowDialog() != DialogResult.OK ? string.Empty : saveFileDialog.FileName;
    }

    public string SaveExcelFile()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = @"Select / Enter Excel File",
            Filter = @"Excel Files|*.xls;*.xlsx;*.xlsm",
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return saveFileDialog.ShowDialog() != DialogResult.OK ? string.Empty : saveFileDialog.FileName;
    }

    public string GetDataFile(string title, string filter)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = title,
            Filter = filter,
            FilterIndex = 2,
            RestoreDirectory = true
        };

        return openFileDialog.ShowDialog() != DialogResult.OK ? string.Empty : openFileDialog.FileName;
    }
    #endregion
}