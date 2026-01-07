using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corno.Web.Areas.Masters.Models;
using Corno.Web.Areas.Masters.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Location;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Services.Location;

namespace Corno.Web.Areas.Masters.Services;

public class LocationService : BaseLocationService, ILocationService
{
    #region -- Constructors --

    public LocationService(IGenericRepository<Location> genericRepository, IExcelFileService<LocationImportModel> excelFileService)
        : base(genericRepository, null, null)
    {
        _excelFileService = excelFileService;
    }

    #endregion

    #region -- Data Members --

    private readonly IExcelFileService<LocationImportModel> _excelFileService;

    #endregion

    #region -- Import --

    private async Task CreateLocationAsync(List<LocationImportModel> importModels)
    {
        // Get or Create Customer
        var first = importModels.FirstOrDefault();

        var location = new Location
        {
            Code = first?.Code,
            Name = first?.Name,
        };

        foreach (var importModel in importModels)
        {
            /*var planItemDetail = new PlanItemDetail
            {
                ItemCode = importModel.PartNo,
                Description = importModel.Description,
                OrderQuantity = importModel.OrderQuantity,
            };*/
            // Progress tracking now handled by ImportSessionService
            importModel.Status = FieldConstants.Yes;
            importModel.Remark = StatusConstants.Imported;
        }

        await AddAndSaveAsync(location).ConfigureAwait(false);
    }

    private async Task UpdatePlanAsync(Location location, List<LocationImportModel> importModels)
    {
        // Get or Create Supplier
        var first = importModels.FirstOrDefault();


        // For update, need plan to be get by Id.
        location.Code = first?.Code;
        location.Name = first?.Name;

        foreach (var importModel in importModels)
        {
            // Progress tracking now handled by ImportSessionService
            importModel.Status = FieldConstants.No;
            importModel.Remark = StatusConstants.Exists;
        }

        await UpdateAndSaveAsync(location).ConfigureAwait(false);
    }

    // OLD METHOD - REMOVED: ImportAsync with IBaseProgressService
    // This should be updated to use the new common import module
    /*
    public async Task<IEnumerable<LocationImportModel>> ImportAsync(HttpPostedFileBase file, IBaseProgressService progressService)
    {
        try
        {
            progressService.Report("Reading excel file", MessageType.Progress);
            var records = _excelFileService.Read(file.InputStream)
                .ToList().Trim();
            if (!records.Any())
                throw new Exception("No entries in excel file to import");

            // Create progress model
            progressService.Initialize(file.FileName, 0, records.Count(), 1);

            var groups = records.GroupBy(p => p.Code);
            progressService.Report("Importing records", MessageType.Progress);

            foreach (var group in groups)
            {
                var first = group.FirstOrDefault();
                var locations = await GetAsync<Location>(p => p.Code == first.Code).ConfigureAwait(false);
                var location = locations.FirstOrDefault();
                if (null != location)
                {
                    await UpdatePlanAsync(location, group.ToList(), progressService).ConfigureAwait(false);
                    continue;
                }

                // Create Plan
                await CreateLocationAsync(group.ToList(), progressService).ConfigureAwait(false);
                
            }
            progressService.Report(MessageConstants.ImportSuccess);


            return records;
        }
        catch (Exception exception)
        {
            progressService.Report(LogHandler.GetDetailException(exception)?.Message);
            throw;
        }
    }
    */

    #endregion
}

