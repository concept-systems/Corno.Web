using System.Linq;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Labels;

public partial class CartonLabelRpt : BaseReport
{
    public CartonLabelRpt(Carton carton, Plan plan, bool bDuplicate)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var dataSource = carton.CartonDetails
                .GroupBy(g => new {g.CartonId, g.Position})
                .Select(g =>  
        {
                var firstDetail = g.FirstOrDefault();
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x =>
                    x.Position == g.Key.Position);
                var cartonNo = carton.GetCartonNoString();
            //var cartonNo = cartonService.GetCartonNo(carton.CartonNo);
                //var qr = plan?.WarehouseOrderNo + @"|" + plan?.SoNo + @"|" +
                //         carton.CartonBarcode;

                var qr = plan?.WarehouseOrderNo + @"|" + plan?.SoNo + @"|" +
                         carton.CartonBarcode;
            return new
                {
                    plan?.SoNo,
                    carton.WarehouseOrderNo,
                    firstDetail?.WarehousePosition,
                    planItemDetail?.Position,
                    CartonNo = cartonNo,
                    QrCode = qr,
                    carton.CartonBarcode,
                    planItemDetail?.ItemCode,
                    ItemName = planItemDetail?.Description,
                    SystemWeight = g.Sum(x => x.SystemWeight),
                    NetWeight = g.Sum(x => x.NetWeight),
                    Quantity = g.Sum(x => x.Quantity)
                };
            });
        DataSource = dataSource;
        if (bDuplicate)
            txtDuplicate.Visible = true;
    }
}