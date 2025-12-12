using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Base;
using Corno.Web.Services.Label.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Label;

public class BaseLabelService : PrintService<Models.Packing.Label>, IBaseLabelService {
    #region -- Constructors --
    public BaseLabelService(IGenericRepository<Models.Packing.Label> genericRepository, IBaseItemService itemService) 
    : base(genericRepository)
    {
        _itemService = itemService;

        SetIncludes(nameof(Models.Packing.Label.LabelDetails));
    }
    #endregion

    #region -- Data Members --
    private readonly IBaseItemService _itemService;
    #endregion

    #region -- Get --
    #region -- Public Methods --

    public override List<string> GetListColumns()
    {
        return new List<string> { FieldConstants.LabelDate, FieldConstants.Barcode, 
            FieldConstants.OrderQuantity, FieldConstants.Quantity };
    }

    public async Task<IEnumerable<Models.Packing.Label>> GetListByDateAsync(DateTime fromDate, DateTime toDate) {
        var list = await GetAsync<Models.Packing.Label>(c => DbFunctions.TruncateTime(c.LabelDate) >= DbFunctions.TruncateTime(fromDate) && 
                                              DbFunctions.TruncateTime(c.LabelDate) <= DbFunctions.TruncateTime(toDate), l => l).ConfigureAwait(false);
        return list;
    }

    public async Task<IEnumerable<Models.Packing.Label>> GetListByReceiptNoAsync(string receiptNo) {
        var list = await GetAsync<Models.Packing.Label>(c => c.ReceiptNo == receiptNo, l => l).ConfigureAwait(false);
        return list;
    }

    public async Task<Models.Packing.Label> GetByBarcodeAsync(string barcode) {
        if (string.IsNullOrEmpty(barcode))
            throw new Exception($"Invalid Barcode : {barcode}");

        var labels = await GetAsync<Models.Packing.Label>(b => b.Barcode == barcode, l => l).ConfigureAwait(false);
        var label = labels.OrderBy(l => l.CreatedDate).LastOrDefault();
        if (null == label)
            throw new Exception("Label with barcode value ('" + barcode + ") does not available in system.");

        return label;
    }

    public async Task<Models.Packing.Label> GetByBarcodeAsync(string barcode, ICollection<string> oldStatus, Carton carton = null) {
        if (string.IsNullOrEmpty(barcode))
            throw new Exception($"Invalid Barcode : {barcode}");

        var label = await FirstOrDefaultAsync(b => b.Barcode == barcode , b => b).ConfigureAwait(false);

        if (null == label)
            throw new Exception("Label with barcode value ('" + barcode + ") does not available in system.");
        if (oldStatus.Contains(label.Status)) return label;

        throw new Exception("Barcode(" + barcode + ") should have status : " +
                            string.Join(",", oldStatus) + ". It has current status : " +
                            label.Status);
    }

    public async Task<IEnumerable<Models.Packing.Label>> GetByPalletNoAsync(string palletNo) {
        if (string.IsNullOrEmpty(palletNo))
            throw new Exception("Invalid Pallet No");

        var labels = await GetAsync(b =>
            b.Status == StatusConstants.PalletIn, l => l, null, false).ConfigureAwait(false);
        if (!labels.Any())
            throw new Exception("Labels with pallet no ('" + palletNo + ") with status as 'Pallet In' does not available in system.");

        return labels;
    }

    private async Task<Models.Packing.Label> GetByNonUniqueBarcodeAndStatusAsync(string barcode, ICollection<string> oldStatus) {
        if (string.IsNullOrEmpty(barcode))
            throw new Exception("Invalid Barcode");

        var label = await FirstOrDefaultAsync(b => b.Barcode == barcode && oldStatus.Contains(b.Status), b => b).ConfigureAwait(false);
        if (null == label)
            throw new Exception("Label with barcode value ('" + barcode + ") does not available in system.");

        return label;
    }
    #endregion
    #endregion

