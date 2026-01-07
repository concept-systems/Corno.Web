using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Windsor;
using Telerik.Reporting;

namespace Corno.Web.Areas.Kitchen.Services;

public class ReportService : BaseReportService, IReportService
{
    #region -- Constructors --

    public ReportService(IPlanService planService, ICartonService cartonService, IUserService userService)
    {
        _planService = planService;
        _cartonService = cartonService;
        _userService = userService;
    }
    #endregion

    #region -- Data Members --
    private readonly IPlanService _planService;
    private readonly ICartonService _cartonService;
    private readonly IUserService _userService;
    #endregion

    #region -- Public Methods (Parameters)
    public void FillUsersParameter(ReportParameterCollection parameters,
        string methodName, Type reportType)
    {
        var objectDataSource = new ObjectDataSource
        {
            DataMember = methodName,
            DataSource = reportType
        };
        objectDataSource.Parameters.Add(new ObjectDataSourceParameter("fromDate", typeof(DateTime), "=Parameters.FromDate"));
        objectDataSource.Parameters.Add(new ObjectDataSourceParameter("toDate", typeof(DateTime), "=Parameters.ToDate"));
        parameters[FieldConstants.User].AvailableValues.DataSource = objectDataSource;
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateUsers(DateTime fromDate, DateTime toDate)
    {
        // Use RunAsync helper to avoid deadlock in synchronous method
        var userIds = RunAsync(() => _cartonService.GetAsync(c =>
                    DbFunctions.TruncateTime(c.PackingDate) >= DbFunctions.TruncateTime(fromDate) &&
                    DbFunctions.TruncateTime(c.PackingDate) <= DbFunctions.TruncateTime(toDate), 
                c => c.ModifiedBy))
            .Distinct();

        var users = RunAsync(() => _userService.GetAsync(u => 
            userIds.Contains(u.Id), u => new { u.Id, u.UserName })).ToList();
        return users;
    }

    /*public void FillUsersParameter(ReportParameter parameter)
    {
        if (parameter == null)
            return;

        var identityService = Bootstrapper.Get<IIdentityService>();

        parameter.AvailableValues.ValueMember = "=Fields.Id";
        parameter.AvailableValues.DisplayMember = "=Fields.Name";
        parameter.AvailableValues.DataSource = identityService.GetUsers()
            .Select(u => new
            {
                u.Id,
                Name = u.UserName
            });
        parameter.MultiValue = false;
    }*/

    public void FillLocationParameter(ReportParameter parameter)
    {
        if (parameter == null) return;

        // Static list - NO expressions needed for ValueMember/DisplayMember
        parameter.AvailableValues.DataSource = new object[]
        {
            new { Id = 0, Name = "All" },
            new { Id = 1, Name = "Shirwal" },
            new { Id = 2, Name = "Khalapur" }
        };

        // Use property names directly (no "=Fields.*" expressions)
        parameter.AvailableValues.ValueMember = "Id";      // Property name
        parameter.AvailableValues.DisplayMember = "Name";  // Property name

        /*if (parameter == null)
            return;
        parameter.AvailableValues.ValueMember = "=Fields.Id";
        parameter.AvailableValues.DisplayMember = "=Fields.Name";
        parameter.AvailableValues.DataSource = new List<MasterDto>
        {
            new() {Id = 0, Name = "All"},
            new() {Id = 1, Name = "Shirwal"},
            new() {Id = 2, Name = "Khalapur"},
        };*/
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateLotNos(DateTime dueDate)
    {
        // Use RunAsync helper to avoid deadlock in synchronous method
        return RunAsync(() => _planService.GetAsync<object>(p => DbFunctions.TruncateTime(p.DueDate) ==
                                     DbFunctions.TruncateTime(dueDate),
                p => new { p.LotNo }))
            .ToArray();
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateWarehouseOrderNos(string lotNo)
    {
        // Use RunAsync helper to avoid deadlock in synchronous method
        return RunAsync(() => _planService.GetAsync<object>(p => p.LotNo == lotNo,
                p => new { p.WarehouseOrderNo }))
            .ToArray();
    }

    [DataObjectMethod(DataObjectMethodType.Select)]
    public IEnumerable UpdateFamilies(string warehouseOrderNo)
    {
        // Use RunAsync helper to avoid deadlock in synchronous method
        var details = RunAsync(() => _planService.FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo,
            p => p.PlanItemDetails));
        var families = details.Select(d => new {Family = d.Group}).Distinct().ToList();
        return families;
    }


    #endregion
}