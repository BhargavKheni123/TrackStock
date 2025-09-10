$(document).ready(function () {
    HasInsertPermission = $("#hdnInsertRights").val();

    if ($("#hdnInsertRights").val() == "False" && $("#hdnUpdateRights").val() == "True") {
        $(".tab1").hide();
        $("#tab1").hide();
    }
    else if ($("#hdnInsertRights").val() == "False" && $("#hdnUpdateRights").val() == "False") {
        $(".tab1").hide();
        $("#tab1").hide();
    }


    if ($("#hdnInsertRights").val() == "True" && $("#hdnUpdateRights").val() == "False") {
        HasOnlyViewPermission = true;
    }
    else if ($("#hdnInsertRights").val() == "False" && $("#hdnUpdateRights").val() == "False") {
        HasOnlyViewPermission = true;
    }
    else {
        HasOnlyViewPermission = false;
    }
});