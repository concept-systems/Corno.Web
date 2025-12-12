using System;
using Corno.Web.Globals;
using Telerik.Reporting;

namespace Corno.Web.Views.Shared.Partials;

public partial class ReportViewer1 : System.Web.Mvc.ViewPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (null == Session[FieldConstants.Label])
            return;
        ReportViewer11.ReportSource = null;
        ReportViewer11.ReportSource = new InstanceReportSource
        {
            ReportDocument = Session[FieldConstants.Label] as Report
        };

        // if (null == Session[FieldConstants.Barcode]) return;

        //var instanceReportSource = new InstanceReportSource
        //{
        //    ReportDocument = ((Report)Session[FieldConstants.MaterialInward])
        //};
        //var reportSource = new Telerik.ReportViewer.Html5.WebForms.ReportSource
        //{
        //    IdentifierType = IdentifierType.TypeReportSource,
        //    Identifier = Session[FieldConstants.MaterialInward].ToString(),
        //    Parameters = { }
        //};

        //if (HttpContext.Current?.Session != null)
        //{
        //    ReportViewer11.ReportSource = new InstanceReportSource
        //    {
        //        ReportDocument = HttpContext.Current?.Session[name: FieldConstants.Barcode] as Report
        //    };
        //}
    }

}