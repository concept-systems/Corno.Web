using System;
using Corno.Web.Globals;
using Telerik.Reporting;

namespace Corno.Web.Views.Shared.Partials;

public partial class ReportViewer12 : System.Web.Mvc.ViewPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (null == Session[FieldConstants.Label])
            return;

        ReportViewer_New.ReportSource = Session[FieldConstants.Label] switch
        {
            Report => new InstanceReportSource { ReportDocument = Session[FieldConstants.Label] as Report },
            ReportBook => new InstanceReportSource { ReportDocument = Session[FieldConstants.Label] as ReportBook },
            _ => null
        };
    }
}