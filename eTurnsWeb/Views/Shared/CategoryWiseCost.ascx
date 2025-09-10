<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<eTurns.DTO.ParentModuleMasterDTO>>"  %>
<%@ Import Namespace="System.Web.UI.DataVisualization.Charting" %>
<%@ Import Namespace="System.Drawing" %>

<%

    double[] yValues = { 65.62, 75.54, 60.45, 34.73, 85.42 };
    string[] xValues = { "France", "Canada", "Germany", "USA", "Italy" };
    chrType.Series["Default"].Points.DataBindXY(xValues, yValues);

    // Set Doughnut chart type
    chrType.Series["Default"].ChartType = SeriesChartType.Doughnut;

    // Set labels style
    chrType.Series["Default"]["PieLabelStyle"] = "Outside";

    // Set Doughnut radius percentage
    chrType.Series["Default"]["DoughnutRadius"] = "30";

    // Explode data point with label "Italy"
    chrType.Series["Default"].Points[4]["Exploded"] = "true";

    // Enable 3D
    chrType.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;

    // Disable the Legend
    chrType.Legends[0].Enabled = false;
       %>
<asp:Chart ID="chrType" runat="server" Height="250px" Width="300px"></asp:Chart>