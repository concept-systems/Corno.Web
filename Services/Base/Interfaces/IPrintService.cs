using System;
using System.Collections.Generic;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Base.Interfaces;

public interface IPrintService<TEntity> : ITransactionService<TEntity>
    where TEntity : class
{
    #region -- Event Handlers --
    event EventHandler<IEnumerable<BaseReport>> OnPreview;
    event EventHandler OnClearControls;
    #endregion

    #region -- Scan --
    bool Scan(string barcode);
    #endregion

    #region -- Preview --
    //IEnumerable<BaseReport> GetPreviewLabels(OperationRequest operationRequest);

    //IEnumerable<BaseReport> PreviewLabels(OperationRequest operationRequest);
    #endregion

    #region -- Print --
    //IEnumerable<BaseReport> PrintLabels(OperationRequest operationRequest, bool bPrintToPrinter = true,
        //int noOfCopies = 1);
    #endregion

    #region -- Duplicate --
    //IEnumerable<BaseReport> GetDuplicateLabels(DuplicateViewModel viewModel);
    //IEnumerable<BaseReport> GetDuplicateLabelRpts(OperationRequest operationRequest);

    //void DuplicateLabels(DuplicateViewModel viewModel, bool bPrintToPrinter = true);

    //void PrintDuplicate(OperationRequest operationRequest, bool bPrintToPrinter = true);
    #endregion

    #region -- Delete --
    //void Delete(OperationRequest operationRequest);
    #endregion

    #region -- UI --
    //object GetLayoutDataSource(OperationRequest operationRequest);
    #endregion
}