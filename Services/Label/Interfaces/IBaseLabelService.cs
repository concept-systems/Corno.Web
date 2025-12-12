using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Dtos;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Base.Interfaces;

namespace Corno.Web.Services.Label.Interfaces;

public interface IBaseLabelService : IPrintService<Models.Packing.Label>
{
    #region -- Get --
    Task<IEnumerable<Models.Packing.Label>> GetListByDateAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<Models.Packing.Label>> GetListByReceiptNoAsync(string receiptNo);
    Task<Models.Packing.Label> GetByBarcodeAsync(string barcode);
    Task<Models.Packing.Label> GetByBarcodeAsync(string barcode, ICollection<string> oldStatus, Carton carton = null);
    Task<IEnumerable<Models.Packing.Label>> GetByPalletNoAsync(string palletNo);
    #endregion

    #region -- Create --
    Task<IEnumerable<Models.Packing.Label>> CreateMultipleLabelsAsync(string grnNo, int itemId, int noOfLabels, double quantity,
        int packingTypeId, double netWeight, double tareWeight);
    #endregion

    #region -- Database --
    Task<bool> UpdateStatusAsync(string barcode, string status, string userId = null);
    Task<bool> UpdateStatusAsync(Carton carton, string newStatus, string userId = null);
    Task<bool> UpdateNonUniqueStatusAsync(string barcode, ICollection<string> oldStatus, string newStatus);
    #endregion

    #region -- Database --
    Task<IEnumerable<MasterDto>> GetDuplicateByListAsync();
    #endregion

    #region -- General --
    Task<Models.Packing.Label> PerformScanOperationAsync(Models.Packing.Label label, string newStatus);
    Task<Models.Packing.Label> PerformScanOperationAsync(string barcode, ICollection<string> oldStatus, string newStatus);
    #endregion

    #region -- Sales --
    // Task SalesReturnAsync(OperationRequest operationRequest);
    #endregion
}