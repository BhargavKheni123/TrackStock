using eTurns.DTO.Resources;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Summary description for ReportBuilderEngine
/// </summary>
public static class ReportEngine
{
    public static string ResourceFileName { get; set; }

    #region Initialize
    public static Stream GenerateReport(MainReportBuilder reportBuilder)
    {
        Stream ret = new MemoryStream(Encoding.UTF8.GetBytes(GetReportData(reportBuilder)));
        return ret;
    }
    static MainReportBuilder InitAutoGenerateReport(MainReportBuilder reportBuilder)
    {
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            DataSet ds = reportBuilder.DataSource;

            int _TablesCount = ds.Tables.Count;
            ReportTable[] reportTables = new ReportTable[_TablesCount];

            if (reportBuilder.AutoGenerateReport)
            {
                for (int j = 0; j < _TablesCount; j++)
                {
                    DataTable dt = ds.Tables[j];
                    ReportColumns[] columns = new ReportColumns[dt.Columns.Count];
                    ReportDimensions ColumnPadding = new ReportDimensions();
                    ColumnPadding.Default = 2.0;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ReportScale ColumnScale = new ReportScale();
                        ColumnScale.Width = Convert.ToDouble(10.20 / (dt.Columns.Count - 1));// 0.75;

                        if (ColumnScale.Width <= 0.30)
                            ColumnScale.Width = 0.30;

                        ColumnScale.Height = 1;

                        if ((ColumnScale.Width <= 0.80) && (dt.Columns[i].ColumnName.EndsWith("Date") || dt.Columns[i].ColumnName.ToLower().EndsWith("edon")
                                    || dt.Columns[i].ColumnName.ToLower().Equals("created") || dt.Columns[i].ColumnName.ToLower().Equals("updated")))
                        {
                            ColumnScale.Width = 0.80;
                        }
                        else if ((ColumnScale.Width <= 0.40) && (dt.Columns[i].ColumnName.Contains("ID")))
                        {
                            ColumnScale.Width = 0.40;
                        }
                        else if ((ColumnScale.Width <= 0.90) && (dt.Columns[i].ColumnName.Contains("Description") || dt.Columns[i].ColumnName.Contains("Comment")))
                        {
                            ColumnScale.Width = 0.90;
                        }


                        columns[i] = new ReportColumns() { ColumnCell = new ReportTextBoxControl() { Name = dt.Columns[i].ColumnName, Size = ColumnScale, Padding = ColumnPadding }, HeaderText = dt.Columns[i].ColumnName, HeaderColumnPadding = ColumnPadding };
                    }

                    reportTables[j] = new ReportTable() { ReportName = dt.TableName, ReportDataColumns = columns };
                }

            }
            reportBuilder.Body = new ReportBody();
            reportBuilder.Body.ReportControlItems = new ReportItems();
            reportBuilder.Body.ReportControlItems.ReportTable = reportTables;
        }
        return reportBuilder;
    }
    static string GetReportData(MainReportBuilder reportBuilder)
    {
        reportBuilder = InitAutoGenerateReport(reportBuilder);
        string rdlcXML = "";
        rdlcXML += @"<?xml version=""1.0"" encoding=""utf-8""?> 
                        <Report xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition""  
                        xmlns:rd=""http://schemas.microsoft.com/SQLServer/reporting/reportdesigner""> 
                      <Body>";

        string _tableData = GenerateTable(reportBuilder);

        if (_tableData.Trim() != "")
        {
            rdlcXML += @"<ReportItems>" + _tableData + @"</ReportItems>";
        }
        //byte[] imgBinary = File.ReadAllBytes(HttpContext.Current.Server.MapPath(@"//Content/images") + "\\logo.png");
        rdlcXML += @"<Height>1.05in</Height> 
                        <Style /> 
                      </Body> 
                      <Width>10.20in</Width> 
                     <ReportParameters>
                        <ReportParameter Name=""eTurnsLogoURL"">
                          <DataType>String</DataType>
                          <Prompt>eTurnsLogoURL</Prompt>
                        </ReportParameter>
                        <ReportParameter Name=""EnterpriseLogoURL"">
                          <DataType>String</DataType>
                          <Nullable>true</Nullable>
                          <Prompt>EnterpriseLogoURL</Prompt>
                         </ReportParameter>
                            <ReportParameter Name=""CompanyLogoURL"">
                            <DataType>String</DataType>
                            <Nullable>true</Nullable>
                            <Prompt>CompanyLogoURL</Prompt>
                         </ReportParameter>
                        <ReportParameter Name=""ReportTitle"">
                            <DataType>String</DataType>
                            <Nullable>true</Nullable>
                            <Prompt>ReportTitle</Prompt>
                         </ReportParameter>                          
                      </ReportParameters>
                      <Page> 
                        " + GetPageHeader(reportBuilder) + GetFooter(reportBuilder) + GetReportPageSettings() + @" 
                        <Style /> 
                      </Page> 
                      <AutoRefresh>0</AutoRefresh> 
                        " + GetDataSet(reportBuilder) + @" 
                      <Language>en-US</Language> 
                      <ConsumeContainerWhitespace>true</ConsumeContainerWhitespace> 
                      <rd:ReportUnitType>Inch</rd:ReportUnitType> 
                      <rd:ReportID>17efa4a3-5c39-4892-a44b-fbde95c96585</rd:ReportID> 
                    </Report>";
        return rdlcXML;
    }
    #endregion

    #region Page Settings
    static string GetReportPageSettings()
    {
        return @"<PageHeight>8.5in</PageHeight> 
    <PageWidth>11.00in</PageWidth> 
    <LeftMargin>0.25in</LeftMargin> 
    <RightMargin>0.25in</RightMargin> 
    <TopMargin>0.25in</TopMargin> 
    <BottomMargin>0.25in</BottomMargin>";
        //<ColumnSpacing>1pt</ColumnSpacing>";
    }
    private static string GetPageHeader(MainReportBuilder reportBuilder)
    {
        string strHeader = "";
        if (reportBuilder.Page == null || reportBuilder.Page.ReportHeader == null) return "";
        ReportSections reportHeader = reportBuilder.Page.ReportHeader;
        strHeader = @"<PageHeader> 
                          <Height>" + reportHeader.Size.Height.ToString() + @"in</Height> 
                          <PrintOnFirstPage>" + reportHeader.PrintOnFirstPage.ToString().ToLower() + @"</PrintOnFirstPage> 
                          <PrintOnLastPage>" + reportHeader.PrintOnLastPage.ToString().ToLower() + @"</PrintOnLastPage> 
                          <ReportItems>";
        ReportTextBoxControl[] headerTxt = reportBuilder.Page.ReportHeader.ReportControlItems.TextBoxControls;
        //if (headerTxt != null)
        //for (int i = 0; i < headerTxt.Count(); i++)
        //{
        //    strHeader += GetHeaderTextBox(headerTxt[i].Name, null, headerTxt[i].ValueOrExpression);
        //}
        strHeader += @" 
                            <Image Name=""eTurnsLogo"">
                            <Source>External</Source>
                            <Value>=Parameters!eTurnsLogoURL.Value</Value>
                            <Sizing>Fit</Sizing>
                            <Top>0.05in</Top>
                            <Left>0.09in</Left>
                            <Height>0.375in</Height>
                            <Width>1.16667in</Width>
                            <Style>
                            <Border>
                               <Style>None</Style>
                            </Border>
                            </Style>
                            </Image>
                            <Image Name=""eTurnsLogo2"">
                                  <Source>External</Source>
                                  <Value>=Parameters!CompanyLogoURL.Value</Value>
                                  <Sizing>Fit</Sizing>
                                  <Top>0.05in</Top>
                                  <Left>8.89082in</Left>
                                  <Height>0.375in</Height>
                                  <Width>1.23958in</Width>
                                  <ZIndex>1</ZIndex>
                                  <Style>
                                    <Border>
                                      <Style>None</Style>
                                    </Border>
                                  </Style>
                                </Image>
                               <Textbox Name=""ReportTitle"">
                              <CanGrow>true</CanGrow>
                              <KeepTogether>true</KeepTogether>
                              <Paragraphs>
                                <Paragraph>
                                  <TextRuns>
                                    <TextRun>
                                      <Value>=Parameters!ReportTitle.Value</Value>
                                      <Style>
                                        <FontFamily>Calibri</FontFamily>
                                        <FontSize>14pt</FontSize>
                                        <FontWeight>Bold</FontWeight>
                                      </Style>
                                    </TextRun>
                                  </TextRuns>
                                  <Style>
                                    <TextAlign>Center</TextAlign>
                                  </Style>
                                </Paragraph>
                              </Paragraphs>
                              <Top>0.030in</Top>
                              <Left>4.20in</Left>
                              <Height>0.30in</Height>
                              <Width>2.50in</Width>
                              <ZIndex>2</ZIndex>
                              <Style>
                                <Border>
                                  <Style>None</Style>
                                </Border>
                                <VerticalAlign>Middle</VerticalAlign>
                                <PaddingLeft>2pt</PaddingLeft>
                                <PaddingRight>2pt</PaddingRight>
                                <PaddingTop>2pt</PaddingTop>
                                <PaddingBottom>2pt</PaddingBottom>
                              </Style>
                            </Textbox>
                    <Textbox Name=""ExecutionTime"">
                              <CanGrow>true</CanGrow>
                              <KeepTogether>true</KeepTogether>
                              <Paragraphs>
                                <Paragraph>
                                  <TextRuns>
                                    <TextRun>
                                      <Value>=First(Fields!CurrentDateTime.Value, ""Report1"")</Value>
                                      <Style>
                                        <FontFamily>Calibri</FontFamily>
                                      </Style>
                                    </TextRun>
                                  </TextRuns>
                                  <Style>
                                    <TextAlign>Left</TextAlign>
                                  </Style>
                                </Paragraph>
                              </Paragraphs>
                              <rd:DefaultName>ExecutionTime</rd:DefaultName>
                              <Top>0.50in</Top>
                              <Left>0.05in</Left>
                              <Height>0.20in</Height>
                              <Width>1.90in</Width>
                              <ZIndex>4</ZIndex>
                              <Style>
                                <Border>
                                  <Style>None</Style>
                                </Border>
                                <PaddingLeft>2pt</PaddingLeft>
                                <PaddingRight>2pt</PaddingRight>
                                <PaddingTop>2pt</PaddingTop>
                                <PaddingBottom>2pt</PaddingBottom>
                              </Style>
                            </Textbox>
                            <Textbox Name=""TotalPages"">
                              <CanGrow>true</CanGrow>
                              <KeepTogether>true</KeepTogether>
                              <Paragraphs>
                                <Paragraph>
                                  <TextRuns>
                                    <TextRun>
                                      <Value>=Globals!PageNumber &amp; "" of "" &amp; Globals!TotalPages</Value>
                                      <Style>
                                        <FontFamily>Calibri</FontFamily>
                                      </Style>
                                    </TextRun>
                                  </TextRuns>
                                  <Style>
                                    <TextAlign>Right</TextAlign>
                                  </Style>
                                </Paragraph>
                              </Paragraphs>
                              <rd:DefaultName>TotalPages</rd:DefaultName>
                              <Top>0.5in</Top>
                              <Left>8.50in</Left>
                              <Height>0.20in</Height>
                              <Width>1.50in</Width>
                              <ZIndex>5</ZIndex>
                              <Style>
                                <Border>
                                  <Style>None</Style>
                                </Border>
                                <PaddingLeft>2pt</PaddingLeft>
                                <PaddingRight>2pt</PaddingRight>
                                <PaddingTop>2pt</PaddingTop>
                                <PaddingBottom>2pt</PaddingBottom>
                              </Style>
                            </Textbox>
                            <Textbox Name=""txtRoomInfo"">
                                      <CanGrow>true</CanGrow>
                                      <KeepTogether>true</KeepTogether>
                                      <Paragraphs>
                                        <Paragraph>
                                          <TextRuns>
                                            <TextRun>
				                              <Value>=First(Fields!RoomInfo.Value, ""Report1"")</Value>
                                              <Style>
                                                <FontFamily>Calibri</FontFamily>
                                                <Color>White</Color>
                                              </Style>
                                            </TextRun>
                                          </TextRuns>
                                          <Style />
                                        </Paragraph>
                                      </Paragraphs>
                                      <rd:DefaultName>txtRoomInfo</rd:DefaultName>
                                      <Top>0.75in</Top>
                                      <Left>0.05in</Left>
                                      <Height>0.75in</Height>
                                      <Width>10.05in</Width>
                                      <ZIndex>3</ZIndex>
                                      <Style>
                                        <Border>
                                          <Style>None</Style>
                                        </Border>
                                        <BackgroundColor>CornflowerBlue</BackgroundColor>
                                        <PaddingLeft>2pt</PaddingLeft>
                                        <PaddingRight>2pt</PaddingRight>
                                        <PaddingTop>2pt</PaddingTop>
                                        <PaddingBottom>2pt</PaddingBottom>
                                      </Style>
                                    </Textbox>
                          </ReportItems> 

                          <Style /> 
                        </PageHeader>";




        return strHeader;
    }
    private static string GetFooter(MainReportBuilder reportBuilder)
    {
        string strFooter = "";
        if (reportBuilder.Page == null || reportBuilder.Page.ReportFooter == null) return "";
        strFooter = @"<PageFooter> 
                          <Height>0.68425in</Height> 
                          <PrintOnFirstPage>true</PrintOnFirstPage> 
                          <PrintOnLastPage>true</PrintOnLastPage> 
                          <ReportItems>";
        ReportTextBoxControl[] footerTxt = reportBuilder.Page.ReportFooter.ReportControlItems.TextBoxControls;
        if (footerTxt != null)
            for (int i = 0; i < footerTxt.Count(); i++)
            {
                if (footerTxt[i] != null)
                {
                    strFooter += GetFooterTextBox(footerTxt[i].Name, null, footerTxt[i].ValueOrExpression);
                }
            }
        strFooter += @"</ReportItems> 
                          <Style /> 
                        </PageFooter>";
        return strFooter;
    }
    #endregion

    #region Dataset
    static string GetDataSet(MainReportBuilder reportBuilder)
    {
        string dataSetStr = "";
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            string dsName = "rptCustomers";
            dataSetStr += @"<DataSources> 
    <DataSource Name=""" + dsName + @"""> 
      <ConnectionProperties> 
        <DataProvider>System.Data.DataSet</DataProvider> 
        <ConnectString>/* Local Connection */</ConnectString> 
      </ConnectionProperties> 
      <rd:DataSourceID>944b21fd-a128-4363-a5fc-312a032950a0</rd:DataSourceID> 
    </DataSource> 
  </DataSources> 
  <DataSets>"
                         + GetDataSetTables(reportBuilder.Body.ReportControlItems.ReportTable, dsName) +
              @"</DataSets>";
        }
        return dataSetStr;
    }
    private static string GetDataSetTables(ReportTable[] tables, string DataSourceName)
    {
        string strTables = "";
        for (int i = 0; i < tables.Length; i++)
        {
            strTables += @"<DataSet Name=""" + tables[i].ReportName + @"""> 
      <Query> 
        <DataSourceName>" + DataSourceName + @"</DataSourceName> 
        <CommandText>/* Local Query */</CommandText> 
      </Query> 
     " + GetDataSetFields(tables[i].ReportDataColumns) + @" 
    </DataSet>";
        }
        return strTables;
    }
    private static string GetDataSetFields(ReportColumns[] reportColumns)
    {
        string strFields = "";

        strFields += @"<Fields>";
        for (int i = 0; i < reportColumns.Length; i++)
        {
            strFields += @"<Field Name=""" + reportColumns[i].ColumnCell.Name + @"""> 
          <DataField>" + reportColumns[i].ColumnCell.Name + @"</DataField> 
          <rd:TypeName>System.String</rd:TypeName> 
        </Field>";
        }
        strFields += @"</Fields>";
        return strFields;
    }
    #endregion

    #region Report Table Configuration
    static string GenerateTable(MainReportBuilder reportBuilder)
    {
        string TableStr = "";
        if (reportBuilder != null && reportBuilder.DataSource != null && reportBuilder.DataSource.Tables.Count > 0)
        {
            ReportTable table = new ReportTable();
            for (int i = 0; i < reportBuilder.Body.ReportControlItems.ReportTable.Length; i++)
            {
                table = reportBuilder.Body.ReportControlItems.ReportTable[i];
                TableStr += @"<Tablix Name=""table_" + table.ReportName + @"""> 
        <TablixBody> 
          " + GetTableColumns(reportBuilder, table) + @" 
          <TablixRows> 
            " + GenerateTableHeaderRow(reportBuilder, table) + GenerateTableRow(reportBuilder, table) + @" 
          </TablixRows> 
        </TablixBody>" + GetTableColumnHeirarchy(reportBuilder, table) + @" 
        <TablixRowHierarchy> 
          <TablixMembers> 
            <TablixMember> 
              <KeepWithGroup>After</KeepWithGroup> 
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember> 
            <TablixMember> 
              <Group Name=""" + table.ReportName + "_Details" + @""" /> 
            </TablixMember> 
          </TablixMembers> 
        </TablixRowHierarchy> 
        <RepeatColumnHeaders>true</RepeatColumnHeaders> 
        <RepeatRowHeaders>true</RepeatRowHeaders> 
        <DataSetName>" + table.ReportName + @"</DataSetName>" + GetSortingDetails(reportBuilder) + @" 
        <Top>0.05in</Top> 
        <Left>0.05in</Left> 
        <Height>0.8in</Height> 
        <Width>10.0in</Width> 
        <ZIndex>1</ZIndex>
        <Style> 
          <Border> 
            <Style>None</Style> 
          </Border> 
        </Style> 
      </Tablix>";
            }
        }
        return TableStr;
    }
    static string GetSortingDetails(MainReportBuilder reportBuilder)
    {
        return "";
        //ReportColumns[] columns = reportBuilder.Body.ReportControlItems.ReportTable[0].ReportDataColumns;
        //ReportTextBoxControl sortColumn = new ReportTextBoxControl();
        //if (columns == null) return "";

        //string strSorting = "";
        //strSorting = @" <SortExpressions>";
        //for (int i = 0; i < columns.Length; i++)
        //{
        //    sortColumn = columns[i].ColumnCell;
        //    strSorting += "<SortExpression><Value>=Fields!" + sortColumn.Name + @".Value</Value>";
        //    if (columns[i].SortDirection == ReportSort.Descending)
        //        strSorting += "<Direction>Descending</Direction>";
        //    strSorting += @"</SortExpression>";
        //}
        //strSorting += "</SortExpressions>";
        //return strSorting;
    }
    static string GenerateTableRow(MainReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportScale colHeight = ColumnCell.Size;
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow = @"<TablixRow> 
                <Height>0.6cm</Height> 
                <TablixCells>";
        for (int i = 0; i < columns.Length; i++)
        {
            ColumnCell = columns[i].ColumnCell;
            if (ColumnCell.Name.Contains("RoomInfo") || ColumnCell.Name.Contains("CurrentDateTime"))
                continue;
            padding = ColumnCell.Padding;
            strTableRow += @"<TablixCell> 
                  <CellContents> 
                   " + GenerateTextBox("txtCell_" + table.ReportName + "_", ColumnCell.Name, "", true, padding) + @" 
                  </CellContents> 
                </TablixCell>";
        }
        strTableRow += @"</TablixCells></TablixRow>";
        return strTableRow;
    }
    static string GenerateTableHeaderRow(MainReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();
        ReportDimensions padding = new ReportDimensions();
        if (columns == null) return "";

        string strTableRow = "";
        strTableRow = @"<TablixRow>
                <Height>0.6cm</Height> 
                <TablixCells>";
        for (int i = 0; i < columns.Length; i++)
        {
            ColumnCell = columns[i].ColumnCell;
            if (ColumnCell.Name.Contains("RoomInfo") || ColumnCell.Name.Contains("CurrentDateTime"))
                continue;
            padding = columns[i].HeaderColumnPadding;
            strTableRow += @"<TablixCell> 
                  <CellContents> 
                  
                   " + GenerateHeaderTableTextBox("txtHeader_" + table.ReportName + "_", ColumnCell.Name, columns[i].HeaderText == null || columns[i].HeaderText.Trim() == "" ? ColumnCell.Name : columns[i].HeaderText, false, padding) + @" 

                  </CellContents> 
                </TablixCell>";
        }
        strTableRow += @"</TablixCells></TablixRow>";
        return strTableRow;
    }

    static string GetTableColumns(MainReportBuilder reportBuilder, ReportTable table)
    {

        ReportColumns[] columns = table.ReportDataColumns;
        ReportTextBoxControl ColumnCell = new ReportTextBoxControl();

        if (columns == null) return "";

        string strColumnHeirarchy = "";
        strColumnHeirarchy = @" 
            <TablixColumns>";
        for (int i = 0; i < columns.Length; i++)
        {
            ColumnCell = columns[i].ColumnCell;
            if (ColumnCell.Name.Contains("RoomInfo") || ColumnCell.Name.Contains("CurrentDateTime"))
                continue;

            strColumnHeirarchy += @" <TablixColumn> 
                                          <Width>" + ColumnCell.Size.Width.ToString() + @"in</Width>  
                                        </TablixColumn>";
        }
        strColumnHeirarchy += @"</TablixColumns>";
        return strColumnHeirarchy;
    }
    static string GetTableColumnHeirarchy(MainReportBuilder reportBuilder, ReportTable table)
    {
        ReportColumns[] columns = table.ReportDataColumns;
        if (columns == null) return "";

        string strColumnHeirarchy = "";
        strColumnHeirarchy = @" 
            <TablixColumnHierarchy> 
          <TablixMembers>";
        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i].ColumnCell.Name.Contains("RoomInfo") || columns[i].ColumnCell.Name.Contains("CurrentDateTime"))
                continue;

            strColumnHeirarchy += "<TablixMember />";
        }
        strColumnHeirarchy += @"</TablixMembers> 
        </TablixColumnHierarchy>";
        return strColumnHeirarchy;
    }
    #endregion

    #region Report TextBox

    public static string GetResourceValue(string FileName, string Key)
    {
        string KeyVal = string.Empty;
        if (Key.ToLower().Contains("udf"))
        {
            string PreFixValue = Key.Replace("UDF10", string.Empty).Replace("UDF1", string.Empty).Replace("UDF2", string.Empty).Replace("UDF3", string.Empty).Replace("UDF4", string.Empty).Replace("UDF5", string.Empty).Replace(".Value", string.Empty);
            KeyVal = ResourceHelper.GetResourceValue(Key, FileName, true);
            if (!KeyVal.Contains(PreFixValue))
                KeyVal = PreFixValue + " " + KeyVal;
        }
        else
        {
            KeyVal = ResourceHelper.GetResourceValue(Key, FileName);
        }

        return KeyVal;
    }

    static string GenerateTextBox(string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null)
    {
        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + strControlIDPrefix + strName + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
        if (isFieldValue)
        {
            strTextBox += @"<Value>=Fields!" + strName + @".Value</Value>";
            // strTextBox += @"<Value>=IIF(Previous(Fields!" + strName + @".Value) = Fields!" + strName + @".Value, Default, Bold)";
        }
        else
        {
            string resvalue = GetResourceValue(ResourceFileName, strValueOrExpression);
            if (resvalue == strValueOrExpression)
            {
                resvalue = GetResourceValue("ResCommon", strValueOrExpression);
            }
            strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
        }
        strTextBox += @"<Style>
                                <FontFamily>Calibri</FontFamily>
                              </Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style />
                              </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + @"</rd:DefaultName> 
                      <Style> 
                        <FontFamily>Calibri</FontFamily>
                        <BackgroundColor>=IIF(RowNumber(Nothing)=1,""Transparent"", IIf(Fields!" + strName + @".Value = Previous(Fields!" + strName + @".Value), ""Transparent"", ""Yellow""))</BackgroundColor>
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
        return strTextBox;
    }
    static string GenerateHeaderTableTextBox(string strControlIDPrefix, string strName, string strValueOrExpression = "", bool isFieldValue = true, ReportDimensions padding = null)
    {
        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + strControlIDPrefix + strName + @"""> 
                      <CanGrow>true</CanGrow> 
                      <KeepTogether>true</KeepTogether> 
                      <Paragraphs> 
                        <Paragraph> 
                          <TextRuns> 
                            <TextRun>";
        if (isFieldValue)
        {
            strTextBox += @"<Value>=Fields!" + strName + @".Value</Value>";
        }
        else
        {
            string resvalue = GetResourceValue(ResourceFileName, strValueOrExpression);
            if (resvalue == strValueOrExpression)
            {
                resvalue = GetResourceValue("ResCommon", strValueOrExpression);
            }
            strTextBox += @"<Value>" + resvalue + "</Value>";
        }

        //else strTextBox += @"<Value>" + strValueOrExpression + "</Value>";
        strTextBox += @"<Style>
                                <FontFamily>Calibri</FontFamily>
                              </Style> 
                            </TextRun> 
                          </TextRuns> 
                          <Style /> 
                        </Paragraph> 
                      </Paragraphs> 
                      <rd:DefaultName>" + strControlIDPrefix + strName + @"</rd:DefaultName> 
                      <Style> 
   <BackgroundColor>#DDDDDD</BackgroundColor>
<FontWeight>Bold</FontWeight>
   <FontFamily>Calibri</FontFamily>
                        <Border> 
                          <Color>LightGrey</Color> 
                          <Style>Solid</Style> 
                        </Border>" + GetDimensions(padding) + @"</Style> 
                    </Textbox>";
        return strTextBox;
    }

    static string GetHeaderTextBox(string textBoxName, ReportDimensions padding = null, params string[] strValues)
    {
        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + textBoxName + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";

        for (int i = 0; i < strValues.Length; i++)
        {
            strTextBox += GetHeaderTextRun(strValues[i].ToString());
        }

        strTextBox += @"</TextRuns> 
              <Style /> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + textBoxName + @"</rd:DefaultName> 
          <Top>0.5cm</Top> 
          <Left>5cm</Left> 
          <Height>0.6cm</Height> 
          <Width>7.93812cm</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

        strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
        return strTextBox;
    }
    static string GetFooterTextBox(string textBoxName, ReportDimensions padding = null, params string[] strValues)
    {

        string strTextBox = "";
        strTextBox = @" <Textbox Name=""" + textBoxName + @"""> 
          <CanGrow>true</CanGrow> 
          <KeepTogether>true</KeepTogether> 
          <Paragraphs> 
            <Paragraph> 
              <TextRuns>";

        for (int i = 0; i < strValues.Length; i++)
        {
            strTextBox += GetTextRun_fot(strValues[i].ToString());
        }

        strTextBox += @"</TextRuns> 
              <Style /> 
            </Paragraph> 
          </Paragraphs> 
          <rd:DefaultName>" + textBoxName + @"</rd:DefaultName> 
          <Top>1.0884cm</Top> 
          <Left>1cm</Left> 
          <Height>0.6cm</Height> 
          <Width>20.9cm</Width> 
          <ZIndex>2</ZIndex> 
          <Style> 
            <Border> 
              <Style>None</Style> 
            </Border>";

        strTextBox += GetDimensions(padding) + @"</Style> 
        </Textbox>";
        return strTextBox;
    }

    static string GetTextRun_fot(string ValueOrExpression)
    {
        //<Value>=""Page "" &amp; Globals!PageNumber &amp; "" of "" &amp; Globals!TotalPages</Value> 
        return "<TextRun>"
                  + "<Value>=&quot;" + ValueOrExpression + "</Value>"
                  + "<Style>"
                    + "<FontSize>8pt</FontSize>"
                  + "</Style>"
                + "</TextRun>";
    }


    static string GetTextRun(string ValueOrExpression)
    {
        return @"<TextRun> 
                  <Value>" + ValueOrExpression + @"</Value> 
                  <Style> 
                    <FontSize>8pt</FontSize> 
                  </Style> 
                </TextRun>";
    }

    static string GetHeaderTextRun(string ValueOrExpression)
    {
        return @"<TextRun> 
                  <Value>" + ValueOrExpression + @"</Value> 
                  <Style> 
                    <FontSize>10pt</FontSize> 
                    <FontWeight>Bold</FontWeight>
                  </Style> 
                </TextRun>";
    }
    #endregion

    #region Images
    static void GenerateReportImage(MainReportBuilder reportBuilder)
    {
    }
    #endregion

    #region Settings
    private static string GetDimensions(ReportDimensions padding = null)
    {
        string strDimensions = "";
        if (padding != null)
        {
            if (padding.Default == 0)
            {
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Left);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Right);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Top);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Bottom);
            }
            else
            {
                strDimensions += string.Format("<PaddingLeft>{0}pt</PaddingLeft>", padding.Default);
                strDimensions += string.Format("<PaddingRight>{0}pt</PaddingRight>", padding.Default);
                strDimensions += string.Format("<PaddingTop>{0}pt</PaddingTop>", padding.Default);
                strDimensions += string.Format("<PaddingBottom>{0}pt</PaddingBottom>", padding.Default);
            }
        }
        return strDimensions;
    }
    #endregion

}

#region Declarations
public static class ReportGlobalParameters
{
    //string data= Globals!PageNumber;
    public static string CurrentPageNumber = " &quot; &amp; Globals!PageNumber &amp; &quot;";
    //public static string TotalPages = "=Globals!OverallTotalPages";
    public static string TotalPages = " &quot; &amp; Globals!TotalPages";
}
public class MainReportBuilder
{
    public ReportPage Page { get; set; }
    public ReportBody Body { get; set; }
    public DataSet DataSource { get; set; }

    private bool autoGenerateReport = true;
    public bool AutoGenerateReport
    {
        get { return autoGenerateReport; }
        set { autoGenerateReport = value; }
    }

}
public class ReportItems
{
    public ReportTextBoxControl[] TextBoxControls { get; set; }
    public ReportTable[] ReportTable { get; set; }
    public ReportImage[] ReportImages { get; set; }
}
public class ReportTable
{
    public string ReportName { get; set; }
    public ReportColumns[] ReportDataColumns { get; set; }
}
public class ReportColumns
{
    public bool isGroupedColumn { get; set; }
    public string HeaderText { get; set; }
    public ReportSort SortDirection { get; set; }
    public ReportFunctions Aggregate { get; set; }
    public ReportTextBoxControl ColumnCell { get; set; }
    public ReportDimensions HeaderColumnPadding { get; set; }
}
public class ReportTextBoxControl
{
    public string Name { get; set; }
    public string[] ValueOrExpression { get; set; }
    public ReportActions Action { get; set; }
    public ReportDimensions Padding { get; set; }
    public int SpaceAfter { get; set; }
    public int SpaceBefore { get; set; }

    private ReportHorizantalAlign textAlign = ReportHorizantalAlign.Default;
    public ReportHorizantalAlign TextAlign
    {
        get { return textAlign; }
        set { textAlign = value; }
    }

    private ReportHorizantalAlign verticalAlign = ReportHorizantalAlign.Default;
    public ReportHorizantalAlign VerticalAlign
    {
        get { return verticalAlign; }
        set { verticalAlign = value; }
    }
    public ReportStyles BorderStyle { get; set; }
    public ReportColor BorderColor { get; set; }
    public ReportScale BorderWidth { get; set; }
    public Color BackgroundColor { get; set; }
    public ReportImage BackgroundImage { get; set; }
    public Font TextFont { get; set; }
    public double LineHeight { get; set; }
    public bool CanGrow { get; set; }
    public bool CanShrink { get; set; }
    public bool ToolTip { get; set; }
    public ReportDimensions Position { get; set; }
    public ReportScale Size { get; set; }
    public bool Visible { get; set; }
}
public class ReportBody
{
    public ReportSections ReportBodySection { get; set; }
    public ReportItems ReportControlItems { get; set; }
}
public class ReportPage
{
    public bool AutoRefresh { get; set; }
    public Color BackgroundColor { get; set; }
    public ReportImage BackgroundImage { get; set; }
    public ReportColor BorderColor { get; set; }
    public ReportScale BorderWidth { get; set; }
    public ReportColumnSettings Columns { get; set; }
    public ReportScale InteractiveSize { get; set; }
    public ReportDimensions Margins { get; set; }
    public ReportScale PageSize { get; set; }
    //nadir margin

    public ReportSections ReportHeader { get; set; }
    public ReportSections ReportFooter { get; set; }
}
public class ReportSections
{
    public ReportStyles BorderStyle { get; set; }
    public ReportColor BorderColor { get; set; }
    public ReportScale BorderWidth { get; set; }
    public Color BackgroundColor { get; set; }
    public ReportImage BackgroundImage { get; set; }
    public ReportScale Size { get; set; }

    private bool printOnFirstPage = true;

    public bool PrintOnFirstPage
    {
        get { return printOnFirstPage; }
        set { printOnFirstPage = value; }
    }

    private bool printOnLastpage = true;

    public bool PrintOnLastPage
    {
        get { return printOnLastpage; }
        set { printOnLastpage = value; }
    }

    public ReportItems ReportControlItems { get; set; }
}
public class ReportColumnSettings
{
    public int Columns { get; set; }
    public int ColumnsSpacing { get; set; }
}
public class ReportActions
{
    public ReportActionType ActionType { get; set; }
    public string ValueOrExpression { get; set; }
}
public class ReportDimensions
{
    public double Left { get; set; }
    public double Right { get; set; }
    public double Top { get; set; }
    public double Bottom { get; set; }
    private double _default = 2;

    public double Default
    {
        get { return _default; }
        set { _default = value; }
    }

}
public class ReportIndent
{
    public double HangingIndent { get; set; }
    public double LeftIndent { get; set; }
    public double RightIndent { get; set; }
}
public class ReportScale
{
    public double Height { get; set; }
    public double Width { get; set; }
}
public class ReportImage
{
    public ReportImageSource ImagePath { get; set; }
    public string ValueOrExpression { get; set; }
    public ReportImageMIMEType MIMEType { get; set; }
    public ReportStyles Border { get; set; }
    public ReportColor Color { get; set; }
    public ReportDimensions Position { get; set; }
    public ReportScale Size { get; set; }
    public ReportDimensions Padding { get; set; }

    private ReportImageScaling reportImageScaling = ReportImageScaling.AutoSize;
    public ReportImageScaling ReportImageScaling
    {
        get { return reportImageScaling; }
        set { reportImageScaling = value; }
    }
}
public class ReportColor
{
    public Color Default { get; set; }
    public Color Left { get; set; }
    public Color Right { get; set; }
    public Color Top { get; set; }
    public Color Bottom { get; set; }
}
public class ReportStyles
{
    public ReportStyle Default { get; set; }
    public ReportStyle Left { get; set; }
    public ReportStyle Right { get; set; }
    public ReportStyle Top { get; set; }
    public ReportStyle Bottom { get; set; }
}
public enum ReportActionType
{
    None,
    HyperLink
}
public enum ReportHorizantalAlign
{
    Left,
    Right,
    Center,
    General,
    Default
}
public enum ReportVerticalAlign
{
    Top,
    Middle,
    Bottom,
    Default
}
public enum ReportImageRepeat
{
    Default,
    Repeat,
    RepeatX,
    RepeatY,
    Clip
}
public enum ReportImageScaling
{

    AutoSize,
    Flip,
    FlipProportional,
    Clip
}
public enum ReportImageSource
{
    External,
    Embedded,
    Database
}
public enum ReportImageMIMEType
{
    Bitmap,
    JPEG,
    GIF,
    PNG,
    xPNG
}
public enum ReportStyle
{
    Default, Dashed, Dotted, Double, Solid, None
}
public enum ReportSort
{
    Ascending,
    Descending
}
public enum ReportFunctions
{
    Avg,
    Count,
    Sum,
    Min,
    Max,
    Aggregate


}
#endregion Declarations