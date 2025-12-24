using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Dtos;
using Corno.Web.Globals;
using Corno.Web.Models.Grn;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services;

public class BaseGrnService : TransactionService<Grn>, IBaseGrnService
{
    #region -- Constructors --
    public BaseGrnService(IGenericRepository<Grn> genericRepository, IBaseItemService itemService)
    : base(genericRepository)
    {
        _itemService = itemService;

        // Do not enable includes globally; use explicit methods when details are needed.
        SetIncludes(nameof(Grn.GrnDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly IBaseItemService _itemService;
    #endregion

    #region -- Get --
    #region -- Public Methods --
    public async System.Threading.Tasks.Task<IEnumerable<MasterDto>> GetGrnItemListAsync(string grnNo)
    {
        var grn = (await GetAsync<object>(g => g.Code == grnNo, g => new { g.GrnDetails }).ConfigureAwait(false))
            .FirstOrDefault();
        if (null == grn) return null;
       /* return from grnDetail in grn.GrnDetails
            join item in _itemService.GetViewModelList() on grnDetail?.ItemId equals item.Id
            select item
            ;*/
       return await System.Threading.Tasks.Task.FromResult<IEnumerable<MasterDto>>(null).ConfigureAwait(false);
    }

    public async System.Threading.Tasks.Task<IEnumerable<Grn>> GetListByDateAsync(DateTime fromDate, DateTime toDate)
    {
        // Read-only; do not load GrnDetails by default
        return await GetAsync<Grn>(p => DbFunctions.TruncateTime(p.GrnDate) >=
                             DbFunctions.TruncateTime(fromDate) &&
                             DbFunctions.TruncateTime(p.GrnDate) <=
                             DbFunctions.TruncateTime(toDate), p => p, null, ignoreInclude: true)
            .ConfigureAwait(false);
    }

    public async System.Threading.Tasks.Task<Grn> GetByInvoiceNoAsync(string invoiceNo)
    {
        var list = await GetAsync<Grn>(p => p.InvoiceNo.Trim() == invoiceNo.Trim(), p => p, null, ignoreInclude: true)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }

    public async System.Threading.Tasks.Task<Grn> GetByReceiptNoAsync(string receiptNo)
    {
        var list = await GetAsync<Grn>(p => p.Code.Trim() == receiptNo.Trim(), p => p, null, ignoreInclude: true)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }

    /// <summary>
    /// Get GRN with details when required by caller.
    /// </summary>
    public async System.Threading.Tasks.Task<Grn> GetByIdWithDetailsAsync(int id)
    {
        //SetIncludes(nameof(Grn.GrnDetails));
        var list = await GetAsync<Grn>(g => g.Id == id, g => g, null, ignoreInclude: false)
            .ConfigureAwait(false);
        return list.FirstOrDefault();
    }
    #endregion
    #endregion

    #region -- Operations --
    public void IncreaseQuantity(Grn grn, GrnDetail grnDetail, double quantity,
        string status)
    {
        switch (status)
        {
            case StatusConstants.Active:
            case StatusConstants.Printed:
                grnDetail.PrintQuantity = (grnDetail.PrintQuantity ?? 0) + quantity;
                break;
            case StatusConstants.QcDone:
                grnDetail.Q1Quantity = (grnDetail.Q1Quantity ?? 0) + quantity;
                break;
            case StatusConstants.RackIn:
                grnDetail.RackInQuantity = (grnDetail.RackInQuantity ?? 0) + quantity;
                break;
            case StatusConstants.RackOut:
                grnDetail.RackOutQuantity = (grnDetail.RackOutQuantity ?? 0) + quantity;
                break;
            default:
                throw new Exception($"Invalid update detail operation {status}");
        }

        grn.Status = StatusConstants.InProcess;
    }
    #endregion

    #region -- Import --

    /*public virtual async Task Import(string filePath, IBaseProgressService progressService, OperationRequest operationRequest = null)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }*/

    #endregion
}