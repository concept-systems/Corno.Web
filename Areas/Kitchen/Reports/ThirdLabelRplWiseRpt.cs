using System;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Reports;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ThirdLabelRplWiseRpt : BaseReport
{
    #region -- Constructors --
    public ThirdLabelRplWiseRpt(ILabelService labelService, IUserService userService,IPlanService planService)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _labelService = labelService;
        _userService = userService;
        _planService = planService;


    }
    #endregion

    #region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly IUserService _userService;
    private readonly IPlanService _planService;

    #endregion

    private void ThirdLabelRplWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate].Value.ToDateTime();
        var toDate = report.Parameters[FieldConstants.ToDate].Value.ToDateTime();

        var labelType = LabelType.SubAssembly.ToString();
        var labelQuery = _labelService.GetQuery();
        var userQuery = _userService.GetQuery();
        var dataSource = from label in labelQuery 
                         where DbFunctions.TruncateTime(label.LabelDate) >= DbFunctions.TruncateTime(fromDate) &&
                               DbFunctions.TruncateTime(label.LabelDate) <= DbFunctions.TruncateTime(toDate) &&
                               label.LabelType == labelType
                        
                         join user in userQuery on label.ModifiedBy equals user.Id into userGroup
                         from user in userGroup.DefaultIfEmpty()
                         select new
                         {
                             UserName = user != null ? user.UserName : null,
                             label.LabelDate,

                             Family = label.Reserved1,
                             label.WarehouseOrderNo,
                             label.Position,
                             label.Barcode,
                             label.Quantity
                         };

        var dataList = dataSource.ToList();
        table1.DataSource = dataList is { Count: > 0 } ? dataList : null;
    }
}