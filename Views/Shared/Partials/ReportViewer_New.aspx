<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer_New.aspx.cs" Inherits="Corno.Web.Views.Shared.Partials.ReportViewer12" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.ReportViewer.WebForms" Assembly="Telerik.ReportViewer.WebForms, Version=17.0.23.118, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <%--<asp:Button ID="PrintButton" runat="server" Text="Print Report" OnClientClick="MyPrint(); return false;" />--%>
    <telerik:ReportViewer ID="ReportViewer_New" runat="server" ViewMode="PrintPreview"
        ShowPrintButton="False" ShowPrintPreviewButton="True" DocumentMapVisible="False"
        ShowHistoryButtons="True" ShowParametersButton="True" ShowNavigationGroup="True" ShowRefreshButton="True"
                          
                          ShowExportGroup="False" Width="100%" Height="100%">
    </telerik:ReportViewer>

    <%-- <asp:Button ID="PrintButton" runat="server" Text="Print Report" OnClientClick="MyPrint(); return false;" />
    <telerik:ReportViewer ID="ReportViewer_New" runat="server" />
    <button id="printToPrinter" onclick="printClientLabel()">Print</button>--%>
</body>
</html>

<script type="text/javascript">

    /*ReportViewer.OnReportLoadedOld = ReportViewer.OnReportLoaded;
    ReportViewer.prototype.OnReportLoaded = function () {
        this.OnReportLoadedOld();
        var printButton = document.getElementById("PrintButton");
        printButton.disabled = false;
    }*/

    /*function printClientLabel() {
        // Print the report
        */<%=ReportViewer_New.ClientID %>/*.PrintReport();
        *//*$("#ReportViewer_New").data("telerik_ReportViewer").printReport();*//*
    }*/


    /*function MyPrint() {
        */<%=ReportViewer_New.ClientID %>/*.PrintReport();
        *//*var pdfUrl = '/Kitchen/PartLabel/GenerateReportPdf'; // Replace with your controller and action URL
        fetch(pdfUrl)
            .then(response => response.blob())
            .then(blob => {
                var fileURL = URL.createObjectURL(blob);
                var printWindow = window.open(fileURL);
                printWindow.print();
                
            })
            .catch(error => {
                console.error('Error fetching or printing PDF: ', error);
            });*//*
    }*/

    function onPrintBegin() {
        alert("inward print begin");
        //$('#form').submit();
    }

    function onPrintEnd() {
        alert("inward print End");
    }

    function printToPrinter() {
        //alert("Printing Satrted");
        <%=ReportViewer_New.ClientID %>.PrintReport();
        //alert("Printing End");
    }
</script>

<script runat="server"> 
    public override void VerifyRenderingInServerForm(Control control)
    {
    }
</script>
