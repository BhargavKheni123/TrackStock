<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="EturnsWeb.ReportViewer"
    MaintainScrollPositionOnPostback="true" Culture="auto" UICulture="auto" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div style="width: 100%; float: left;">
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </div>
        <div>
            <asp:DropDownList ID="drpreport" Visible="false" runat="server" OnSelectedIndexChanged="drpreport_SelectedIndexChanged"
                AutoPostBack="true">
            </asp:DropDownList>

        </div>
        <div>
            <rsweb:ReportViewer ID="SqlReportViewer" runat="server" Width="99%" Style="border: 1px solid;"
                Height="800px" Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote">
                <ServerReport ReportServerUrl="" />
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
--%>
