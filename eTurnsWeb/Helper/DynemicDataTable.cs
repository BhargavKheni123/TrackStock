using System.Text;
using System.Web.Mvc;


public static class DynemicDataTable
{
    public static MvcHtmlString RenderDynemicDataTableFieldArray(this HtmlHelper htmlHelper, string TableName)
    {
        if (TableName == "ItemLocationTable")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"mDataProp\": null, \"sClass\": \"read_only control center\", \"sDefaultContent\": #2#<img src=###/Content/images/DeleteRed.png###>#2# }#3#");
            sb.Append("{ \"mDataProp\": \"ItemNumber\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"BinNumber\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"CustomerOwnedQuantity\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"ConsignedQuantity\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"ItemCriticalValue\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"ItemMinimumValue\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            sb.Append("{ \"mDataProp\": \"Created\", \"sClass\": \"read_only\", \"bSortable\": false,");
            sb.Append("\"fnRender\": function (obj, val) {");
            sb.Append("return GetDateInFullFormat(val);");
            sb.Append("}");
            sb.Append("}#3#");
            sb.Append("{ \"mDataProp\": \"ItemMaximumValue\", \"sClass\": \"read_only\", \"bSortable\": false }#3#");
            //sb.Append("]");

            return MvcHtmlString.Create(sb.ToString());

        }
        return MvcHtmlString.Create("");

        //    var ObjectTable = TableName + UniqueID;

        //var value = ColumnData;
        //value = value.replace('###', '"').replace('###', '"');
        //value = value.replace("#2#", "'").replace("#2#", "'");
        //ColumnData = value;
        //var FinalData = '';
        //FinalData = ColumnData.toString();
        //var ColumnArray = FinalData.toString().split("#3#");
        //var aoColumns = [];
        //for (var i = 0; i <= ColumnArray.length - 1; i++) {
        //    if (i != (ColumnArray.length - 1))
        //        aoColumns.push(ColumnArray[i]);
        //}
        ////#3#
    }
}
