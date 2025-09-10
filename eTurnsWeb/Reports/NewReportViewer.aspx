<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewReportViewer.aspx.cs"
    Inherits="eTurnsWeb.Reports.NewReportViewer" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../Scripts/jquery-1.7.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.20.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <%--<br />
    <input type="button" id="printreport" value="Print" />
    <br />--%>
        <%--<asp:Button Text="Print" runat="server" ID="btnPrintReport" OnClientClick="DoPrint();" />--%>
        <%--<input type="button" value="Print" onclick="return DoPrint();" />--%>
        <b>Export to : </b>
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToExcelxls %>" onclick="return DoExport('xls');" />
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToExcelxlsx %>" onclick="return DoExport('xlsx');" />
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToWordDoc %>" onclick="return DoExport('doc');" />
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToWordDocx %>" onclick="return DoExport('docx');" />
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToPDF %>" onclick="return DoExport('pdf');" />
        <input type="button" value="<%= eTurns.DTO.ResReportMaster.ExportToIMAGE %>" onclick="return DoExport('tif');" />
        <%--<div id="loadPrinters">
            Click to load and select one of the installed printers!
                                    <br />
            <a onclick="javascript:jsWebClientPrint.getPrinters();" class="btn btn-success">Load installed printers...</a>

            <br />
            <br />
        </div>
        <div id="installedPrinters" style="visibility: hidden">
            <label for="installedPrinterName">Select an installed Printer:</label>
            <select name="installedPrinterName" id="installedPrinterName"></select>
        </div>--%>
        <div style="width: 99%; float: left;">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Local" Width="100%" ShowPrintButton="true" Height="650px">
            </rsweb:ReportViewer>
            <asp:HiddenField ID="hdnURL" runat="server" />
            <asp:HiddenField ID="rptID" runat="server" />
            <%=System.Web.Helpers.AntiForgery.GetHtml() %>
        </div>
    </form>
</body>

<script type="text/javascript">
    // Print function (require the reportviewer client ID)
    function printReport(report_ID) {
        var rv1 = $('#' + report_ID);
        var iDoc = rv1.parents('html');
        // Reading the report styles
        var styles = iDoc.find("head style[id$='ReportControl_styles']").html();
        if ((styles == undefined) || (styles == '')) {
            iDoc.find('head script').each(function () {
                var cnt = $(this).html();
                var p1 = cnt.indexOf('ReportStyles":"');
                if (p1 > 0) {
                    p1 += 15;
                    var p2 = cnt.indexOf('"', p1);
                    styles = cnt.substr(p1, p2 - p1);
                }
            });
        }
        if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }
        styles = '<style type="text/css">' + styles + "</style>";

        // Reading the report html
        var table = rv1.find("div[id$='_oReportDiv']");
        if (table == undefined) {
            alert("Report source not found.");
            return;
        }

        // Generating a copy of the report in a new window
        var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';
        var docCnt = styles + table.parent().html();
        var docHead = '<head><title>Printing ...</title><style>body{margin:5;padding:0;}</style></head>';
        var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=720, height=600, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;
        var newWin = window.open("", "_blank", winAttr);
        writeDoc = newWin.document;
        writeDoc.open();
        writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');
        writeDoc.close();

        // The print event will fire as soon as the window loads
        newWin.focus();
        // uncomment to autoclose the preview window when printing is confirmed or canceled.
        // newWin.close();
    };

    $('#printreport').click(function () {

        printReport('ReportViewer1');
    });

    function DoPrint() {
        $.ajax({
            "url": $("input[type='hidden'][id*='hdnURL']").val(),
            //"data": { strCartItems: strjson, IsOnlyFromUI: true },
            "type": "POST",
            "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
            //"async": false,
            //"cache": false,
            "dataType": "json",
            "success": function (response) {
                var h = $(window).height() - 25;
                var w = $(window).width() - 25;
                var PDFFilePath = window.location.protocol + "//" + window.location.host + "/Content/OpenAccess/ReportsPDF/" + response;
                var URLToOpen = window.location.protocol + "//" + window.location.host + "/ReportBuilder/PrintReportPreview?pdfname=" + response;
                var strWindowFeatures = "menubar=yes,top=0,left=0,location=yes,resizable=yes,scrollbars=yes,status=yes,width=" + w + ",height=" + h + "";
                var windowObjectReference = window.open(URLToOpen, "ReportPrint", strWindowFeatures);
            },
            "error": function (response) {
            }
        });
        return false;
    }
    function DoExport(FileType) {
        $.ajax({
            "url": "/ReportBuilder/GenerateReportOfCurrentReport",
            "data": { id: $("input[type='hidden'][id*='rptID']").val(), FileType: FileType },
            "type": "POST",
            "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },

            "dataType": "json",
            "success": function (response) {
                if (response.result) {
                    window.location.href = response.url;
                }
                else {
                    var h = $(window).height() - 25;
                    var w = $(window).width() - 25;
                    var PDFFilePath = window.location.protocol + "//" + window.location.host + "/Content/OpenAccess/ReportsPDF/" + response;
                    var URLToOpen = window.location.protocol + "//" + window.location.host + "/ReportBuilder/PrintReportPreview?pdfname=" + response;
                    var strWindowFeatures = "menubar=yes,top=0,left=0,location=yes,resizable=yes,scrollbars=yes,status=yes,width=" + w + ",height=" + h + "";

                    var event = document.createEvent("MouseEvent");
                    event.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
                    var link = document.createElement('a');
                    link.href = PDFFilePath;
                    link.download = response;
                    link.dispatchEvent(event);
                }
                //if (FileType == "pdf") {
                //    var windowObjectReference = window.open(URLToOpen,"ReportPrint", strWindowFeatures);
                //}
                //else {
                //    var windowObjectReference = window.open(URLToOpen, "ReportPrint", strWindowFeatures);
                //    //window.setTimeout(function () {
                //    //    windowObjectReference.close();
                //    //}, 1000);
                //}
            },
            "error": function (response) {
            }
        });
        return false;
    }
</script>
</html>
