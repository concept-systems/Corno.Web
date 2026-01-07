using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Corno.Web.Areas.Kitchen.Dto.Rack_Out;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Warehouse;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Kitchen.Services;

public sealed class RackOutService : BaseRackOutService, IRackOutService
{
    #region -- Constructors --
    public RackOutService(ILabelService labelService)
    : base(null)
    {
        _labelService = labelService;
    }
    #endregion

    #region -- Data Members --

    private readonly ILabelService _labelService;

    #endregion

    #region -- Protected Methods --
    
    private static void ValidateFields(RackOutViewDto dto)
    {
        if (string.IsNullOrEmpty(dto.CartonBarcode))
            throw new Exception($"Invalid Part barcode.");
    }


    public async Task PerformRackOut(RackOutViewDto dto)
    {
        const string newStatus = StatusConstants.RackOut;
        var oldStatus = new[] { StatusConstants.RackIn, StatusConstants.RackOut };
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

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

        scope.Complete();

        // Update view data
        dto.RackOutDetails.Add(new CartonRackOutViewDto
        {
            WarehouseOrderNo = dto.WarehouseOrderNo,
            CartonBarcode = dto.CartonBarcode
        });

    }

    #endregion
}