using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Labels;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Microsoft.AspNet.Identity;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Services;

public sealed class LabelService : BaseService<Label>, ILabelService
{
    #region -- Constructors --
    public LabelService(IGenericRepository<Label> genericRepository, IBaseItemService itemService, IMiscMasterService miscMasterService)
        : base(genericRepository)
    {
        _genericRepository = genericRepository;
        _itemService = itemService;
        _miscMasterService = miscMasterService;

        SetIncludes(nameof(Label.LabelDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly IGenericRepository<Label> _genericRepository;
    private readonly IBaseItemService _itemService;
    private readonly IMiscMasterService _miscMasterService;

    private const string NewStatus = StatusConstants.Printed;

    #endregion

    #region -- Protected Methods --

    protected bool ValidateDto(LabelDto dto)
    {
        if (dto.ItemId <= 0)
            throw new Exception("Invalid item.");

        return true;
    }

    #endregion

    #region -- Public Methods --

    public async Task<List<Label>> CreateLabelsAsync(LabelDto dto)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        // 2. Get Item (async)
        var item = await _itemService.FirstOrDefaultAsync(p => p.Id == dto.ItemId, p => p).ConfigureAwait(false);
        if (item == null)
            throw new Exception("Item not found.");

        var itemPacketDetail = item.ItemPacketDetails.FirstOrDefault();
        if (null == itemPacketDetail)
            throw new Exception("Packing type not available in system");

        // 3. Get Max SerialNo (async)
        var maxSerialNo = await _genericRepository.MaxAsync(null, c => c.SerialNo).ConfigureAwait(false);
        var serialNo = maxSerialNo + 1;

        // Resolve user id safely
        var userId = HttpContext.Current?.User?.Identity?.GetUserId() ?? "System";

        // 4. Create Labels
        var labels = new List<Label>();
        for (var index = 0; index < dto.Weight; index++)
        {
            var label = new Label
            {
                SerialNo = serialNo++,
                LabelDate = DateTime.Now,
                ItemId = item.Id,
                NetWeight = itemPacketDetail.Quantity,

                PackingTypeId = itemPacketDetail.Id,
                Barcode = $"{serialNo}",
                Quantity = dto.Weight,
                CreatedBy = userId,
                ModifiedBy = userId,
                Status = NewStatus,
            };

            label.LabelDetails.Add(new LabelDetail()
            {
                ScanDate = DateTime.Now,
                CreatedBy = userId,
                ModifiedBy = userId,
                Status = NewStatus,
            });

            label.SetProperty(FieldConstants.ManufacturingDate, dto.ManufacturingDate);
            label.SetProperty("ExpiryDate", dto.ExpiryDate);
            label.SetProperty(FieldConstants.Rate, dto.Mrp);

            labels.Add(label);
        }

        return labels;
    }

    public async Task<BaseReport> CreateLabelReportAsync(List<Label> labels, Item item, bool bDuplicate)
    {
        // Simulate async path for report creation if needed
        var labelRpt = new LabelRpt(labels, item);
        return await Task.FromResult<BaseReport>(labelRpt).ConfigureAwait(false);
    }

    public async Task UpdateDatabaseAsync(List<Label> labels)
    {
        if (labels == null || !labels.Any())
            throw new Exception("Invalid labels");

        // Save labels to database (async)
        await AddRangeAsync(labels).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    #endregion
}
