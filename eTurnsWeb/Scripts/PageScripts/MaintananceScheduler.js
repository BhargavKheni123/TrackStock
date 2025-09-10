//Reorder START
var objMaintananceColumn = {};
$("a#ColumnOrderSetupMaintenance").off("click");
$(document).on("click", "a#ColumnOrderSetup1", function () {
    $("#ColumnSortableModal1").dialog("open");
});

$("#ColumnSortableModal1").dialog({
    autoOpen: false,
    modal: true,
    width: 500,
    title: strReorderColumnPopupHeader,
    draggable: true,
    resizable: true,
    open: function () {
        GenerateColumnSortableMaintanace();
        $("#ColumnSortable1").sortable({ axis: "y", containment: "parent" });
    },
    close: function () {

    }
});

function SortableArrayMaintanance(ulColumnsOrder) {
    var sortableArray = '';
    $(ulColumnsOrder).children("li").each(function () {
        if (sortableArray != '') {
            sortableArray = sortableArray + "," + $(this).attr("id");
        }
        else {
            sortableArray = $(this).attr("id");
        }
    });
    return sortableArray;
}

function GetColumnIndexMaintanance(ColumnName) {
    if (typeof (objMaintananceColumn) != "undefined") {
        return objMaintananceColumn[ColumnName];
    }
}

function GenerateColumnSortableMaintanace() {
    var tempDuplicateColumns = new Array();
    var processedColumnIndex = new Array();
    //clear the old elements     
    var blankNUmber = 0;
    $('#ColumnSortable1 li').each(function (index) {
        $(this).remove();
    });
    var div = document.createElement('div');
    div.id = "CheckAll";
    //li.className = 'ui-state-default';
    //    li.innerHTML = oColumn.sTitle.trim();
    div.innerHTML = "<input type='hidden' value ='ColumnSortable1' id='hiddenDivName' /><a href='javascript:;' onclick='CheckAll(this);'  id='CheckAll' >CheckAll</a>&nbsp;<a href='javascript:;' onclick='UnCheckAll(this);' id='UnCheckAll' >UnCheckAll</a>" +
        "&nbsp;&nbsp;&nbsp;<input type='checkbox' onclick='ShowAllHidden(this);' id='ShowAllHidden' checked='checked' />Show All Hidden";
    $('div.sortableContainer').find("div#CheckAll").remove();
    $('div.sortableContainer').prepend(div);
    //get current columns order and generate li sortable accordingly

    var oTableReorder = $('#' + DTName).dataTable();

    //if ((typeof oTable) === 'undefined' && (typeof oTableReorderId) === 'undefined')
    //    return false;

    //if ((typeof oTable) === 'undefined' && oTableReorderId != null && oTableReorderId != undefined)
    //    oTableReorder = $('#' + oTableReorderId).dataTable();
    //else
    //    oTableReorder = oTable;

    var datatableColumns = oTableReorder.fnSettings().aoColumns;

    for (i = 0, iLen = datatableColumns.length; i < iLen; i++) {
        var oColumn = datatableColumns[i];
        var style = '';
        var LiStyle = 'display:block';
        var LiClassName = 'ui-state-default'
        if (oColumn.sClass.indexOf('NotHide') >= 0) {
            style = ' disabled="disabled" ';
            LiClassName = LiClassName + ' HiddenLi';
        }

        var colindxbyname = '';
        if (oColumn.sTitle.trim() != '') {
            colindxbyname = GetColumnIndexMaintanance(oColumn.sTitle.trim());
        }
        else {
            colindxbyname = GetColumnIndexMaintanance('blankFieldName' + blankNUmber);
        }

        processedColumnIndex.push(colindxbyname);

        var li = document.createElement('li');
        li.id = colindxbyname;
        li.className = LiClassName;
        li.innerHTML = oColumn.sTitle.trim();
        li.style = LiStyle;
        if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0) {
            if (oColumn.sTitle.trim() != '') {
                li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetColumnIndexMaintanance(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
            }
            else {
                li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetColumnIndexMaintanance('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
                blankNUmber++;
            }
        }
        else {

            if (oColumn.sTitle.trim() != '') {
                li.innerHTML = '<input type="checkbox" class="checkBox" id="' + GetColumnIndexMaintanance(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
            }
            else {

                li.innerHTML = '<input type="checkbox" class="checkBox" id="' + GetColumnIndexMaintanance('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
                blankNUmber++;
            }
        }
        $('#ColumnSortable1').append(li);


    }

}
//Reoreder END