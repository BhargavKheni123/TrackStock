<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Globalization" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <%--<img src="/TransferChart/CreateChart?i=<%= System.DateTime.Now.Ticks.ToString() %>&chartType=<%=System.Web.UI.DataVisualization.Charting.SeriesChartType.Column %>"
        alt="" />--%>
        <img src="/TransferChart/CreateChart?i=<%= eTurns.DAL.DateTimeUtility.DateTimeNow.Ticks.ToString(CultureInfo.InvariantCulture) %>&chartType=Column"
        alt="" />
</body>
</html>
