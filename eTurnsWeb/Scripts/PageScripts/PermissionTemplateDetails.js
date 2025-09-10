$(document).ready(function () {

    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllView$Tab2']", function () {
        $("#Roomtab2").find("input[type='checkbox'][name$='IsView']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllInsert$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='IsInsert']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllUpdate$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='IsUpdate']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllDelete$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='IsDelete']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllShowDeleted$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='ShowDeleted']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllShowArchived$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='ShowArchived']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllShowUDF$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='ShowUDF']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab2").on("change", "input[type='checkbox'][id='AllShowChangeLog$Tab2']", function () {
        $("div#Roomtab2").find("input[type='checkbox'][name$='ShowChangeLog']").prop("checked", $(this).prop("checked"));
    });



    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllView$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='IsView']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllInsert$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='IsInsert']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllUpdate$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='IsUpdate']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllDelete$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='IsDelete']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllShowDeleted$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='ShowDeleted']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllShowArchived$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='ShowArchived']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllShowUDF$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='ShowUDF']").prop("checked", $(this).prop("checked"));
    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][id='AllShowChangeLog$Tab3']", function () {
        $("div#Roomtab3").find("input[type='checkbox'][name$='ShowChangeLog']").prop("checked", $(this).prop("checked"));
    });

    $("div#Roomtab2").on("change", "input[type='checkbox'][name$='RowSelectAll$Tab2']", function () {
        $(this).parent().parent().find("input:checkbox").prop("checked", $(this).prop("checked"));

    });
    $("div#Roomtab3").on("change", "input[type='checkbox'][name$='RowSelectAll$Tab3']", function () {
        $(this).parent().parent().find("input:checkbox").prop("checked", $(this).prop("checked"));

    });
});

function SelectAllTabChk(Chked, TabNO) {
    if (TabNO == '1') {
        $("#Roomtab2 input:checkbox").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '2') {
        $("#Roomtab3 input:checkbox").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '3') {
        $("#Roomtab4 input:checkbox").each(function () {
            var vModuleID = $(this).prop("id").split('_')[0]
            if ((vModuleID != "116" && vModuleID != "118" && vModuleID != "121") || Chked.checked == false)
                this.checked = Chked.checked;
            else
                this.checked = false;
        });
    }
}
function onSuccess(response) {
    IsRefreshGrid = true;
    showNotificationDialog();
    $("#spanGlobalMessage").html(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idValue = $("#hiddenID").val();

    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        clearControls('frmPermissionTemplate');

        $('input:checkbox').removeAttr('checked');
        $("#txtDescription").val("");

        $("#txtRoleName").val("");
        $("#txtRoleName").focus();
    }
    else if (idValue == 0) {
        clearControls('frmPermissionTemplate');
        $("#txtRoleName").val("");
        $("#txtRoleName").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            clearControls('frmPermissionTemplate');

        }
    }
    else if (idValue > 0) {

        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#txtRoleName").val("");
            $("#txtRoleName").focus();
        }
        else {
            clearControls('frmPermissionTemplate');
            SwitchTextTab(0, 'RoleCreate', 'frmPermissionTemplate');
        }
    }
    $('#NarroSearchClear').click();
}
function onFailure(message) {
    $("#spanGlobalMessage").html(message.statusText);
    showNotificationDialog();
    $("#txtRoleName").focus();
}
$('#btnCancel').click(function (e) {
    $('#NarroSearchClear').click();
    SwitchTextTab(0, 'RoleCreate', 'frmPermissionTemplate');
});