using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Reports;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using System;
using System.Collections;
using System.Linq;
using MoreLinq;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class HandoverRpt : BaseReport
{
    #region -- Constructrs --
    public HandoverRpt()
    {
        InitializeComponent();

        var reportService = Bootstrapper.Get<IReportService>();
        reportService.FillLocationParameter(ReportParameters[FieldConstants.Location]);
    }
    #endregion

    #region -- Private Methods --
    private IEnumerable GetDataSource(string warehouseOrderNo, int locationId, bool bUpdateQuantity)
    {
        //Bootstrapper.StaticContainer.BeginLifetimeScope();

        var planService = Bootstrapper.Get<IPlanService>();
        var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        var itemService = Bootstrapper.Get<IBaseItemService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        if (string.IsNullOrEmpty(warehouseOrderNo))
            return null;

        var plan = RunAsync(() => planService.GetByWarehouseOrderNoAsync(warehouseOrderNo));
        /*if (bUpdateQuantity)
            RunAsync(() => planService.UpdatePackQuantitiesAsync(plan));*/

        var families = RunAsync(() => planService.GetFamiliesAsync(locationId, plan));
        var planItemDetails = plan.PlanItemDetails.Where(d => families.Contains(d.Group)).ToList();
        var warehouse = RunAsync(() => miscMasterService.GetViewModelAsync(plan.WarehouseId ?? 0));

        var distinctParentItemIds = planItemDetails.Select(d => d.ParentItemId)
            .Distinct();
        var parentItems = RunAsync(() => itemService.GetViewModelListAsync(i => distinctParentItemIds.Contains(i.Id)))
            .ToList();
        var cartons = RunAsync(() => cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
                c => new { c.Id, c.PackingDate, c.WarehouseOrderNo, c.CartonNo, c.CartonDetails })).ToList();
        var cartonDetails = cartons.SelectMany(p => p.CartonDetails, (p, d) => new
        {
            p.CartonNo,
            p.PackingDate,
            d.Position,
            Quantity = d.Quantity ?? 0
        }).ToList();
        var dataSource = (from planItemDetail in planItemDetails
                          join parentItem in parentItems
                              on planItemDetail?.ParentItemId equals parentItem?.Id into defaultParentItem
                          from parentItem in defaultParentItem.DefaultIfEmpty()
                          join cartonDetail in cartonDetails
                              on planItemDetail.Position equals cartonDetail.Position into groupedCartons
                          from cartonDetail in groupedCartons.DefaultIfEmpty()
                          select new
                          {
                              OneLineItemCode = plan.System,
                              plan.WarehouseOrderNo,
                              plan.SoNo,
                              BranchName = warehouse?.Name,
                              Family = planItemDetail.Group,

                              planItemDetail.WarehousePosition,
                              planItemDetail.Position,

                              CartonNo = cartonDetail != null ? $"C{cartonDetail.CartonNo.ToString().PadLeft(3, '0')}" : string.Empty,

                              ParentItemCode = parentItem?.Code,
                              planItemDetail.ItemCode,
                              ItemName = planItemDetail.Description,
                              planItemDetail.DrawingNo,

                              PackDate = cartonDetail?.PackingDate?.ToString("dd/MM/yyyy"),
                              OrderQuantity = planItemDetail.OrderQuantity ?? 0,
                              PackQuantity = (double?)(cartonDetail?.Quantity ?? 0)
                          })
    .ToList();

        // Add unpacked quantity rows if needed
        var unpackedRows = planItemDetails
            .GroupJoin(cartonDetails, pid => pid.Position, cd => cd.Position, (pid, cds) => new
            {
                PlanItem = pid,
                PackedQuantity = cds.Sum(cd => cd.Quantity)
            })
            .Where(x => (x.PlanItem.OrderQuantity ?? 0) > x.PackedQuantity)
            .Select(x => new
            {
                OneLineItemCode = plan.System,
                plan.WarehouseOrderNo,
                plan.SoNo,
                BranchName = warehouse?.Name,
                Family = x.PlanItem.Group,

                x.PlanItem.WarehousePosition,
                x.PlanItem.Position,

                CartonNo = string.Empty, // Unpacked
                ParentItemCode = parentItems.FirstOrDefault(p => p.Id == x.PlanItem.ParentItemId)?.Code,
                x.PlanItem.ItemCode,
                ItemName = x.PlanItem.Description,
                x.PlanItem.DrawingNo,

                PackDate = string.Empty,
                OrderQuantity = x.PlanItem.OrderQuantity ?? 0,
                PackQuantity = (double?)null//x.PlanItem.OrderQuantity - x.PackedQuantity
            }).ToList();

        dataSource.AddRange(unpackedRows);
        return !dataSource.Any() ? null : 
            dataSource.DistinctBy(d => new { d.CartonNo, d.Position });
        //var packedItems = cartons.SelectMany(cd =>
        //        cd.CartonDetails, (carton, cartonDetail) =>
        //        {
        //            var planItemDetail = planItemDetails.FirstOrDefault(x => x.Position == cartonDetail?.Position);
        //            if (null == planItemDetail) return null;
        //            var parentItem = parentItems.FirstOrDefault(x => x.Id == planItemDetail?.ParentItemId);
        //            return new
        //            {
        //                OneLineItemCode = plan.System,
        //                plan.WarehouseOrderNo,
        //                plan.SoNo,
        //                BranchName = warehouse?.Name,
        //                Family = planItemDetail.Group,

        //                planItemDetail.WarehousePosition,
        //                planItemDetail.Position,

        //                CartonNo = $"C{carton.CartonNo.ToString().PadLeft(3, '0')}" as object,

        //                ParentItemCode = parentItem?.Code,
        //                planItemDetail.ItemCode,
        //                ItemName = planItemDetail.Description,
        //                planItemDetail.DrawingNo,

        //                PackDate = carton.PackingDate?.ToString("dd/MM/yyyy"),
        //                planItemDetail.OrderQuantity,
        //                PackQuantity = cartonDetail?.Quantity ?? 0
        //            };
        //        })
        //    .Where(d => null != d).ToList();

        //var unpackedItems = (from planItemDetail in planItemDetails.Where(d =>
        //        (d.OrderQuantity ?? 0) > (d.PackQuantity ?? 0))
        //                     join parentItem in parentItems on planItemDetail?.ParentItemId equals parentItem?.Id into defaultParentItem
        //                     from parentItem in defaultParentItem.DefaultIfEmpty()
        //                     select new
        //                     {
        //                         OneLineItemCode = plan.System,
        //                         plan.WarehouseOrderNo,
        //                         plan.SoNo,
        //                         BranchName = warehouse?.Name,
        //                         Family = planItemDetail?.Group,

        //                         planItemDetail?.WarehousePosition,
        //                         planItemDetail?.Position,

        //                         CartonNo = null as object,

        //                         ParentItemCode = parentItem?.Code,
        //                         planItemDetail?.ItemCode,
        //                         ItemName = planItemDetail?.Description,
        //                         planItemDetail?.DrawingNo,

        //                         PackDate = string.Empty,
        //                         planItemDetail?.OrderQuantity,
        //                         PackQuantity = (double)0
        //                     }).ToList();

        /*var dataSource = packedItems.Concat(unpackedItems).ToList();
        return dataSource;*/
    }

    private void HandleNeedDataSource(object sender)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var warehouseOrderNo = report.Parameters[FieldConstants.WarehouseOrderNo]
            .Value.ToString();
        var locationId = report.Parameters[FieldConstants.Location].Value.ToString().ToInt();
        var bUpdateQuantity = report.Parameters["UpdateQuantities"].Value.ToBoolean();

        var dataSource = GetDataSource(warehouseOrderNo, locationId, bUpdateQuantity);
        if (dataSource == null)
            return;
        
        var dataList = dataSource.Cast<object>().ToList();
        report.DataSource = dataList.Count > 0 ? dataList : null;
    }


    #endregion

    #region -- Events --
    private void HandoverRpt_NeedDataSource(object sender, EventArgs e)
    {
        try
        {
            HandleNeedDataSource(sender);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

    }
    #endregion
}