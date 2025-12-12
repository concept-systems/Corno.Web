using System;
using System.Collections.Generic;
using System.IO;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Services.File.Interfaces;
using Ganss.Excel;

namespace Corno.Web.Services.File;

public class ExcelFileService<TEntity> : IExcelFileService<TEntity>
{
    #region -- Private Methods --

    //private void CheckErrors(IEnumerable<TEntity> data)
    //{
    //    var firstException = (data as IEnumerable<IAllowFieldTypeConversionExceptions>)?
    //        .Where(d => d.FieldTypeConversionExceptions.Count() > 0)
    //        .FirstOrDefault();

    //    if (firstException == null)
    //        return;

    //    var message = string.Empty;
    //    foreach (var exception in firstException.FieldTypeConversionExceptions)
    //        message = $"Row ({exception.Row}), Column ({exception.ColumnName}) -> {exception.InnerException?.Message} {Environment.NewLine}";

    //    throw new Exception(message);
    //}
    #endregion

    #region -- Methods --
    // Excel Mapper
    public IEnumerable<TEntity> Read(string fileName, int sheetIndex = 0, int headerRowNo = 0)
    {
        try
        {
            var mapper = new ExcelMapper
            {
                HeaderRowNumber = headerRowNo,
                MinRowNumber = headerRowNo + 1,
                SkipBlankRows = true,
                TrackObjects = true,
            };
            var data = mapper.Fetch<TEntity>(fileName, sheetIndex);

            // Check if any errors found.
            //CheckErrors(data);
            return data;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            throw;
        }
    }

    public IEnumerable<TEntity> Read(Stream fileStream, int sheetIndex = 0, int headerRowNo = 0)
    {
        try
        {
            var mapper = new ExcelMapper
            {
                HeaderRowNumber = headerRowNo,
                MinRowNumber = headerRowNo + 1,
                SkipBlankRows = true,
                TrackObjects = true,
            };
            var data = mapper.Fetch<TEntity>(fileStream, sheetIndex);

            // Check if any errors found.
            //CheckErrors(data);
            return data;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            throw;
        }
    }

    public IEnumerable<TEntity> Read(string fileName, string sheetName = FieldConstants.Sheet1,
        int headerRowNo = 0)
    {
        try
        {
            var mapper = new ExcelMapper
            {
                HeaderRowNumber = headerRowNo,
                MinRowNumber = headerRowNo + 1,
                SkipBlankRows = true,
            };
            var data = mapper.Fetch<TEntity>(fileName, sheetName);

            // Check if any errors found.
            //CheckErrors(data);
            return data;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            throw;
        }
    }

    public void Save(string fileName, string sheetName, IEnumerable<TEntity> data)
    {
        // Keep this try Catch
        var mapper = new ExcelMapper();
        mapper.SaveAsync(fileName, data, sheetName);
    }

    public void Save(string fileName, int sheetNo, IEnumerable<TEntity> data)
    {
        // Keep this try Catch
        var mapper = new ExcelMapper();
        mapper.SaveAsync(fileName, data, sheetNo);
    }

    /*public void ChangeCellColor(string filePath, int sheetNo, IEnumerable<string> data)
    {
        // Load an existing Excel file or create a new one
        using var package = new ExcelPackage(new FileInfo(filePath));
        // If you use EPPlus in a noncommercial context
        // according to the Polyform Noncommercial license:
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        // Select the worksheet where you want to change cell colors
        var worksheet = package.Workbook.Worksheets[0]; // Replace with your sheet name

        // Define the color you want to set (e.g., red)
        var color = System.Drawing.Color.Red; // Replace with the desired color
        foreach (var targetValue in data)
        {
            // Loop through the worksheet to find and highlight cells with the target value
            foreach (var cell in worksheet.Cells)
            {
                if (cell.Text != targetValue) continue;
                // Set the cell's background color
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(color);
            }
        }

        // Save the changes
        package.Save();
    }*/
    #endregion
}