using System;
using System.Collections.Generic;
using Corno.Web.Models.Base;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Base.Interfaces;

namespace Corno.Web.Services.Base;

public class PrintService<TEntity> : TransactionService<TEntity>, IPrintService<TEntity>
    where TEntity : TransactionModel, new()
{
    #region -- Constructors --

    protected PrintService(IGenericRepository<TEntity> genericRepository) : base(genericRepository)
    {
    }

    #endregion

    #region -- Data Members --

    //private readonly IBaseReportService _reportService;
    #endregion

    #region -- Event Handlers --
    public event EventHandler<IEnumerable<BaseReport>> OnPreview;
    public event EventHandler OnClearControls;
    #endregion

    #region -- General --
    /*protected virtual bool ValidateFields(OperationRequest operationRequest)
    {
        return true;
    }

    protected virtual IEnumerable<TEntity> GetEntities(OperationRequest operationRequest)
    {
        return null;
    }*/
    #endregion

    #region -- Scan --
    public virtual bool Scan(string barcode)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region -- Preview --
    /*public virtual IEnumerable<BaseReport> GetPreviewLabels(OperationRequest operationRequest)
    {
        var entities = GetEntities(operationRequest);
        operationRequest?.Set(FieldConstants.Entities, entities);
        return GetPreviewLabels(entities, operationRequest);
    }

    protected virtual IEnumerable<BaseReport> GetPreviewLabels(IEnumerable<TEntity> entities, OperationRequest operationRequest = null)
    {
        // Do Nothing
        return null;
    }

    protected virtual void PreviewLabels(IEnumerable<BaseReport> reports)
    {
        OnPreview?.Invoke(null, reports);
    }

    public virtual IEnumerable<BaseReport> PreviewLabels(OperationRequest operationRequest)
    {
        var reports = GetPreviewLabels(operationRequest).ToList();
        //operationRequest?.InputData.Add(FieldConstants.Labels, reports);
        operationRequest?.Set(FieldConstants.Labels, reports);
        OnPreview?.Invoke(null, reports);

        return reports;
    }*/
    #endregion

    #region -- Print --

    //protected virtual void Print(IEnumerable<TEntity> entities, IEnumerable<BaseReport> reports, OperationRequest operationRequest = null,
    //    bool bPrintToPrinter = true, int noOfCopies = 1 )
    //{
    //    // Print the report using the printer settings.
    //    if (bPrintToPrinter)
    //        _reportService?.PrintDirectToPrinter(reports, noOfCopies); 
    //}

    //public virtual IEnumerable<BaseReport> PrintLabels(OperationRequest operationRequest, bool bPrintToPrinter = true,
    //    int noOfCopies = 1)
    //{
    //    var entities = GetEntities(operationRequest).ToList();
    //    operationRequest.Set(FieldConstants.Entities, entities);

    //    var reports = GetPreviewLabels(entities, operationRequest).ToList();
    //    OnPreview?.Invoke(null, reports);

    //    UpdateDatabase(entities, operationRequest);

    //    // Print
    //    Print(entities, reports, operationRequest, bPrintToPrinter, noOfCopies);

    //    OnClearControls?.Invoke(null, null);

    //    return reports;
    //}

    #endregion

    #region -- Duplicate --
    //public virtual IEnumerable<BaseReport> GetDuplicateLabels(DuplicateViewModel duplicateViewModel)
    //{
    //    // Ignore
    //    return null;
    //}

    //public virtual void DuplicateLabels(DuplicateViewModel duplicateViewModel,
    //    bool bPrintToPrinter = true)
    //{
    //    var reports = GetDuplicateLabels(duplicateViewModel).ToList();
    //    OnPreview?.Invoke(null, reports);
    //    // Print the report using the printer settings.
    //    if (bPrintToPrinter)
    //        _reportService.PrintDirectToPrinter(reports);
    //    OnClearControls?.Invoke(null, null);
    //}

    //public virtual void PrintDuplicate(OperationRequest operationRequest,
    //    bool bPrintToPrinter = true)
    //{
    //    var reports = GetDuplicateLabelRpts(operationRequest).ToList();
    //    OnPreview?.Invoke(null, reports);
    //    // Print the report using the printer settings.
    //    if (bPrintToPrinter)
    //        _reportService.PrintDirectToPrinter(reports);
    //    OnClearControls?.Invoke(null, null);
    //}

    #endregion

    #region -- UI --
    //public virtual object GetLayoutDataSource(OperationRequest operationRequest)
    //{
    //    // Do Nothing
    //    return null;
    //}

    protected virtual void ClearControls()
    {
        OnClearControls?.Invoke(null, null);
    }
    #endregion
}