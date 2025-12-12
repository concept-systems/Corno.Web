<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="Corno.Web.Views.Shared.Partials.ReportViewer1" %>



<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=17.0.23.118, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<!DOCTYPE html>

<%--<style>
    .ReportViewer:-webkit-full-screen {
        width: 100% !important;
        height: 100% !important;
    }
</style>

<telerik:RadScriptManager runat="server" ID="RadScriptManager1">
    <Scripts>
        <%--This script refernece is only needed if you do not use any Telerik.Web.UI controls on the page
        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
    </Scripts>
</telerik:RadScriptManager>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<%--<asp:Button Text="Put the Spreadsheet in Full Screen" ID="btn1" OnClientClick="FullScreen(); return false;" runat="server" />--%>
    <telerik:ReportViewer ID="ReportViewer11" runat="server" ViewMode="PrintPreview"
        ShowPrintButton="True" ShowPrintPreviewButton="True" DocumentMapVisible="False"
        ShowHistoryButtons="True"
        Width="100%" Height="100%">
        <ClientEvents PrintBegin="onPrintBegin()"
                      PrintEnd="onPrintEnd()" >
        </ClientEvents>
    </telerik:ReportViewer>
    <!--<input type="button" value="Test" onclick="printToPrinter();"/>-->
</body>
</html>

<script type="text/javascript">
    //ReportViewer.OnReportLoadedOld = ReportViewer.OnReportLoadedOld;
    //ReportViewer.Prototype.OnReportLoadedOld = function() {
    //    this.OnReportLoadedOld();
    //    var printButton = document.getElementById("PrintButton");
    //    printButton.disabled = false;
    //}
    ReportViewer.prototype.printReport = function() {
        this.PrintAs("Default");
    }

    function onPrintBegin() {
        alert("inward print begin");
        $('#form').submit();
    }

    function onPrintEnd() {
        alert("inward print End");
    }

    function printToPrinter() {
        //alert("Printing Satrted");
        <%=ReportViewer11.ClientID %>.PrintReport();
        //alert("Printing End");
    }

    function FullScreen() {
        //the reference is to the DOM element, not to a Telerik control
        var elem = document.getElementById("<%=ReportViewer11.ClientID%>");
        //see more on the API, handling events and knowing when the full screen is  not allowed in the FullScreen API article https://developer.mozilla.org/en-US/docs/Web/API/Fullscreen_API
        //note: the support for full screen depends on the browsers and is not supported by Telerik
        //the Telerik API used in this example is only used for browser detection that is necessary
        //at the time of writing to call the correponding non-standards method, and this may change in the future
        //more on the Telerik browser detection API is available in the following article:
        //https://docs.telerik.com/devtools/aspnet-ajax/mobile-support/helper-tools/browser-detection-api
        var fName = null;
        if (Telerik.Web.Browser.chrome || Telerik.Web.Browser.edge || Telerik.Web.Browser.opera || Telerik.Web.Browser.safari) {
            fName = "webkitRequestFullscreen";
        }
        if (Telerik.Web.Browser.ff) {
            fName = "mozRequestFullScreen";
        }
        if (Telerik.Web.Browser.ie && Telerik.Web.Browser.documentMode >= 11) {
            fName = "msRequestFullscreen";
        }
        if (!fName) {
            console.log("your browser does not support full screen mode");
        } else {
            if (elem[fName] && typeof elem[fName] === "function") {
                console.log("going into full screen mode");
                elem[fName]();
            }
        }
    }
</script>

<script runat="server"> 
    public override void VerifyRenderingInServerForm(Control control) 
    { 
    } 
</script> 
