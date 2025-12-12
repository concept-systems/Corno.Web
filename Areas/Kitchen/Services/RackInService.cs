using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Rack_In;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.File.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Corno.Web.Areas.Kitchen.Services;

public sealed class RackInService : LabelService, IRackInService
{
    #region -- Constructors --
    public RackInService(IGenericRepository<Label> genericRepository, ILabelService labelService, IExcelFileService<SalvaginiExcelDto> excelFileService, IUserService userService)
        : base(genericRepository, excelFileService, userService)
    {
        _labelService = labelService;
    }
    #endregion

    #region -- Data Members --

    private readonly ILabelService _labelService;
    
    #endregion

    #region -- Protected Methods --
    private static void ValidateFields(RackInViewDto dto)
    {
        if (string.IsNullOrEmpty(dto.RackNo))
            throw new Exception($"Invalid Rack No.");

        if (string.IsNullOrEmpty(dto.CartonBarcode))
            throw new Exception($"Invalid Item barcode.");
    }

    public async Task PerformRackIn(RackInViewDto dto)
    {
        const string newStatus = StatusConstants.RackIn;
        var oldStatus = new[] { StatusConstants.RackIn, StatusConstants.RackIn };
        var label = await _labelService.FirstOrDefaultAsync(d => d.Barcode == dto.CartonBarcode, d => d).ConfigureAwait(false);
        if (!oldStatus.Contains(label.Status))
            throw new Exception($"Required Rack In. Current Status {label.Status}.");

        if (label.Status != StatusConstants.Bent)
            throw new Exception($"Expected label status is 'Active', but current label status is {label.Status}");

        var planService = Bootstrapper.Get<IPlanService>();
        var plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
        
        // Add label detail
        var labelDetail = new LabelDetail
        {
            ScanDate = DateTime.Now,
            LabelId = label.Id,

            CreatedBy = HttpContext.Current.User.Identity.GetUserId(),
            ModifiedBy = HttpContext.Current.User.Identity.GetUserId(),

            Status = newStatus
        };

        label.LabelDetails.Add(labelDetail);
        label.Status = newStatus;

        await planService.UpdateAsync(plan).ConfigureAwait(false);
        await _labelService.UpdateAndSaveAsync(label).ConfigureAwait(false);
        // Update view data
        dto.RackInDetails.Add(new CartonRackInViewDto()
        {
            PalletNo = dto.PalletNo,
            RackNo = dto.RackNo,
        });

    }


    #endregion
}