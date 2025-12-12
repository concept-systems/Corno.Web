using Corno.Web.Dtos;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Labels;

public partial class BranchLabelRpt : BaseReport
{
    public BranchLabelRpt(Carton carton, Plan plan, MasterDto warehouse, bool bDuplicate)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var oneLineItemCode = plan?.System;
        var labelId = $"1141_{oneLineItemCode}_{carton.CartonNo.ToString().PadLeft(6, '0')}";
        var qr = $@"{oneLineItemCode}|||1|||||{carton.CartonNo}|{labelId}";
        var cartonNo = carton.GetCartonNoString();
        var dataSource = new
        {
            plan?.SoNo,
            oneLineItemCode,
            StoreName = warehouse?.Name,
            CartonNo = cartonNo,
            Line1 = @"Not to be sold Separately",
            Line2 = @"To be sold only with",
            QrCode = qr
        };

        DataSource = dataSource;

        if (bDuplicate)
            txtDuplicate.Visible = true;
    }
}