    #region -- Create --
    #region -- Public Create Methods --
    public async Task<IEnumerable<Models.Packing.Label>> CreateMultipleLabelsAsync(string grnNo, int itemId, int noOfLabels, double quantity,
        int packingTypeId, double netWeight, double tareWeight) {
        var item = await _itemService.GetByIdAsync(itemId).ConfigureAwait(false);
        if (null == item)
            throw new Exception("Invalid selected item.");

        var serialNo = await GetNextSerialNoAsync().ConfigureAwait(false);
        var labels = new List<Models.Packing.Label>();
        for (var index = 0; index < noOfLabels; index++) {
            labels.Add(new Models.Packing.Label {
                SerialNo = serialNo++,
                LabelDate = DateTime.Now,
                ItemId = item.Id,
                PackingTypeId = packingTypeId,
                Quantity = quantity,
                Barcode = grnNo + "," + serialNo,
                GrnNo = grnNo,
                TareWeight = tareWeight,
                GrossWeight = netWeight + tareWeight,
                NetWeight = netWeight
            });
        }

        return labels;
    }
    #endregion
    #endregion

    #region -- Database --
    #region -- Public Methods --
    public async Task<bool> UpdateStatusAsync(string barcode, string newStatus, string userId = null) {
        var label = await GetByBarcodeAsync(barcode).ConfigureAwait(false);
        if (null == label) return false;

        label.Status = newStatus;
        label.AddDetail(null, null, null, null, newStatus, userId);
        await UpdateAsync(label).ConfigureAwait(false);

        return true;
    }

    public async Task<bool> UpdateStatusAsync(Carton carton, string newStatus, string userId = null)
    {
        var barcodes = carton.CartonDetails.Select(d => d.Barcode).Distinct();
        var labels = await GetAsync(l => barcodes.Contains(l.Barcode), l => l, null, false).ConfigureAwait(false);
        foreach (var label in labels)
        {
            label.Status = newStatus;
            label.AddDetail(null, null, null, null, newStatus, userId);
        }
        await UpdateRangeAsync(labels).ConfigureAwait(false);
            
        return true;
    }

    public async Task<bool> UpdateNonUniqueStatusAsync(string barcode, ICollection<string> oldStatus, string newStatus) {
        var label = await GetByNonUniqueBarcodeAndStatusAsync(barcode, oldStatus).ConfigureAwait(false);
        if (null == label) return false;

        label.Status = newStatus;
        await UpdateAsync(label).ConfigureAwait(false);

        return true;
    }
    #endregion
    #endregion

    #region -- Duplicate --
    #region -- Public Methods --

    public async Task<IEnumerable<MasterDto>> GetDuplicateByListAsync() {
        throw new NotImplementedException("GetDuplicateByList not implemented");
    }
    #endregion
    #endregion

    #region -- General --
    #region -- Public Methods --
    public async Task<Models.Packing.Label> PerformScanOperationAsync(Models.Packing.Label label, string newStatus)
    {
        label.Status = newStatus;

        label.LabelDetails.Add(new LabelDetail
        {
            ScanDate = DateTime.Now,
            Status = newStatus
        });

        await UpdateAsync(label).ConfigureAwait(false);

        return label;
    }

    public async Task<Models.Packing.Label> PerformScanOperationAsync(string barcode, ICollection<string> oldStatus,
        string newStatus) {
        if (string.IsNullOrEmpty(barcode))
            throw new Exception("Invalid Barcode");

        var label = await GetByBarcodeAsync(barcode, oldStatus).ConfigureAwait(false);
        await PerformScanOperationAsync(label, newStatus).ConfigureAwait(false);
            
        return label;
    }

    #endregion
    #endregion

    #region -- Sales --
    // public async Task SalesReturnAsync(OperationRequest operationRequest)
    //{
    //}
    #endregion
}