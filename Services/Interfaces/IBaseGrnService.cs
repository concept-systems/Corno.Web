using System;
using System.Collections.Generic;
using Corno.Web.Dtos;
using Corno.Web.Models.Grn;

namespace Corno.Web.Services.Interfaces;

public interface IBaseGrnService : ITransactionService<Grn>
{
    #region -- Get --

    System.Threading.Tasks.Task<IEnumerable<MasterDto>> GetGrnItemListAsync(string grnNo);
    System.Threading.Tasks.Task<IEnumerable<Grn>> GetListByDateAsync(DateTime fromDate, DateTime toDate);
    System.Threading.Tasks.Task<Grn> GetByInvoiceNoAsync(string invoiceNo);
    System.Threading.Tasks.Task<Grn> GetByReceiptNoAsync(string receiptNo);
    #endregion

    #region -- Operations --
    void IncreaseQuantity(Grn grn, GrnDetail grnDetail, double quantity,
        string status);

    #endregion

    #region -- Import --

    //Task Import(string filePath, IBaseProgressService progressService, OperationRequest operationRequest = null);

    #endregion
}