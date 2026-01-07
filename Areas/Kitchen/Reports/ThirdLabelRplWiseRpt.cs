using System;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Globals.Enums;
using Corno.Web.Reports;
using Corno.Web.Windsor;

namespace Corno.Web.Areas.Kitchen.Reports;

public partial class ThirdLabelRplWiseRpt : BaseReport
{
    #region -- Constructors --
    public ThirdLabelRplWiseRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }
    #endregion

        private void ThirdLabelRplWiseRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromDate = report.Parameters[FieldConstants.FromDate].Value.ToDateTime();
        var toDate = report.Parameters[FieldConstants.ToDate].Value.ToDateTime();

        var labelType = nameof(LabelType.SubAssembly);

        var labelService = Bootstrapper.Get<ILabelService>();
        var userService = Bootstrapper.Get<IUserService>();

        // IMPORTANT:
        // Do NOT join IQueryable from different services (different DbContexts) in a single LINQ-to-Entities query.
        // That caused: "The specified LINQ expression contains references to queries that are associated with different contexts."
        // Instead, execute each query separately (ToList) and join in memory (LINQ-to-Objects), similar to the newer reports.

        var labelQuery = labelService.GetQuery();
        var userQuery = userService.GetQuery();

        // First, get labels from the database (single context) with the required filters and projected fields.
        var labels = labelQuery
            .Where(label =>
                DbFunctions.TruncateTime(label.LabelDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(label.LabelDate) <= DbFunctions.TruncateTime(toDate) &&
                label.LabelType == labelType)
            .Select(label => new
            {
                label.LabelDate,
                label.Reserved1,
                label.WarehouseOrderNo,
                label.Position,
                label.Barcode,
                label.Quantity,
                label.ModifiedBy
            })
            .ToList();

        if (labels.Count == 0)
        {
            table1.DataSource = null;
            return;
        }

        // Then, get users from their own context and project only the needed fields.
        var users = userQuery
            .Select(user => new
            {
                user.Id,
                user.UserName
            })
            .ToList();

        // Finally, join labels and users in memory.
        var dataList = (from label in labels
                        join user in users on label.ModifiedBy equals user.Id into userGroup
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
                        })
            .ToList();

        table1.DataSource = dataList.Count > 0 ? dataList : null;
    }
}