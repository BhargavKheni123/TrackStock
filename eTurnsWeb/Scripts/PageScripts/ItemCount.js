function DoNarrowSearchForItemCount() {
    $("#btnAddAll").show();
    var narrowSearchFields = '';
    var narrowSearchValues = '';
    var narrowSearchItem = '';

    eraseCookieforshift("selectstartindex");
    eraseCookieforshift("selectendindex");
    eraseCookieforshift("finalselectedarray");

    ///////////////////////////////////////////1///////////////////////////////////////////////////
    if (ItemLocationNarroValues != undefined && ItemLocationNarroValues.length > 0) {
        narrowSearchFields += "ItemLocations" + ",";
        narrowSearchValues += ItemLocationNarroValues + "@";
    }
    else {
        narrowSearchFields += "ItemLocations" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////////////////////2////////////////////////////////////////
    if (ManufacturerNarroValues != undefined && ManufacturerNarroValues.length > 0) {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += ManufacturerNarroValues + "@";
    }
    else {
        narrowSearchFields += "ManufacturerID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////3//////////////////////////////////////////////////////////////
    
    if (ItemTrackingTypeNarroValues != undefined && ItemTrackingTypeNarroValues.length > 0) {
        narrowSearchFields += "ItemTrackingType" + ",";
        narrowSearchValues += ItemTrackingTypeNarroValues + "@";
    }
    else {
        narrowSearchFields += "ItemTrackingType" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////4//////////////////////////////////////////////////////////

    if (ItemTypeNarroSearchValue != undefined && ItemTypeNarroSearchValue.length > 0) {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += ItemTypeNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "ItemType" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////5////////////////////////////////////////
    if (PullSupplierNarroValues != undefined && PullSupplierNarroValues.length > 0) {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += PullSupplierNarroValues + "@";
    }
    else {
        narrowSearchFields += "SupplierID" + ",";
        narrowSearchValues += "@";
    }

    ///////////////////////////////////////////////////////6///////////////////////////////////////
    if (PullCategoryNarroValues != undefined && PullCategoryNarroValues.length > 0) {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += PullCategoryNarroValues + "@";
    }
    else {
        narrowSearchFields += "CategoryID" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////7//////////////////////////////////////////////////////////////
    if (StockStatusTypeNarroValues != undefined && StockStatusTypeNarroValues.length > 0) {
        narrowSearchFields += "StockStatus" + ",";
        narrowSearchValues += StockStatusTypeNarroValues + "@";
    }
    else {
        narrowSearchFields += "StockStatus" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////8//////////////////////////////////////////////////////////////
    if (ItemActive != undefined && ItemActive.length > 0) {
        narrowSearchFields += "IsActive" + ",";
        narrowSearchValues += ItemActive + "@";
    }
    else {
        narrowSearchFields += "IsActive" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////9//////////////////////////////////////////////////////////
    if (CostNarroSearchValue != undefined && CostNarroSearchValue.length > 0) {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += CostNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "Cost" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////10/////////////////////////////////////////////////////////////
    if (AverageCostNarroSearchValue != undefined && AverageCostNarroSearchValue.length > 0) {

        narrowSearchFields += "AverageCost" + ",";
        narrowSearchValues += AverageCostNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "AverageCost" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////11//////////////////////////////////////////////////////////////
    if (TurnsNarroSearchValue != undefined && TurnsNarroSearchValue.length > 0) {

        narrowSearchFields += "Turns" + ",";
        narrowSearchValues += TurnsNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "Turns" + ",";
        narrowSearchValues += "@";
    }

    ///////////////////////////////////////////12///////////////////////////////////////////////////////
    if (InventoryClassificationNarroSearchValue != undefined && InventoryClassificationNarroSearchValue.length > 0) {
        narrowSearchFields += "InventoryClassification" + ",";
        narrowSearchValues += InventoryClassificationNarroSearchValue + "@";
    }
    else {
        narrowSearchFields += "InventoryClassification" + ",";
        narrowSearchValues += "@";
    }

    //////////////////////////////////////////////13//////////////////////////////////////////////////
    if (UserCreatedNarroValues != undefined && UserCreatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]CreatedBy#" + UserCreatedNarroValues;
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += UserCreatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += "@";
    }
    //////////////////////////////////////////////////14//////////////////////////////////////////////
    if (UserUpdatedNarroValues != undefined && UserUpdatedNarroValues.length > 0) {
        //narrowSearchItem += "[###]UpdatedBy#" + UserUpdatedNarroValues;
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += UserUpdatedNarroValues + "@";
    }
    else {
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////15//////////////////////////////////////////
    if (($('#DateCFromIM').val() != '' && $('#DateCToIM').val() != '')) {
        narrowSearchFields += "DateCreatedFrom" + ",";

        if (typeof $('#DateCToIM').val() != "undefined" && typeof $('#DateCFromIM').val() != "undefined" && $('#DateCFromIM').val() != '' && $('#DateCToIM').val() != '')
            narrowSearchValues += ($('#DateCFromIM').val()) + "," + ($('#DateCToIM').val()) + "@";
        else
            narrowSearchValues += "@";
    }
    else {
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += "@";
    }
    ///////////////////////////////////////////////////////16/////////////////////////////////////////
    if (($('#DateUFromIM').val() != '' && $('#DateUToIM').val() != '')) {
        narrowSearchFields += "DateUpdatedFrom" + ",";
        if (typeof $('#DateUFromIM').val() != "undefined" && typeof $('#DateUToIM').val() != "undefined" && $('#DateUToIM').val() != '' && $('#DateUFromIM').val() != '')
            narrowSearchValues += ($('#DateUFromIM').val()) + "," + ($('#DateUToIM').val()) + "@";
        else
            narrowSearchValues += "@";
    }
    else {
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += "@";
    }   

    //////////////////////////////////////////////////////17///////////////////////////////////////////
    if (UserUDF1NarrowValues != undefined && UserUDF1NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF1#" + UserUDF1NarrowValues;
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += UserUDF1NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////18//////////////////////////////////////////////
    if (UserUDF2NarrowValues != undefined && UserUDF2NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF2#" + UserUDF2NarrowValues;
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += UserUDF2NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////19///////////////////////////////////////////
    if (UserUDF3NarrowValues != undefined && UserUDF3NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF3#" + UserUDF3NarrowValues;
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += UserUDF3NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += "@";
    }
    ////////////////////////////////////////////////////20/////////////////////////////////////////
    if (UserUDF4NarrowValues != undefined && UserUDF4NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF4#" + UserUDF4NarrowValues;
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += UserUDF4NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += "@";
    }
    /////////////////////////////////////////////////////21////////////////////////////////////////
    if (UserUDF5NarrowValues != undefined && UserUDF5NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF5#" + UserUDF5NarrowValues;
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += UserUDF5NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////22////////////////////////////////////////
    if (UserUDF6NarrowValues != undefined && UserUDF6NarrowValues.length > 0) {
        narrowSearchFields += "UDF6" + ",";
        narrowSearchValues += UserUDF6NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF6" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////23////////////////////////////////////////
    if (UserUDF7NarrowValues != undefined && UserUDF7NarrowValues.length > 0) {
        narrowSearchFields += "UDF7" + ",";
        narrowSearchValues += UserUDF7NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF7" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////24////////////////////////////////////////
    if (UserUDF8NarrowValues != undefined && UserUDF8NarrowValues.length > 0) {
        narrowSearchFields += "UDF8" + ",";
        narrowSearchValues += UserUDF8NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF8" + ",";
        narrowSearchValues += "@";
    }
    
    /////////////////////////////////////////////////////25////////////////////////////////////////
    if (UserUDF9NarrowValues != undefined && UserUDF9NarrowValues.length > 0) {
        narrowSearchFields += "UDF9" + ",";
        narrowSearchValues += UserUDF9NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF9" + ",";
        narrowSearchValues += "@";
    }

    /////////////////////////////////////////////////////26////////////////////////////////////////
    if (UserUDF10NarrowValues != undefined && UserUDF10NarrowValues.length > 0) {
        narrowSearchFields += "UDF10" + ",";
        narrowSearchValues += UserUDF10NarrowValues + "@";
    }
    else {
        narrowSearchFields += "UDF10" + ",";
        narrowSearchValues += "@";
    }
    

    var searchtext = '';
    if ($("#ItemModel_filter").val() != '')
        searchtext = $("#ItemModel_filter").val().replace(/'/g, "''").replace(/&/g, "&amp;").replace(/>/g, "&gt;").replace(/</g, "&lt;").replace(/"/g, "&quot;");
    narrowSearch = narrowSearchFields + "[###]" + narrowSearchValues + "[###]" + searchtext;

    if (narrowSearch.length > 10)
        NarrowSearchInGridIM(narrowSearch);
    else if (UserCreatedNarroValues == undefined || UserCreatedNarroValues.length == 0 ||
         UserUpdatedNarroValues == undefined || UserUpdatedNarroValues.length == 0)
        NarrowSearchInGridIM(narrowSearch);
    
}


function NarrowSearchInGridIM(searchstr) {   
    FilterStringGlobalUse = searchstr;

    $('#ItemModeDataTable').dataTable().fnFilter(searchstr, null, null, null)
}
