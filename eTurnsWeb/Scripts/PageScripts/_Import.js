var ErrorLineNumber = '';
$(document).ready(function () {
    $("select[id='ddlModule']").find("option[value='-995']").text("Import Receipt-Add new Receipt");
    $('#btnAllRecordDownload').click(function () {
        var selectedModuleVal = $('#ddlModule option:selected').attr('value');
        var ModuleName = 'AssetMaster';

        if (selectedModuleVal == BinMasterselval) {
            ModuleName = 'AdjustmentCountCSV';
        }
        else if (selectedModuleVal == eVMISetupselval) {
            ModuleName = 'ItemLocationCSV';
        }
        else if (selectedModuleVal == LocationMasterselval) {
            ModuleName = 'LocationMasterList';
        }
        else if (selectedModuleVal == ToolCategoryselval) {
            ModuleName = 'ToolCategoryList';
        }
        else if (selectedModuleVal == CategoryMasterselval) {
            ModuleName = 'CategoryMasterList';
        }
        else if (selectedModuleVal == CustomerMasterselval) {
            ModuleName = 'CustomerMasterList';
        }
        else if (selectedModuleVal == FreightTypeMasterselval) {
            ModuleName = 'FreightTypeMasterList';
        }
        else if (selectedModuleVal == GLAccountsMasterselval) {
            ModuleName = 'GLAccountMasterList';
        }
        else if (selectedModuleVal == GXPRConsignedJobMasterselval) {
            ModuleName = '';
        }
        else if (selectedModuleVal == JobTypeMasterselval) {
            ModuleName = 'CategoryMasterList';
        }
        else if (selectedModuleVal == ShipViaMasterselval) {
            ModuleName = 'ShipViaMasterList';
        }
        else if (selectedModuleVal == TechnicianMasterselval) {
            ModuleName = 'TechnicianList';
        }
        else if (selectedModuleVal == ManufacturerMasterselval) {
            ModuleName = 'ManufacturerMasterList';
        }
        else if (selectedModuleVal == MeasurementTermMasterselval) {
            ModuleName = 'MeasurementTermList';
        }
        else if (selectedModuleVal == UnitMasterselval) {
            ModuleName = 'UnitMasterList';
        }
        else if (selectedModuleVal == SupplierMasterselval) {
            ModuleName = 'SupplierMasterList';
        }
        else if (selectedModuleVal == ItemMasterselval) {
            ModuleName = 'ItemMasterList';
        }
        else if (selectedModuleVal == BOMItemMasterselval) {
            ModuleName = 'BOMItemMasterList';
        }
        else if (selectedModuleVal == CostUOMMasterselval) {
            ModuleName = 'CostUOMMasterList';
        }
        else if (selectedModuleVal == InventoryClassificationMasterselval) {
            ModuleName = 'InventoryClassificationMasterList';
        }
        else if (selectedModuleVal == ToolMasterselval) {
            ModuleName = 'ToolList';
        }
        else if (selectedModuleVal == Assetsselval) {
            ModuleName = 'AssetMasterList';
        }
        else if (selectedModuleVal == QuickListPermissionselval) {
            ModuleName = 'QuickList';
        }
        else if (selectedModuleVal == Kitsselval) {
            ModuleName = 'KitsCSV';
        }
        else if (selectedModuleVal == ItemManufacturerselval) {
            ModuleName = 'ItemManufacturerCSV';
        }
        else if (selectedModuleVal == ItemSupplierselval) {
            ModuleName = 'ItemSupplierCSV';
        }
        else if (selectedModuleVal == BarcodeMasterselval) {
            ModuleName = 'BarcodeMasterCSV';
        }
        else if (selectedModuleVal == UDFselval) {
            ModuleName = 'UDFMasterCSV';
        }
        else if (selectedModuleVal == ProjectMasterselval) {
            ModuleName = 'ProjectMasterCSV';
        }
            //else if (selectedModuleVal == ItemLocationQtyval) {
            //    ModuleName = 'ItemLocationCSV';
            //}
        else if (selectedModuleVal == ManualCountval) {
            ModuleName = 'ManualCountCSV';
        }
        else if (selectedModuleVal == ItemLocationQtyval) {
            ModuleName = 'ItemLocationQtyCSV';
        }
        else if (selectedModuleVal == WorkOrderval) {
            ModuleName = 'WorkOrder';
        }
        else if (selectedModuleVal == PullImport) {
            ModuleName = 'PullImport';
        }
        
        ExportAllData(ModuleName, 'CSV');
    });
    $('#btnDownload').click(function () {

        var arrItems = new Array();
        var randomNumber = (new Date()).valueOf();
        var selectedModuleVal = $('#ddlModule option:selected').attr('value');

        if (selectedModuleVal == BinMasterselval) {
            window.open(PathToCSv + "AdjustmentCount.csv?q=et&_" + randomNumber);
        }
        if (selectedModuleVal == eVMISetupselval) {
            window.open(PathToCSv + "InventoryLocation.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == LocationMasterselval) {
            window.open(PathToCSv + "Location.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ToolCategoryselval) {
            window.open(PathToCSv + "ToolCategory.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == CategoryMasterselval) {
            window.open(PathToCSv + "Category.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == CustomerMasterselval) {
            window.open(PathToCSv + "Customer.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == FreightTypeMasterselval) {
            window.open(PathToCSv + "FreightType.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == GLAccountsMasterselval) {
            window.open(PathToCSv + "GLAccount.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == GXPRConsignedJobMasterselval) {
            window.open(PathToCSv + "GXPR Consigned Job.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == JobTypeMasterselval) {
            window.open(PathToCSv + "Job Type.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ShipViaMasterselval) {
            window.open(PathToCSv + "Ship Via.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == TechnicianMasterselval) {
            window.open(PathToCSv + "Technicians.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ManufacturerMasterselval) {
            window.open(PathToCSv + "Manufacturer.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == MeasurementTermMasterselval) {
            window.open(PathToCSv + "Measurement Term.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == UnitMasterselval) {
            window.open(PathToCSv + "Units.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == SupplierMasterselval) {
            window.open(PathToCSv + "Supplier.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ItemMasterselval) {
            window.open(PathToCSv + "Item.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == BOMItemMasterselval) {
            window.open(PathToCSv + "BOMItem.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == CostUOMMasterselval) {
            window.open(PathToCSv + "CostUOM.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == InventoryClassificationMasterselval) {
            window.open(PathToCSv + "InventoryClassification.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ToolMasterselval) {
            window.open(PathToCSv + "Tools.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == Assetsselval) {
            window.open(PathToCSv + "Assets.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == QuickListPermissionselval) {
            window.open(PathToCSv + "QuickList.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == Kitsselval) {
            window.open(PathToCSv + "Kits.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ItemManufacturerselval) {
            window.open(PathToCSv + "ItemManufacturer.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ItemSupplierselval) {
            window.open(PathToCSv + "ItemSupplier.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == BarcodeMasterselval) {
            window.open(PathToCSv + "BarcodeMaster.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == UDFselval) {
            window.open(PathToCSv + "UDFMaster.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ProjectMasterselval) {
            window.open(PathToCSv + "Projects.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ItemLocationQtyval) {
            window.open(PathToCSv + "ItemLocationQuantity.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ManualCountval) {
            window.open(PathToCSv + "ManualCount.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == WorkOrderval) {
            window.open(PathToCSv + "WorkOrder.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == PullImport) {
            window.open(PathToCSv + "PullImport.csv?q=et&_" + randomNumber);
        }        
    });



    $('#btnSave').click(function () {
        ErrorLineNumber = '';
        var Records = 0;
        var ErrorMessage = ShowingErrorMessage;
        $('#ImportProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
        //$('#DivLoading').show();

        if (!ValidateValueWithDataType()) {
            //ErrorMessage += " :" + ErrorLineNumber;
            closeImportInfoModel();
            $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");
            
            $('#ImportInfoDialog').modal();
            $('#DivLoading').hide();
            return;
        }



        $('#btnSave').prop("disabled", "disabled");

        var arrItems = new Array();
        var selectedModuleVal = $('#ddlModule option:selected').attr('value');

        if (selectedModuleVal == BinMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $(this).find('#aSerialNumber_' + tempINVLocID).val();
                var tempLotNumber = $(this).find('#aLotNumber_' + tempINVLocID).val();
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                //var tempReceived = $(this).find('#aReceived_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": tempSerialNumber, "LotNumber": tempLotNumber, "Expiration": tempExpiration,
                    "Cost": tempaCost
                };
                arrItems.push(data);
                if((Records % 1000) == 0)
                {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {
                            
                        }
                        , error: function (xhr, status, error) {

                           
                        },
                        complete: function () {
                            
                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ItemLocationQtyval) {
            if (!isUniqueSerialQty()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $(this).find('#aSerialNumber_' + tempINVLocID).val();
                var tempLotNumber = $(this).find('#aLotNumber_' + tempINVLocID).val();
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                var tempReceived = $(this).find('#aReceived_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": tempSerialNumber, "LotNumber": tempLotNumber, "Expiration": tempExpiration,
                    "Cost": tempaCost, "Received": tempReceived
                };
                arrItems.push(data);
                
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ManualCountval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $(this).find('#aSerialNumber_' + tempINVLocID).val();
                var tempLotNumber = $(this).find('#aLotNumber_' + tempINVLocID).val();
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                var tempReceived = $(this).find('#aReceived_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": tempSerialNumber, "LotNumber": tempLotNumber, "Expiration": tempExpiration,
                    "Cost": tempaCost, "Received": tempReceived
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == eVMISetupselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempminimumquantity = $(this).find('#aminimumquantity_' + tempINVLocID).val();
                var tempmaximumquantity = $(this).find('#amaximumquantity_' + tempINVLocID).val();
                var tempcriticalquantity = $(this).find('#acriticalquantity_' + tempINVLocID).val();
                var tempSensorId = $(this).find('#aSensorAddress_' + tempINVLocID).val();
                var tempSensorPort = $(this).find('#aSensorPort_' + tempINVLocID).val();
                var tempisdefault = $(this).find('#aisdefault_' + tempINVLocID).is(':checked');
                var tempisdeleted = $(this).find('#aisdeleted_' + tempINVLocID).is(':checked');

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "MinimumQuantity": tempminimumquantity, "MaximumQuantity": tempmaximumquantity,
                    "CriticalQuantity": tempcriticalquantity, "SensorId": tempSensorId, "SensorPort": tempSensorPort, "IsDefault": tempisdefault, "IsDeleted": tempisdeleted
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == LocationMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempLocationID = $(this).find('#LocationID').text();
                var tempLocation = $(this).find('#aLocation_' + tempLocationID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempLocationID, "BinNumber": tempLocation, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }

            });
        }
        else if (selectedModuleVal == ToolCategoryselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempToolCategoryID = $(this).find('#ToolCategoryID').text();
                var tempToolCategory = $(this).find('#aToolCategory_' + tempToolCategoryID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempToolCategoryID, "ToolCategory": tempToolCategory, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }

            });
        }
        else if (selectedModuleVal == CategoryMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempCatID = $(this).find('#CatID').text();
                var tempCategory = $(this).find('#aCategory_' + tempCatID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                if (vUDF1 == null)
                    vUDF1 = null;

                if (vUDF2 == null)
                    vUDF2 = null;
                if (vUDF3 == null)
                    vUDF3 = null;
                if (vUDF4 == null)
                    vUDF4 = null;
                if (vUDF5 == null)
                    vUDF5 = null;


                var data = { "Category": tempCategory, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }

            });
        }
        else if (selectedModuleVal == CostUOMMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempCostID = $(this).find('#CostID').text();
                var tempCost = $(this).find('#aCost_' + tempCostID).val();
                var tempCostvalue = $(this).find('#aCostvalue_' + tempCostID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempCostID, "CostUOM": tempCost, "CostUOMValue": tempCostvalue, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }

            });
        }
        else if (selectedModuleVal == InventoryClassificationMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempInvID = $(this).find('#InvID').text();
                var tempInv = $(this).find('#aInv_' + tempInvID).val();
                var tempBase = $(this).find('#aBase_' + tempInvID).val();
                var tempStart = $(this).find('#aStart_' + tempInvID).val();
                var tempEnd = $(this).find('#aEnd_' + tempInvID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempInvID, "InventoryClassification": tempInv, "BaseOfInventory": tempBase, "RangeStart": tempStart, "RangeEnd": tempEnd, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == CustomerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempCustID = $(this).find('#CustID').text();
                var tempCustomer = $(this).find('#aCustomer_' + tempCustID).val();
                var tempAccount = $(this).find('#aAccount_' + tempCustID).val();
                var tempContact = $(this).find('#aContact_' + tempCustID).val();
                var tempAddress = $(this).find('#aAddress_' + tempCustID).val();
                var tempCity = $(this).find('#aCity_' + tempCustID).val();
                var tempState = $(this).find('#aState_' + tempCustID).val();
                var tempZipCode = $(this).find('#aZipCode_' + tempCustID).val();
                var tempCountry = $(this).find('#aCountry_' + tempCustID).val();
                var tempPhone = $(this).find('#aPhone_' + tempCustID).val();
                var tempEmail = $(this).find('#aEmail_' + tempCustID).val();
                var remarks = $(this).find('#aRemarks_' + tempCustID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempCustID, "Customer": tempCustomer, "Account": tempAccount, "Contact": tempContact, "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry, "Phone": tempPhone, "Email": tempEmail, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, "remarks": remarks };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == FreightTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempFreightID = $(this).find('#FreightID').text();
                var tempFreightType = $(this).find('#aFreightType_' + tempFreightID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempFreightID, "FreightType": tempFreightType, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == GLAccountsMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempGLAccountID = $(this).find('#GLAccountID').text();
                var tempGLAccount = $(this).find('#aGLAccount_' + tempGLAccountID).val();
                var tempDesc = $(this).find('#aDesc_' + tempGLAccountID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempGLAccountID, "GLAccount": tempGLAccount, "Description": tempDesc, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == GXPRConsignedJobMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempGXPRID = $(this).find('#GXPRID').text();
                var tempGXPRConsigmentJob = $(this).find('#aGXPRConsinged_' + tempGXPRID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempGXPRID, "GXPRConsigmentJob": tempGXPRConsigmentJob, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == JobTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempJobTypeID = $(this).find('#JobTypeID').text();
                var tempJobType = $(this).find('#aJobType_' + tempJobTypeID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempJobTypeID, "JobType": tempJobType, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ShipViaMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempShipViaID = $(this).find('#ShipViaID').text();
                var tempShipVia = $(this).find('#aShipVia_' + tempShipViaID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempShipViaID, "ShipVia": tempShipVia, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == TechnicianMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Recods++;
                var tempTechnicianID = $(this).find('#TechnicianID').text();
                var tempTechnician = $(this).find('#aTechnician_' + tempTechnicianID).val();
                var tempTechnicianCode = $(this).find('#aTechnicianCode_' + tempTechnicianID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempTechnicianID, "Technician": tempTechnician, "TechnicianCode": tempTechnicianCode, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ManufacturerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempManufacturerID = $(this).find('#ManufacturerID').text();
                var tempManufacturer = $(this).find('#aManufacturer_' + tempManufacturerID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempManufacturerID, "Manufacturer": tempManufacturer, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == MeasurementTermMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempMeasurementTermID = $(this).find('#MeasurementTermID').text();
                var tempMeasurementTerm = $(this).find('#aMeasurementTerm_' + tempMeasurementTermID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempMeasurementTermID, "MeasurementTerm": tempMeasurementTerm, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == UnitMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempUnitID = $(this).find('#UnitID').text();
                var tempUnit = $(this).find('#aUnit_' + tempUnitID).val();

                var tempDescription = $(this).find('#aDescription_' + tempUnitID).val();
                var tempOdometer = $(this).find('#aOdometer_' + tempUnitID).val();
                var tempOpHours = $(this).find('#aOpHours_' + tempUnitID).val();
                var tempSerialNo = $(this).find('#aSerialNo_' + tempUnitID).val();
                var tempYear = $(this).find('#aYear_' + tempUnitID).val();
                var tempMake = $(this).find('#aMake_' + tempUnitID).val();
                var tempModel = $(this).find('#aModel_' + tempUnitID).val();
                var tempPlate = $(this).find('#aPlate_' + tempUnitID).val();
                var tempEngineModel = $(this).find('#aEngineModel_' + tempUnitID).val();
                var tempEngineSerialNo = $(this).find('#aEngineSerialNo_' + tempUnitID).val();
                var tempMarkupParts = $(this).find('#aMarkupParts_' + tempUnitID).val();
                var tempMarkupLabour = $(this).find('#aMarkupLabour_' + tempUnitID).val();

                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = {
                    "ID": tempUnitID, "Unit": tempUnit, "Description": tempDescription, "Odometer": tempOdometer,
                    "OpHours": parseFloat(tempOpHours), "SerialNo": parseFloat(tempSerialNo), "Year": parseFloat(tempYear), "Make": tempMake, "Model": tempModel, "Plate": tempPlate,
                    "EngineModel": tempEngineModel, "EngineSerialNo": tempEngineSerialNo, "MarkupParts": parseFloat(tempMarkupParts),
                    "MarkupLabour": parseFloat(tempMarkupLabour),
                    UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == SupplierMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempSupplierID = $(this).find('#SupplierID').text();
                var tempSupplier = $(this).find('#aSupplier_' + tempSupplierID).val();

                var tempSupplierColor = $(this).find('#aSupplierColor_' + tempSupplierID).val();
                var tempDescription = $(this).find('#aDescription_' + tempSupplierID).val();
                var tempBranchNumber = $(this).find('#aBranchNumber_' + tempSupplierID).val();
                var tempMaximumOrderSize = $(this).find('#aMaximumOrderSize_' + tempSupplierID).val();

                //var tempAccountNo = $(this).find('#aAccountNo_' + tempSupplierID).val();
                //var tempReceiverID = $(this).find('#aReceiverID_' + tempSupplierID).val();
                var tempAddress = $(this).find('#aAddress_' + tempSupplierID).val();
                var tempCity = $(this).find('#aCity_' + tempSupplierID).val();
                var tempState = $(this).find('#aState_' + tempSupplierID).val();
                var tempZipCode = $(this).find('#aZipCode_' + tempSupplierID).val();
                var tempCountry = $(this).find('#aCountry_' + tempSupplierID).val();
                var tempContact = $(this).find('#aContact_' + tempSupplierID).val();
                var tempPhone = $(this).find('#aPhone_' + tempSupplierID).val();
                var tempFax = $(this).find('#aFax_' + tempSupplierID).val();
                //var tempEmail = $(this).find('#aEmail_' + tempSupplierID).val();

                //                var tempIsEmailPOInBody = $(this).find('#aIsEmailPOInBody_' + tempSupplierID).is(':checked');
                //                var tempIsEmailPOInPDF = $(this).find('#aIsEmailPOInPDF_' + tempSupplierID).is(':checked');
                //                var tempIsEmailPOInCSV = $(this).find('#aIsEmailPOInCSV_' + tempSupplierID).is(':checked');
                //                var tempIsEmailPOInX12 = $(this).find('#aIsEmailPOInX12_' + tempSupplierID).is(':checked');

                var tempIsSendtoVendor = $(this).find('#aIsSendtoVendor_' + tempSupplierID).is(':checked');
                var tempIsVendorReturnAsn = $(this).find('#aIsVendorReturnAsn_' + tempSupplierID).is(':checked');
                var tempIsSupplierReceivesKitComponents = $(this).find('#aIsSupplierReceivesKitComponents_' + tempSupplierID).is(':checked');

                var tempOrderNumberTypeBlank = $(this).find('#aOrderNumberTypeBlank_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeFixed = $(this).find('#aOrderNumberTypeFixed_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeBlanketOrderNumber = $(this).find('#aOrderNumberTypeBlanketOrderNumber_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeIncrementingOrderNumber = $(this).find('#aOrderNumberTypeIncrementingOrderNumber_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeIncrementingbyDay = $(this).find('#aOrderNumberTypeIncrementingbyDay_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeDateIncrementing = $(this).find('#aOrderNumberTypeDateIncrementing_' + tempSupplierID).is(':checked');
                var tempOrderNumberTypeDate = $(this).find('#aOrderNumberTypeDate_' + tempSupplierID).is(':checked');

                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var tempAccountNumber = $(this).find('#aAccountNumber_' + tempSupplierID).val();
                var tempAccountName = $(this).find('#aAccountName_' + tempSupplierID).val();
                var tempBlanketPONumber = $(this).find('#aBlanketPONumber_' + tempSupplierID).val();
                var tempStartDate = $(this).find('#aStartDate_' + tempSupplierID).val();
                var tempEndDate = $(this).find('#aEndDate_' + tempSupplierID).val();
                var tempMaxLimit = $(this).find('#aMaxLimit_' + tempSupplierID).val();
                var tempDoNotExceed = $(this).find('#aDoNotExceed_' + tempSupplierID).is(':checked');

                var tempPullPurchaseNumberFixed = $(this).find('#aPullPurchaseNumberFixed_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberBlanketOrder = $(this).find('#aPullPurchaseNumberBlanketOrder_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberDateIncrementing = $(this).find('#aPullPurchaseNumberDateIncrementing_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberDate = $(this).find('#aPullPurchaseNumberDate_' + tempSupplierID).is(':checked');
                var tempLastPullPurchaseNumberUsed = $(this).find('#aLastPullPurchaseNumberUsed_' + tempSupplierID).val();
                var tempIsBlanketDeleted = $(this).find('#aIsBlanketDeleted_' + tempSupplierID).is(':checked');

                var data = {
                    "ID": tempSupplierID, "SupplierName": tempSupplier, "SupplierColor": tempSupplierColor, "Description": tempDescription,
                    "BranchNumber": tempBranchNumber, "MaximumOrderSize": tempMaximumOrderSize, //"ReceiverID": tempReceiverID,
                    "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry,
                    "Contact": tempContact, "Phone": tempPhone, "Fax": tempFax, "IsSendtoVendor": tempIsSendtoVendor,
                    "IsVendorReturnAsn": tempIsVendorReturnAsn, "IsSupplierReceivesKitComponents": tempIsSupplierReceivesKitComponents,
                    //"Email": tempEmail, "IsEmailPOInBody": tempIsEmailPOInBody, "IsEmailPOInPDF": tempIsEmailPOInPDF, "IsEmailPOInCSV": tempIsEmailPOInCSV,
                    //"IsEmailPOInX12": tempIsEmailPOInX12,
                    "OrderNumberTypeBlank": tempOrderNumberTypeBlank, "OrderNumberTypeFixed": tempOrderNumberTypeFixed, "OrderNumberTypeBlanketOrderNumber": tempOrderNumberTypeBlanketOrderNumber,
                    "OrderNumberTypeIncrementingOrderNumber": tempOrderNumberTypeIncrementingOrderNumber, "OrderNumberTypeIncrementingbyDay": tempOrderNumberTypeIncrementingbyDay,
                    "OrderNumberTypeDateIncrementing": tempOrderNumberTypeDateIncrementing, "OrderNumberTypeDate": tempOrderNumberTypeDate,
                    UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5,
                    "AccountNumber": tempAccountNumber, "AccountName": tempAccountName, "BlanketPONumber": tempBlanketPONumber, "StartDate": tempStartDate, "EndDate": tempEndDate,
                    "MaxLimit": tempMaxLimit, "IsNotExceed": tempDoNotExceed, "PullPurchaseNumberFixed": tempPullPurchaseNumberFixed, "PullPurchaseNumberBlanketOrder": tempPullPurchaseNumberBlanketOrder,
                    "PullPurchaseNumberDateIncrementing": tempPullPurchaseNumberDateIncrementing, "PullPurchaseNumberDate": tempPullPurchaseNumberDate, "LastPullPurchaseNumberUsed": tempLastPullPurchaseNumberUsed,
                    "IsBlanketDeleted": tempIsBlanketDeleted
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ItemMasterselval) {
            if (!ValidateCriticalMinMax()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            if (!ValidateImageName()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempItemID = $(this).find('#ItemID').text();

                var tempItemNumber = $(this).find('#aItemNumber_' + tempItemID).val();
                var tempManufacturer = $(this).find('#aManufacturer_' + tempItemID).val();
                var tempManufacturerNumber = $(this).find('#aManufacturerNumber_' + tempItemID).val();
                var tempSupplierName = $(this).find('#aSupplierName_' + tempItemID).val();
                var tempBlanketOrderNumber = $(this).find('#aBlanketOrderNumber_' + tempItemID).val();
                var tempSupplierPartNo = $(this).find('#aSupplierPartNo_' + tempItemID).val();
                var tempUPC = $(this).find('#aUPC_' + tempItemID).val();
                var tempUNSPSC = $(this).find('#aUNSPSC_' + tempItemID).val();
                var tempDescription = $(this).find('#aDescription_' + tempItemID).val();
                var tempLongDescription = $(this).find('#aLongDescription_' + tempItemID).val();
                var tempCategoryName = $(this).find('#aCategoryName_' + tempItemID).val();
                var tempGLAccount = $(this).find('#aGLAccount_' + tempItemID).val();
                var tempUOM = $(this).find('#aUOM_' + tempItemID).val();
                var tempCostUOM = $(this).find('#aCostUOM_' + tempItemID).val();
                //var tempPricePerTerm = $(this).find('#aPricePerTerm_' + tempItemID).val();
                var DefaultReorderQuantity = $(this).find('#aDefaultReorderQuantity_' + tempItemID).val();
                var DefaultPullQuantity = $(this).find('#aDefaultPullQuantity_' + tempItemID).val();
                var Cost = $(this).find('#aCost_' + tempItemID).val();
                var Markup = $(this).find('#aMarkup_' + tempItemID).val();
                var SellPrice = $(this).find('#aSellPrice_' + tempItemID).val();
                var ExtendedCost = $(this).find('#aExtendedCost_' + tempItemID).text();
                var LeadTimeInDays = $(this).find('#aLeadTimeInDays_' + tempItemID).val();
                var Link1 = $(this).find('#aLink1_' + tempItemID).val();
                var Link2 = $(this).find('#aLink2_' + tempItemID).val();
                var Trend = $(this).find('#aTrend_' + tempItemID).is(':checked');
                var Taxable = $(this).find('#aTaxable_' + tempItemID).is(':checked');
                var Consignment = $(this).find('#aConsignment_' + tempItemID).is(':checked');
                var StagedQuantity = $(this).find('#aStagedQuantity_' + tempItemID).text();
                var InTransitquantity = $(this).find('#aIn_Transitquantity_' + tempItemID).text();
                var OnOrderQuantity = $(this).find('#aOnOrderQuantity_' + tempItemID).text();
                var OnTransferQuantity = $(this).find('#aOnTransferQuantity_' + tempItemID).text();
                var SuggestedOrderQuantity = $(this).find('#aSuggestedOrderQuantity_' + tempItemID).text();
                var RequisitionedQuantity = $(this).find('#aRequisitionedQuantity_' + tempItemID).text();
                var AverageUsage = $(this).find('#aAverageUsage_' + tempItemID).text();
                var Turns = $(this).find('#aTurns_' + tempItemID).text();
                var OnHandQuantity = $(this).find('#aOn_HandQuantity_' + tempItemID).val();
                var CriticalQuantity = $(this).find('#aCriticalQuantity_' + tempItemID).val();
                var MinimumQuantity = $(this).find('#aMinimumQuantity_' + tempItemID).val();
                var MaximumQuantity = $(this).find('#aMaximumQuantity_' + tempItemID).val();
                var WeightPerPiece = $(this).find('#aWeightPerPiece_' + tempItemID).val();
                var ItemUniqueNumber = $(this).find('#aItemUniqueNumber_' + tempItemID).val();
                var IsTransfer = $(this).find('#aIsTransfer_' + tempItemID).is(':checked');
                var IsPurchase = $(this).find('#aIsPurchase_' + tempItemID).is(':checked');
                var InventryLocation = $(this).find('#aInventryLocation_' + tempItemID).val();
                var InventoryClassification = $(this).find('#aInventoryClassification_' + tempItemID).val();
                var SerialNumberTracking = $(this).find('#aSerialNumberTracking_' + tempItemID).is(':checked');
                var LotNumberTracking = $(this).find('#aLotNumberTracking_' + tempItemID).is(':checked');
                var DateCodeTracking = $(this).find('#aDateCodeTracking_' + tempItemID).is(':checked');
                var ItemType = $(this).find('#aItemType_' + tempItemID).val();
                var ImagePath = $(this).find('#aImagePath_' + tempItemID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();
                var IsLotSerialExpiryCost = $(this).find('#aIsLotSerialExpiryCost_' + tempItemID).val();
                var ItemLevelMinMaxQtyRequired = $(this).find('#aItemLevelMinMaxQtyRequired_' + tempItemID).is(':checked');

                var IsBuildBreak = $(this).find('#aIsBuildBreak_' + tempItemID).is(':checked');
                var IsAutoInventoryClassification = $(this).find('#aIsAutoInventoryClassification_' + tempItemID).is(':checked');
                var PullQtyScanOverride = $(this).find('#aPullQtyScanOverride_' + tempItemID).is(':checked');
                var EnforceDefaultReorderQuantity = $(this).find('#aEnforceDefaultReorderQuantity_' + tempItemID).is(':checked');
                var TrendingSetting = $(this).find('#atrendingsetting_' + tempItemID).val();

                var IsPackslipMandatoryAtReceive = $(this).find('#aIsPackslipMandatoryAtReceive_' + tempItemID).is(':checked');

                var tempItemImageExternalURL = $(this).find('#aItemImageExternalURL_' + tempItemID).val();
                var tempItemDocExternalURL = $(this).find('#aItemDocExternalURL_' + tempItemID).val();
                var ItemIsDeleted = $(this).find('#aItemIsDeleted_' + tempItemID).is(':checked');
                var ItemLink2ExternalURL = $(this).find('#aItemLink2ExternalURL_' + tempItemID).val();
                //alert($(this).find('#aIsPackslipMandatoryAtReceive' + tempItemID).is(':checked'));

                if (CriticalQuantity == null || CriticalQuantity == '') {
                    CriticalQuantity = 0;
                }
                if (MinimumQuantity == null || MinimumQuantity == '') {
                    MinimumQuantity = 0;
                }
                if (MaximumQuantity == null || MaximumQuantity == '') {
                    MaximumQuantity = 0;
                }

                var data = {
                    // "ID": tempItemID,
                    "ItemNumber": tempItemNumber,
                    "ManufacturerName": tempManufacturer,
                    "ManufacturerNumber": tempManufacturerNumber,
                    "SupplierName": tempSupplierName,
                    "BlanketOrderNumber": tempBlanketOrderNumber,
                    "SupplierPartNo": tempSupplierPartNo,
                    "UPC": tempUPC,
                    "UNSPSC": tempUNSPSC,
                    "Description": tempDescription,
                    "LongDescription": tempLongDescription,
                    "CategoryName": tempCategoryName,
                    "GLAccount": tempGLAccount,
                    "Unit": tempUOM,
                    // CommentBY Esha For Cost UOM
                    //"PricePerTerm": parseFloat(tempPricePerTerm),
                    // Added BY Esha
                    //"PricePerTerm": 0,
                    "CostUOMName": tempCostUOM,
                    // End

                    "DefaultReorderQuantity": parseFloat(DefaultReorderQuantity),
                    "DefaultPullQuantity": parseFloat(DefaultPullQuantity),
                    "Cost": parseFloat(Cost),
                    "Markup": parseFloat(Markup),
                    "SellPrice": parseFloat(SellPrice),
                    "ExtendedCost": parseFloat(ExtendedCost),
                    "LeadTimeInDays": parseInt(LeadTimeInDays),
                    "Link1": Link1,
                    "Link2": Link2,
                    "Trend": Trend,
                    "Taxable": Taxable,
                    "Consignment": Consignment,
                    "StagedQuantity": parseFloat(StagedQuantity),
                    "InTransitquantity": parseFloat(InTransitquantity),
                    "OnOrderQuantity": parseFloat(OnOrderQuantity),
                    "OnTransferQuantity": parseFloat(OnTransferQuantity),
                    "SuggestedOrderQuantity": parseFloat(SuggestedOrderQuantity),
                    "RequisitionedQuantity": parseFloat(RequisitionedQuantity),
                    "AverageUsage": parseFloat(AverageUsage),
                    "Turns": parseFloat(Turns),
                    "OnHandQuantity": parseFloat(OnHandQuantity),
                    "CriticalQuantity": parseFloat(CriticalQuantity),
                    "MinimumQuantity": parseFloat(MinimumQuantity),
                    "MaximumQuantity": parseFloat(MaximumQuantity),
                    "WeightPerPiece": parseFloat(WeightPerPiece),
                    "ItemUniqueNumber": ItemUniqueNumber,
                    "IsTransfer": IsTransfer,
                    "IsPurchase": IsPurchase,
                    "InventryLocation": InventryLocation,
                    "InventoryClassificationName": InventoryClassification,
                    "SerialNumberTracking": SerialNumberTracking,
                    "LotNumberTracking": LotNumberTracking,
                    "DateCodeTracking": DateCodeTracking,
                    "ItemTypeName": ItemType,
                    "ImagePath": ImagePath,
                    "UDF1": vUDF1,
                    "UDF2": vUDF2,
                    "UDF3": vUDF3,
                    "UDF4": vUDF4,
                    "UDF5": vUDF5,
                    "IsLotSerialExpiryCost": IsLotSerialExpiryCost,
                    "IsItemLevelMinMaxQtyRequired": ItemLevelMinMaxQtyRequired,
                    "TrendingSettingName": TrendingSetting,
                    "PullQtyScanOverride": PullQtyScanOverride,
                    "IsEnforceDefaultReorderQuantity": EnforceDefaultReorderQuantity,
                    "IsAutoInventoryClassification": IsAutoInventoryClassification,
                    "IsBuildBreak": IsBuildBreak,
                    "IsPackslipMandatoryAtReceive": IsPackslipMandatoryAtReceive,
                    "ItemImageExternalURL": tempItemImageExternalURL,
                    "ItemDocExternalURL": tempItemDocExternalURL,
                    "IsDeleted": ItemIsDeleted,
                    "ItemLink2ExternalURL": ItemLink2ExternalURL
                };


                //                var data = { "ID": tempItemID, "SupplierName": tempSupplier, "Description": tempDescription, "AccountNo": tempAccountNo,
                //                    "ReceiverID": tempReceiverID, "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry,
                //                    "Contact": tempContact, "Phone": tempPhone, "Fax": tempFax,
                //                    "Email": tempEmail, "IsEmailPOInBody": tempIsEmailPOInBody, "IsEmailPOInPDF": tempIsEmailPOInPDF, "IsEmailPOInCSV": tempIsEmailPOInCSV,
                //                    "IsEmailPOInX12": tempIsEmailPOInX12, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                //                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });

        }
        else if (selectedModuleVal == BOMItemMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempItemID = $(this).find('#BOMItemID').text();

                var tempItemNumber = $(this).find('#aBOMItemNumber_' + tempItemID).val();
                var tempManufacturer = $(this).find('#aBOMManufacturer_' + tempItemID).val();
                var tempManufacturerNumber = $(this).find('#aBOMManufacturerNumber_' + tempItemID).val();
                var tempSupplierName = $(this).find('#aBOMSupplierName_' + tempItemID).val();
                var tempSupplierPartNo = $(this).find('#aBOMSupplierPartNo_' + tempItemID).val();
                var tempUPC = $(this).find('#aBOMUPC_' + tempItemID).val();
                var tempUNSPSC = $(this).find('#aBOMUNSPSC_' + tempItemID).val();
                var tempDescription = $(this).find('#aBOMDescription_' + tempItemID).val();
                var tempCategoryName = $(this).find('#aBOMCategoryName_' + tempItemID).val();
                var tempGLAccount = $(this).find('#aBOMGLAccount_' + tempItemID).val();
                var tempUnit = $(this).find('#aBOMUnit_' + tempItemID).val();
                var LeadTimeInDays = $(this).find('#aLeadTimeInDays_' + tempItemID).val();
                var Taxable = $(this).find('#aBOMTaxable_' + tempItemID).is(':checked');
                var Consignment = $(this).find('#aBOMConsignment_' + tempItemID).is(':checked');
                var ItemUniqueNumber = $(this).find('#aBOMItemUniqueNumber_' + tempItemID).val();
                var IsTransfer = $(this).find('#aBOMIsTransfer_' + tempItemID).is(':checked');
                var IsPurchase = $(this).find('#aBOMIsPurchase_' + tempItemID).is(':checked');
                //var InventryLocation = $(this).find('#aBOMInventryLocation_' + tempItemID).val();
                var InventoryClassification = $(this).find('#aBOMInventoryClassification_' + tempItemID).val();
                var SerialNumberTracking = $(this).find('#aBOMSerialNumberTracking_' + tempItemID).is(':checked');
                var LotNumberTracking = $(this).find('#aBOMLotNumberTracking_' + tempItemID).is(':checked');
                var DateCodeTracking = $(this).find('#aBOMDateCodeTracking_' + tempItemID).is(':checked');
                var ItemType = $(this).find('#aBOMItemType_' + tempItemID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = {
                    // "ID": tempItemID,
                    "ItemNumber": tempItemNumber,
                    "ManufacturerName": tempManufacturer,
                    "ManufacturerNumber": tempManufacturerNumber,
                    "SupplierName": tempSupplierName,
                    "SupplierPartNo": tempSupplierPartNo,
                    "UPC": tempUPC,
                    "UNSPSC": tempUNSPSC,
                    "Description": tempDescription,
                    "CategoryName": tempCategoryName,
                    "GLAccount": tempGLAccount,
                    "Unit": tempUnit,
                    "LeadTimeInDays": LeadTimeInDays,
                    "Taxable": Taxable,
                    "Consignment": Consignment,
                    "ItemUniqueNumber": ItemUniqueNumber,
                    "IsTransfer": IsTransfer,
                    "IsPurchase": IsPurchase,
                    //"InventryLocation": InventryLocation,
                    "InventoryClassificationName": InventoryClassification,
                    "SerialNumberTracking": SerialNumberTracking,
                    "LotNumberTracking": LotNumberTracking,
                    "DateCodeTracking": DateCodeTracking,
                    "ItemTypeName": ItemType,
                    UDF1: vUDF1,
                    UDF2: vUDF2,
                    UDF3: vUDF3,
                    UDF4: vUDF4,
                    UDF5: vUDF5
                };

                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });

        }
        else if (selectedModuleVal == ToolMasterselval) {


            if (!isUniqueSerial()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }

            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempToolID = $(this).find('#ToolID').text();

                var tempToolName = $(this).find('#aToolName_' + tempToolID).val();

                var tempSerial = $(this).find('#aSerial_' + tempToolID).val();
                var tempDescription = $(this).find('#aDescription_' + tempToolID).val();
                var tempCost = $(this).find('#aCost_' + tempToolID).val();
                var tempQuantity = $(this).find('#aQuantity_' + tempToolID).val();
                var tempToolCategory = $(this).find('#aToolCategory_' + tempToolID).val();
                var tempLocation = $(this).find('#aLocation_' + tempToolID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();
                var vTechnician = $(this).find('#aTechnician_' + tempToolID).val();
                var vCheckOutQty = $(this).find('#aCheckOutQty_' + tempToolID).val();
                var vCheckInQty = $(this).find('#aCheckInQty_' + tempToolID).val();
                var tempIsGrpofItems = 0;
                var vCheckOutUDF1 = $(this).find('#aCheckOutUDF1').val();
                var vCheckOutUDF2 = $(this).find('#aCheckOutUDF2').val();
                var vCheckOutUDF3 = $(this).find('#aCheckOutUDF3').val();
                var vCheckOutUDF4 = $(this).find('#aCheckOutUDF4').val();
                var vCheckOutUDF5 = $(this).find('#aCheckOutUDF5').val();

                if ($(this).find("input[type='checkbox'][id*='aisgroupofitems']").is(':checked'))
                    tempIsGrpofItems = 1;
                else
                    tempIsGrpofItems = 0;
                var data = {
                    "ID": tempToolID, "ToolName": tempToolName, "Serial": tempSerial, "Description": tempDescription,
                    "Cost": parseFloat(tempCost), "Quantity": parseFloat(tempQuantity), "ToolCategory": tempToolCategory, "Location": tempLocation,
                    UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, IsGroupOfItems: tempIsGrpofItems, "Technician": vTechnician, "CheckOutQuantity": vCheckOutQty,
                    "CheckInQuantity": vCheckInQty, "CheckOutUDF1": vCheckOutUDF1, "CheckOutUDF2": vCheckOutUDF2, "CheckOutUDF3": vCheckOutUDF3, "CheckOutUDF4": vCheckOutUDF4, "CheckOutUDF5": vCheckOutUDF5
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }


            });

        }
        else if (selectedModuleVal == Assetsselval) {
            //if (!ValidatePurchaseDate()) {
            //    $('#DivLoading').hide();
            //    $('#btnSave').removeProp("disabled");
            //    closeImportInfoModel();
            //    return;
            //}
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempAssetID = $(this).find('#AssetID').text();
                var tempAssetName = $(this).find('#aAssetName_' + tempAssetID).val();
                var tempSerial = $(this).find('#aSerial_' + tempAssetID).val();
                var tempDescription = $(this).find('#aDescription_' + tempAssetID).val();
                var tempMake = $(this).find('#aMake_' + tempAssetID).val();
                var tempModel = $(this).find('#aModel_' + tempAssetID).val();
                var tempToolCategory = $(this).find('#aToolCategory_' + tempAssetID).val();
                var tempPurchaseDate = $(this).find('#aPurchaseDate_' + tempAssetID).val();


                var tempPurchasePrice = $(this).find('#aPurchasePrice_' + tempAssetID).val();
                var tempDepreciatedValue = $(this).find('#aDepreciatedValue_' + tempAssetID).val();
                var tempAssetCategory = $(this).find('#aAssetCategoryValue_' + tempAssetID).val();

                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var vUDF6 = $(this).find('#aUDF6').val();
                var vUDF7 = $(this).find('#aUDF7').val();
                var vUDF8 = $(this).find('#aUDF8').val();
                var vUDF9 = $(this).find('#aUDF9').val();
                var vUDF10 = $(this).find('#aUDF10').val();


                var data = {
                    "ID": tempAssetID, "AssetName": tempAssetName, "Serial": tempSerial, "Description": tempDescription,
                    "Make": tempMake, "Model": tempModel, "ToolCategory": tempToolCategory, "PurchaseDateString": tempPurchaseDate,
                    "PurchasePrice": tempPurchasePrice, "DepreciatedValue": tempDepreciatedValue, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5,
                    UDF6: vUDF6, UDF7: vUDF7, UDF8: vUDF8, UDF9: vUDF9, UDF10: vUDF10,
                    "AssetCategory": tempAssetCategory
                };

                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == QuickListPermissionselval) {

            if (!isUniqueQuickListBin()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempQuickListID = $(this).find('#QuickListID').text();
                var tempQuickListname = $(this).find('#aQuickListname_' + tempQuickListID).val();
                var tempType = $(this).find('#aType_' + tempQuickListID).val();
                var tempComments = $(this).find('#aComments_' + tempQuickListID).val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempQuickListID).val();
                var tempBinNumber = $(this).find('#aBinNumber_' + tempQuickListID).val();
                var tempQuantity = $(this).find('#aQuantity_' + tempQuickListID).val();
                var tempConsignedQuantity = $(this).find('#aConsignedQuantity_' + tempQuickListID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = {
                    "ID": tempQuickListID, "QuickListname": tempQuickListname, "Type": tempType, "Comments": tempComments, "ItemNumber": tempItemNumber, "BinNumber": tempBinNumber,
                    "Quantity": tempQuantity, "ConsignedQuantity": tempConsignedQuantity, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == Kitsselval) {

            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempKitID = $(this).find('#KitID').text();
                var tempKitPartNumber = $(this).find('#aKitPartNumber_' + tempKitID).val();

                var tempKitItemNumber = $(this).find('#aKitItemNumber_' + tempKitID).val();
                var tempQuantityPerKit = $(this).find('#aQuantityPerKit_' + tempKitID).val();
                //var tempOnHandQuantity = $(this).find('#aOnHandQuantity_' + tempKitID).val();
                var tempIsBuildBreak = 1;
                if ($(this).find("input[type='checkbox'][id*='aIsBuildBreak_" + tempKitID + "']").is(':checked'))
                    tempIsBuildBreak = true;
                else
                    tempIsBuildBreak = false;
                var tempIsDelete = 1;
                if ($(this).find("input[type='checkbox'][id*='aIsDelete_" + tempKitID + "']").is(':checked'))
                    tempIsDelete = true;
                else
                    tempIsDelete = false;
                //var tempAvailableItemsInWIP = $(this).find('#aAvailableItemsInWIP_' + tempKitID).val();
                var tempDescription = $(this).find('#aDescription_' + tempKitID).val();
                var tempCriticalQuantity = $(this).find('#aCriticalQuantity_' + tempKitID).val();
                var tempMinimumQuantity = $(this).find('#aMinimumQuantity_' + tempKitID).val();
                var tempMaximumQuantiy = $(this).find('#aMaximumQuantiy_' + tempKitID).val();
                var tempReOrderType = $(this).find('#aReOrderType_' + tempKitID).val();
                var tempKitCategory = $(this).find('#aKitCategory_' + tempKitID).val();
                var tempKitDemand = $(this).find('#aKitDemand_' + tempKitID).val();
                var tempAvailableKitQuantity = $(this).find('#aAvailableKitQuantity_' + tempKitID).val();
                ////////////////////////////////////////////////////////
                var tempSupplierName = $(this).find('#aSupplierName_' + tempKitID).val();

                var tempSupplierPartNo = $(this).find('#aSupplierPartNo_' + tempKitID).val();
                var tempDefaultLocation = $(this).find('#aDefaultLocation_' + tempKitID).val();
                var tempCostUOMName = $(this).find('#aCostUOMName_' + tempKitID).val();

                var tempUOM = $(this).find('#aUOM_' + tempKitID).val();

                var tempDefaultReorderQuantity = $(this).find('#aDefaultReorderQuantity_' + tempKitID).val();
                var tempDefaultPullQuantity = $(this).find('#aDefaultPullQuantity_' + tempKitID).val();
                //var tempItemTypeName = $(this).find('#aItemTypeName_' + tempKitID).val();
                var tempIsItemLevelMinMaxQtyRequired = 1;
                if ($(this).find("input[type='checkbox'][id*='aIsItemLevelMinMaxQtyRequired_" + tempKitID + "']").is(':checked'))
                    tempIsItemLevelMinMaxQtyRequired = true;
                else
                    tempIsItemLevelMinMaxQtyRequired = false;

                var data = {
                    "ID": tempKitID, "KitPartNumber": tempKitPartNumber, "ItemNumber": tempKitItemNumber, "QuantityPerKit": tempQuantityPerKit, "IsBuildBreak": tempIsBuildBreak, "IsDeleted": tempIsDelete
                    , "Description": tempDescription, "CriticalQuantity": tempCriticalQuantity, "MinimumQuantity": tempMinimumQuantity, "AvailableKitQuantity": tempAvailableKitQuantity,
                    "MaximumQuantiy": tempMaximumQuantiy, "ReOrderType": tempReOrderType, "KitCategory": tempKitCategory, "KitDemand": tempKitDemand,
                    "SupplierName": tempSupplierName, "SupplierPartNo": tempSupplierPartNo, "DefaultLocationName": tempDefaultLocation, "CostUOMName": tempCostUOMName,
                    "UOM": tempUOM, "DefaultReorderQuantity": tempDefaultReorderQuantity, "DefaultPullQuantity": tempDefaultPullQuantity, "IsItemLevelMinMaxQtyRequired": tempIsItemLevelMinMaxQtyRequired

                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ItemManufacturerselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempManufacturerID = $(this).find('#ManufacturerID').text();
                var tempItemnumber = $(this).find('#aItemNumber_' + tempManufacturerID).val();

                var tempManufacturerName = $(this).find('#aManufacturerName_' + tempManufacturerID).val();
                var tempManufacturerNumber = $(this).find('#aManufacturerNumber_' + tempManufacturerID).val();

                var tempIsDefault = 1;
                if ($(this).find("input[type='checkbox'][id*='aIsDefault_" + tempManufacturerID + "']").is(':checked'))
                    tempIsDefault = true;
                else
                    tempIsDefault = false;

                var data = {
                    "ID": tempManufacturerID, "Itemnumber": tempItemnumber, "ManufacturerName": tempManufacturerName, "ManufacturerNumber": tempManufacturerNumber, "IsDefault": tempIsDefault
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ItemSupplierselval) {
            if (!isUniqueSupplierPartNumber()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }

            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempSupplierID = $(this).find('#SupplierID').text();
                var tempItemnumber = $(this).find('#aItemNumber_' + tempSupplierID).val();

                var tempSupplierName = $(this).find('#aSupplierName_' + tempSupplierID).val();
                var tempSupplierNumber = $(this).find('#aSupplierNumber_' + tempSupplierID).val();
                var tempBlanketPOID = $(this).find('#aBlanketPOID_' + tempSupplierID).val();

                var tempIsDefault = 1;
                if ($(this).find("input[type='checkbox'][id*='aIsDefault_']").is(':checked'))
                    tempIsDefault = true;
                else
                    tempIsDefault = false;

                var data = {
                    "ID": tempSupplierID, "Itemnumber": tempItemnumber, "SupplierName": tempSupplierName, "SupplierNumber": tempSupplierNumber, "IsDefault": tempIsDefault, "BlanketPOName": tempBlanketPOID
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == BarcodeMasterselval) {
            if (!checkModuleName()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempBarcodeID = $(this).find('#BarcodeID').text();
                var tempModuleName = $(this).find('#aModuleName_' + tempBarcodeID).val();
                var tempItemnumber = $(this).find('#aItemNumber_' + tempBarcodeID).val();
                var tempBarCodeString = $(this).find('#aBarCodeString_' + tempBarcodeID).val();
                var tempBinNumber = $(this).find('#aBinNumber_' + tempBarcodeID).val();



                var data = {
                    "ID": tempBarcodeID, "Itemnumber": tempItemnumber, "ModuleName": tempModuleName, "BarCodeString": tempBarCodeString, "BinNumber": tempBinNumber
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == UDFselval) {
            //if (!checkModuleName()) {
            //    $('#DivLoading').hide();
            //    $('#btnSave').removeProp("disabled");
            //    closeImportInfoModel();
            //    return;
            //}
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempUDFID = $(this).find('#UDFID').text();
                var tempModuleName = $(this).find('#aModuleName_' + tempUDFID).val();
                var tempControlType = $(this).find('#aControlType_' + tempUDFID).val();
                var tempUDFName = $(this).find('#aUDFName_' + tempUDFID).val();
                var tempUDFColumnName = $(this).find('#aUDFColumnName_' + tempUDFID).val();
                var tempDefaultValue = $(this).find('#aDefaultValue_' + tempUDFID).val();
                var tempOptionName = $(this).find('#aOptionName_' + tempUDFID).val();
                var tempIsRequired = $(this).find('#aIsRequired_' + tempUDFID).is(":checked");
                var tempIsDeleted = $(this).find('#aIsDeleted_' + tempUDFID).is(":checked");
                var tempIncludeInNarrowSearch = $(this).find('#aIncludeInNarrowSearch_' + tempUDFID).is(":checked");



                var data = {
                    "ID": tempUDFID, "UDFName": tempUDFName, "UDFColumnName": tempUDFColumnName, "ModuleName": tempModuleName, "ControlType": tempControlType, "DefaultValue": tempDefaultValue, "OptionName": tempOptionName, "IsRequired": tempIsRequired, "IsDeleted": tempIsDeleted, "IncludeInNarrowsearch": tempIncludeInNarrowSearch
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == ProjectMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempProjectMasterID = $(this).find('#ProjectMasterID').text();
                var tempProjectSpendName = $(this).find('#aProjectSpendName_' + tempProjectMasterID).val();
                var tempDescription = $(this).find('#aDescription_' + tempProjectMasterID).val();
                var tempDollarLimitAmount = $(this).find('#aDollarLimitAmount_' + tempProjectMasterID).val();
                var tempTrackAllUsageAgainstThis = $(this).find('#aTrackAllUsageAgainstThis_' + tempProjectMasterID).is(":checked");
                var tempIsClosed = $(this).find('#aIsClosed_' + tempProjectMasterID).is(":checked");
                var tempIsDeleted = $(this).find('#aIsDeleted_' + tempProjectMasterID).is(":checked");
                var tempUDF1 = $(this).find('#aUDF1_' + tempProjectMasterID).val();
                var tempUDF2 = $(this).find('#aUDF2_' + tempProjectMasterID).val();
                var tempUDF3 = $(this).find('#aUDF3_' + tempProjectMasterID).val();
                var tempUDF4 = $(this).find('#aUDF4_' + tempProjectMasterID).val();
                var tempUDF5 = $(this).find('#aUDF5_' + tempProjectMasterID).val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempProjectMasterID).val();
                var tempItemDollarLimitAmount = $(this).find('#aItemDollarLimitAmount_' + tempProjectMasterID).val();
                var tempItemQuantityLimitAmount = $(this).find('#aItemQuantityLimitAmount_' + tempProjectMasterID).val();
                var tempIsLineItemDeleted = $(this).find('#aIsLineItemDeleted_' + tempProjectMasterID).is(":checked");




                var data = {
                    "ID": tempProjectMasterID, "ProjectSpendName": tempProjectSpendName, "Description": tempDescription,
                    "DollarLimitAmount": tempDollarLimitAmount, "TrackAllUsageAgainstThis": tempTrackAllUsageAgainstThis,
                    "IsClosed": tempIsClosed, "IsDeleted": tempIsDeleted, "UDF1": tempUDF1, "UDF2": tempUDF2,
                    "UDF3": tempUDF3, "UDF4": tempUDF4, "UDF5": tempUDF5, "ItemNumber": tempItemNumber,
                    "ItemDollarLimitAmount": tempItemDollarLimitAmount, "ItemQuantityLimitAmount": tempItemQuantityLimitAmount,
                    "IsLineItemDeleted": tempIsLineItemDeleted
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == WorkOrderval) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempWOID = $(this).find('#InvLocId').text();
                var tempWOName = $(this).find('#aWOName_' + tempWOID).val();
                var tempReleaseNumber = $(this).find('#aReleaseNumber_' + tempWOID).val();
                var tempDescription = $(this).find('#aDescription_' + tempWOID).val();
                var tempWOStatus = $(this).find('#aWOStatus_' + tempWOID).val();
                var tempTechnician = $(this).find('#aTechnician_' + tempWOID).val();
                var tempCustomer = $(this).find('#aCustomer_' + tempWOID).val();
                var tempWOType = $(this).find('#aWOType_' + tempWOID).val();
                var tempUDF1 = $(this).find('#aUDF1_' + tempWOID).val();
                var tempUDF2 = $(this).find('#aUDF2_' + tempWOID).val();
                var tempUDF3 = $(this).find('#aUDF3_' + tempWOID).val();
                var tempUDF4 = $(this).find('#aUDF4_' + tempWOID).val();
                var tempUDF5 = $(this).find('#aUDF5_' + tempWOID).val();





                var data = {
                    "ID": tempWOID, "WOName": tempWOName, "Description": tempDescription,
                    "WOStatus": tempWOStatus, "Technician": tempTechnician,
                    "Customer": tempCustomer, "WOType": tempWOType, "UDF1": tempUDF1, "UDF2": tempUDF2,
                    "UDF3": tempUDF3, "UDF4": tempUDF4, "UDF5": tempUDF5, "ReleaseNumber": tempReleaseNumber
                };
                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }
        else if (selectedModuleVal == PullImport) {
            $('#myImportDataTable tbody tr').each(function () {
                Records++;
                var tempPIID = $(this).find('#InvLocId').text();
                var tempPIItemNumber = $(this).find('#aItemNumber_' + tempPIID).val();
                var tempPIPullQuantity = $(this).find('#PullQuantity_' + tempPIID).val();
                var tempPILocationName = $(this).find('#aLocationName_' + tempPIID).val();
                var tempPIUDF1 = $(this).find('#aUDF1_' + tempPIID).val();
                var tempPIUDF2 = $(this).find('#aUDF2_' + tempPIID).val();
                var tempPIUDF3 = $(this).find('#aUDF3_' + tempPIID).val();
                var tempPIUDF4 = $(this).find('#aUDF4_' + tempPIID).val();
                var tempPIUDF5 = $(this).find('#aUDF5_' + tempPIID).val();
                var tempPIProjectSpendName = $(this).find('#aProjectSpendName_' + tempPIID).val();
                var tempPIPullOrderNumber = $(this).find('#aPullOrderNumber_' + tempPIID).val();
                var tempPIWorkOrder = $(this).find('#aWorkOrder_' + tempPIID).val();
                var tempPIActionType = $(this).find('#aActionType_' + tempPIID).val();
                
                var data = {
                    "ID": tempPIID, "ItemNumber": tempPIItemNumber, "PullQuantity": tempPIPullQuantity, "Location": tempPILocationName,
                    "UDF1": tempPIUDF1, "UDF2": tempPIUDF2, "UDF3": tempPIUDF3, "UDF4": tempPIUDF4, "UDF5": tempPIUDF5,
                    "ProjectSpendName": tempPIProjectSpendName, "PullOrderNumber": tempPIPullOrderNumber, "WorkOrder": tempPIWorkOrder, "ActionType": tempPIActionType
                };

                arrItems.push(data);
                if ((Records % 500) == 0) {
                    $.ajax({
                        url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {

                        }
                        , error: function (xhr, status, error) {


                        },
                        complete: function () {

                        }
                    });
                    arrItems = new Array();
                }
            });
        }

        var IDs = '';
        var IDsWithLink2 = '';

        $('#ImportProcessing').modal();
        $.ajax({
            url: SaveImportURL
                        , type: 'POST'
                        , data: { 'TableName': CurModuleval, 'para': JSON.stringify(arrItems) }
                        , cache: false
                        , timeout: 0
                        , success: function (response) {
                            oTable.fnDraw();
                            //$('div#target').fadeToggle();
                            IDs = response.savedOnlyitemIds;
                            IDsWithLink2 = response.savedItemIdsWithLink2;

                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime, function () {
                                CheckValidation(true, response.allSuccesfulRecords);
                                $('#btnSave').removeProp("disabled");
                                $('#DivLoading').hide();
                            });

                            $("input#uploadZIPFile").hide();
                            $("input#uploadZIPFileForLink2").hide();
                            $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color:green'>Records imported successfully.</b>");
                            closeImportInfoModel();
                            $('#ImportInfoDialog').modal();
                            //                            $("#spanGlobalMessage").text(response.Message);
                            //                            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        }
                        , error: function (xhr, status, error) {

                            var FinalErr = xhr.responseText;

                            //$('#ImportInfoDialog').find("#ImportMSG").html(FinalErr);
                            $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>Some error occured during import processing. All data is not saved successfully.</b>");
                            closeImportInfoModel();
                            $('#ImportInfoDialog').modal();
                            //                            $("#spanGlobalMessage").text(message.statusText);
                            //                            $('div#target').fadeToggle();
                            //                            $("div#target").delay(2000).fadeOut(200);
                            $('#btnSave').removeProp("disabled");
                            //$('#DivLoading').hide();
                        },
            complete: function () {
                if ($.trim($('#ddlModule option:selected').text()) == 'Items') {

                    if (document.getElementById("uploadZIPFile").value != '') {

                        uploadZIPFile(IDs);

                    }
                    if (document.getElementById("uploadZIPFileForLink2").value != '') {

                        uploadZIPFileLink2(IDsWithLink2);

                    }
                    
                }


            }
        });


    });


});


function CheckValidation(checkValidate, allSuccesfulRecords) {

    var allSuccessRecords = true;
    var selectedModuleVal = $('#ddlModule option:selected').attr('value');
    var lengthofTable = $('#myImportDataTable tbody').find('tr').length;
    if (lengthofTable > 0) {
        if (selectedModuleVal == BinMasterselval) {

        }
        else if (selectedModuleVal == LocationMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempLocation = $(this).find('#aLocation_' + id).val();
                if (tempLocation == "") {
                    allSuccessRecords = false;
                    $(this).find('#aLocation_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == ToolCategoryselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempToolCategory = $(this).find('#aToolCategory_' + id).val();
                if (tempToolCategory == "") {
                    allSuccessRecords = false;
                    $(this).find('#aToolCategory_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == CategoryMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempCategory = $(this).find('#aCategory_' + id).val();
                if (tempCategory == "") {
                    allSuccessRecords = false;
                    $(this).find('#aCategory_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == CustomerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempCustomer = $(this).find('#aCustomer_' + id).val();
                if (tempCustomer == "") {
                    allSuccessRecords = false;
                    $(this).find('#aCustomer_' + id).addClass("importtext-box validationerror");
                }
                var tempAccount = $(this).find('#aAccount_' + id).val();
                if (tempAccount == "") {
                    allSuccessRecords = false;
                    $(this).find('#aAccount_' + id).addClass("importtext-box validationerror");
                }
                var tempEmail = $(this).find('#aEmail_' + id).val();
                if (tempEmail != "") {

                    var pattern = /^\b[A-Z0-9._%-]+@@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
                    if (!pattern.test(tempEmail)) {
                        allSuccessRecords = false;
                        $(this).find('#aEmail_' + id).addClass("importtext-box validationerror");
                    }
                }
            });
        }
        else if (selectedModuleVal == FreightTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempFreightType = $(this).find('#aFreightType_' + id).val();
                if (tempFreightType == "") {
                    allSuccessRecords = false;
                    $(this).find('#aFreightType_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == GLAccountsMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempGLAccount = $(this).find('#aGLAccount_' + id).val();
                if (tempGLAccount == "") {
                    allSuccessRecords = false;
                    $(this).find('#aGLAccount_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == GXPRConsignedJobMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempGXPRConsinged = $(this).find('#aGXPRConsinged_' + id).val();
                if (tempGXPRConsinged == "") {
                    allSuccessRecords = false;
                    $(this).find('#aGXPRConsinged_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == JobTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempJobType = $(this).find('#aJobType_' + id).val();
                if (tempJobType == "") {
                    allSuccessRecords = false;
                    $(this).find('#aJobType_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == ShipViaMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempShipVia = $(this).find('#aShipVia_' + id).val();
                if (tempShipVia == "") {
                    allSuccessRecords = false;
                    $(this).find('#aShipVia_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == TechnicianMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempTechnician = $(this).find('#aTechnician_' + id).val();
                var tempTechnicianCode = $(this).find('#aTechnicianCode_' + id).val();
                //if (tempTechnician == "") {
                //    allSuccessRecords = false;
                //    $(this).find('#aTechnician_' + id).addClass("importtext-box validationerror");
                //}
                if (tempTechnicianCode == "") {
                    allSuccessRecords = false;
                    $(this).find('#aTechnicianCode_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == ManufacturerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempManufacturer = $(this).find('#aManufacturer_' + id).val();
                if (tempManufacturer == "") {
                    allSuccessRecords = false;
                    $(this).find('#aManufacturer_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == MeasurementTermMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempMeasurementTerm = $(this).find('#aMeasurementTerm_' + id).val();
                if (tempMeasurementTerm == "") {
                    allSuccessRecords = false;
                    $(this).find('#aMeasurementTerm_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == UnitMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempUnit = $(this).find('#aUnit_' + id).val();
                if (tempUnit == "") {
                    allSuccessRecords = false;
                    $(this).find('#aUnit_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == SupplierMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {

                var id = $(this).attr('id');
                var tempSupplier = $(this).find('#aSupplier_' + id).val();

                if (tempSupplier == "") {
                    allSuccessRecords = false;
                    $(this).find('#aSupplier_' + id).addClass("importtext-box validationerror");
                }
                //wi-1266
                //                var tempEmail = $(this).find('#aEmail_' + id).val();
                //                if (tempEmail != "") {
                //                    var pattern = /^\b[A-Z0-9._%-]+@@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
                //                    if (!pattern.test(tempEmail)) {
                //                        allSuccessRecords = false;
                //                        $(this).find('#aEmail_' + id).addClass("importtext-box validationerror");
                //                    }
                //                }
            });
        }
        else if (selectedModuleVal == ItemMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {

                var id = $(this).attr('id');
                var tempItemNumber = $(this).find('#aItemNumber_' + id).val();
                var tempUnit = $(this).find("#aUnit_" + id).val();
                if (tempItemNumber == "") {
                    allSuccessRecords = false;
                    $(this).find('#aItemNumber_' + id).addClass("importtext-box validationerror");
                }
                if (tempUnit == "") {
                    allSuccessRecords = false;
                    $(this).find('#aUnit_' + id).addClass("importtext-box validationerror");
                }
            });
        }
        else if (selectedModuleVal == BarcodeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var tempItemNumber = $(this).find('#aItemNumber_' + id).val();
                //var tempUnit = $(this).find("#aUnit_" + id).val();
                if (tempItemNumber == "") {
                    allSuccessRecords = false;
                    $(this).find('#aItemNumber_' + id).addClass("importtext-box validationerror");
                }

            });
        }
        //else if (selectedModuleVal == ItemManufacturerselval) {
        //    $('#myImportDataTable tbody tr').each(function () {
        //        var id = $(this).attr('id');
        //        var tempItemNumber = $(this).find('#aItemNumber_' + id).val();
        //        var tempManufactureName = $(this).find("#aManufacturerName_" + id).val();
        //        var tempManufactureNumber = $(this).find("#aManufacturerNumber_" + id).val();
        //        var tempIsDefault = $(this).find("#aIsDefault_" + id).is(":checked");
        //        if (tempItemNumber == "") {
        //            allSuccessRecords = false;
        //            $(this).find('#aItemNumber_' + id).addClass("importtext-box validationerror");
        //        }
        //        if (tempManufactureName == "") {
        //            allSuccessRecords = false;
        //            $(this).find('#aManufacturerName_' + id).addClass("importtext-box validationerror");
        //        }
        //    });
        //}
    }

    if (checkValidate == true && allSuccessRecords == true && allSuccesfulRecords == true) {
        $(function () {
            $.post('/Import/ResetSession',
                   { key: "CurModulevalue", value: AssetMasterval }, function (data) {

                       $('#myImportDataTable').html('');

                       $("select#ddlModule")[0].selectedIndex = 0;
                       $("select#ddlModule").attr('selected', true);

                   });
        });
    }


}
function checkBlankValidation(ctrlName) {
    if (ctrlName.value == "") {
        $(ctrlName).addClass("validationerror");
        $(ctrlName).css('border-color', 'red')
    }
    else {
        $(ctrlName).removeClass("validationerror");
        $(ctrlName).css('border-color', '#DDD');
    }


}
function CheckLotSerial(Curobj) {
    if ($(Curobj).prop("checked") == true) {
        if ($(Curobj).prop("id").indexOf("SerialNumberTracking") > 0) {
            $(Curobj).parent().parent().find("input[type='checkbox'][id*='LotNumberTracking']").prop("checked", false);
        }
        else if ($(Curobj).prop("id").indexOf("LotNumberTracking") > 0) {
            $(Curobj).parent().parent().find("input[type='checkbox'][id*='SerialNumberTracking']").prop("checked", false);
        }
    }
}
function CheckEmailValidataion(ctrlName) {
    if (ctrlName.value != "") {
        var pattern = /^\b[A-Z0-9._%-]+@@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
        if (!pattern.test(ctrlName.value))
            $(ctrlName).addClass("validationerror");
        else
            $(ctrlName).removeClass("validationerror");
    }
}
function isNumberKey(evt) {


    var keyCode = [evt.keyCode || evt.which];

    if (keyCode == 8 || keyCode == 13 || keyCode == 9)
        return true;
    if (keyCode > 57 || keyCode < 48)
        return false;
    else
        return true;
}
function CheckFileName() {
    var fileName = document.getElementById("uploadFile").value;
    if (fileName == "") {
        alert(ImportBrowseFileMsg);
        return false;
    }
    var strfilename = fileName.split(".");
    if (strfilename[strfilename.length - 1].toUpperCase() == "CSV" || strfilename[strfilename.length - 1].toUpperCase() == "XLS")
        return true;
    else {
        alert("File with " + fileName + " isinvalid. Upload a validfile with csv extensions");
        return false;
    }
    return true;
}
function CheckZIPFileName() {
    if ($.trim($('#ddlModule option:selected').text()) == 'Items') {
        var fileName = document.getElementById("uploadZIPFile").value;
        if (fileName == "") {
            return true;
        }
        var strfilename = fileName.split(".");
        if (strfilename[strfilename.length - 1].toUpperCase() == "ZIP")
            return true;
        else {
            alert("File with " + fileName + " isinvalid. Upload a validfile with zip extensions");
            document.getElementById("uploadZIPFile").value = '';
            return false;
        }
    }
    return true;
}



function ValidateCriticalMinMax() {
    var checkval = false;
    var i = 1;
    $('#myImportDataTable tbody tr').each(function () {
        var strmsg = '';
        var tempItemID = $(this).find('#ItemID').text();
        var CriticalQuantity = $(this).find('#aCriticalQuantity_' + tempItemID).val();
        var MinimumQuantity = $(this).find('#aMinimumQuantity_' + tempItemID).val();
        var MaximumQuantity = $(this).find('#aMaximumQuantity_' + tempItemID).val();
        if (CriticalQuantity == null || CriticalQuantity == '') {
            CriticalQuantity = 0;
        }
        if (MinimumQuantity == null || MinimumQuantity == '') {
            MinimumQuantity = 0;
        }
        if (MaximumQuantity == null || MaximumQuantity == '') {
            MaximumQuantity = 0;
        }
        if ((parseFloat(CriticalQuantity) <= parseFloat(MinimumQuantity))) {
            $(this).find('#aCriticalQuantity_' + tempItemID).css('border-color', '#DDD');
            $(this).find('#aMinimumQuantity_' + tempItemID).css('border-color', '#DDD');

        }
        else {
            $(this).find('#aCriticalQuantity_' + tempItemID).css('border-color', 'red');
            $(this).find('#aMinimumQuantity_' + tempItemID).css('border-color', 'red');

            checkval = true;

            strmsg = minmaxmsg;
        }
        if ((parseFloat(MinimumQuantity) <= parseFloat(MaximumQuantity))) {

            $(this).find('#aMinimumQuantity_' + tempItemID).css('border-color', '#DDD');
            $(this).find('#aMaximumQuantity_' + tempItemID).css('border-color', '#DDD');
        }
        else {

            $(this).find('#aMinimumQuantity_' + tempItemID).css('border-color', 'red');
            $(this).find('#aMaximumQuantity_' + tempItemID).css('border-color', 'red');
            checkval = true;
            if (strmsg == '') {
                strmsg = minmaxmsg1;
            }
            else {
                strmsg = strmsg + ' ' + minmaxmsg2;
            }

        }

        if (strmsg != '') {
            if (ErrorLineNumber != '') {
                ErrorLineNumber += "," + i;
            }
            else {
                ErrorLineNumber = i;
            }
            $(this).find('#Reason').html(strmsg);
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');
            $(this).css("background-color", "pink");
        }
        else {
            if ($(this).find('#Status').html() != 'Success') {
                $(this).find('#Reason').html('N/A');
                $(this).find('#Status').html('N/A');
                $(this).find('#Status').css('color', '#000000');
                $(this).find('#Reason').css('color', '#000000');
            }
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }
        i++;
    });

    // FillRowColor()

    if (checkval == true) {
        return false;

    }
    else {
        return true;
    }
}
function isUniqueSerial() {
    // Collect all values in an array
    var values = [];
    //$('#myImportDataTable tbody tr td input[id^="aSerial_"]').each(function (idx, val) { values.push($(val).val()); });
    
    $('#myImportDataTable tbody tr').each(function (idx, val1) {

        values.push(
            $(val1).find('input[id^="aSerial_"]').val() + '###' +
            $(val1).find('input[id^="aToolName_"]').val()
           // $(val1).find('input[id^="aTechnician_"]').val()


            );
    });

    // Sort it
    values.sort();

    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {

        var prevSerial = values[k].split('###')[0];
        var currentSerial = values[k - 1].split('###')[0];
        var prevTool = values[k].split('###')[1];
        var currentTool = values[k - 1].split('###')[1];

        if (prevSerial == currentSerial && prevTool != currentTool) {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $(this).find('input[id^="aSerial_"]').val();

                if (existing.indexOf(value) >= 0) {
                    if (ErrorLineNumber != '') {
                        ErrorLineNumber += "," + i;
                    }
                    else {
                        ErrorLineNumber = i;
                    }
                    $(this).closest('tr').css('background-color', 'pink');
                    $(this).find('input[id^="aSerial_"]').css('border-color', 'red');
                    $(this).find('#Reason').html('Please enter unique serial value');
                    $(this).find('#Status').html('Fail');
                }
                else {
                    $(this).find('input[id^="aSerial_"]').css('border-color', '#DDD');
                    if ($(this).attr("class") == 'even') {
                        $(this).css("background-color", "#FFF");
                    }
                    else {
                        $(this).css("background-color", "#EFF7FF");
                    }
                }
                i++;
                existing.push(value);
            });

            return false;
        }
    }
    return true;
}
function isUniqueQuickListBin() {
    // Collect all values in an array
    var values = [];
    //$('#myImportDataTable tbody tr td input[id^="aSerial_"]').each(function (idx, val) { values.push($(val).val()); });

    $('#myImportDataTable tbody tr').each(function (idx, val1) {
        // if ($(val1).find('input[id^="aBinNumber_"]').val() != '') {
        values.push(
            $(val1).find('input[id^="aQuickListname_"]').val() + '###' +
            $(val1).find('input[id^="aItemNumber_"]').val() + '###' +
            $(val1).find('input[id^="aBinNumber_"]').val()
           // $(val1).find('input[id^="aTechnician_"]').val()


            );
        //  }
    });

    // Sort it
    values.sort();

    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {
        
        var prevQuickList = values[k].split('###')[0];
        var currentQuickList = values[k - 1].split('###')[0];
        var prevItemNumber = values[k].split('###')[1];
        var currentItemNumber = values[k - 1].split('###')[1];
        var prevBinNumber = values[k].split('###')[2];
        var currentBinNumber = values[k - 1].split('###')[2];
        //alert("q"+prevQuickList);
        //alert("q" + currentQuickList);
        //alert("i"+prevItemNumber);
        //alert("i" + currentItemNumber);
        //alert("p" + prevBinNumber);
        //alert("p" + currentBinNumber);

        if (prevQuickList == currentQuickList && prevItemNumber == currentItemNumber && prevBinNumber == currentBinNumber) {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $(this).find('input[id^="aQuickListname_"]').val() + '###' + $(this).find('input[id^="aItemNumber_"]').val() + "###" + $(this).find('input[id^="aBinNumber_"]').val();

                if (existing.indexOf(value) >= 0) {
                    if (ErrorLineNumber != '') {
                        ErrorLineNumber += "," + i;
                    }
                    else {
                        ErrorLineNumber = i;
                    }
                    $(this).closest('tr').css('background-color', 'pink');
                    $(this).find('input[id^="aBinNumber_"]').css('border-color', 'red');
                    $(this).find('#Reason').html('Please enter Unique BinNumber');
                    $(this).find('#Status').html('Fail');
                }
                else {
                    $(this).find('input[id^="aBinNumber_"]').css('border-color', '#DDD');
                    if ($(this).attr("class") == 'even') {
                        $(this).css("background-color", "#FFF");
                    }
                    else {
                        $(this).css("background-color", "#EFF7FF");
                    }
                }
                i++;
                existing.push(value);
            });

            return false;
        }
    }
    return true;
}
function isUniqueSupplierPartNumber() {
    // Collect all values in an array
    var values = [];
    $('#myImportDataTable tbody tr td input[id^="aSupplierNumber_"]').each(function (idx, val) { values.push($(val).val()); });

    // Sort it
    values.sort();

    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {
        if (values[k] == values[k - 1]) {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $(this).find('input[id^="aSupplierNumber_"]').val();

                if (existing.indexOf(value) >= 0) {
                    if (ErrorLineNumber != '') {
                        ErrorLineNumber += "," + i;
                    }
                    else {
                        ErrorLineNumber = i;
                    }
                    $(this).closest('tr').css('background-color', 'pink');
                    $(this).find('input[id^="aSupplierNumber_"]').css('border-color', 'red');
                    $(this).find('#Reason').html('Please enter unique Supplier Number');
                    $(this).find('#Status').html('Fail');
                }
                else {
                    $(this).find('input[id^="aSupplierNumber_"]').css('border-color', '#DDD');
                    if ($(this).attr("class") == 'even') {
                        $(this).css("background-color", "#FFF");
                    }
                    else {
                        $(this).css("background-color", "#EFF7FF");
                    }
                }
                i++;
                existing.push(value);
            });

            return false;
        }
    }
    return true;
}
function checkModuleName() {
    // Collect all values in an array
    var values = [];
    var checkval = false;
    var strmsg = '';
    var i = 1;
    $('#myImportDataTable tbody tr').each(function () {
        strmsg = '';
        var tempBarcodeID = $(this).find("#BarcodeID").text()
        var tempaModule = $(this).find('#aModuleName_' + tempBarcodeID).val();

        if ($.trim(tempaModule).toLowerCase() == "assets" || $.trim(tempaModule).toLowerCase() == "item master") {

            strmsg = '';
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
            $(this).find('#Status').css('color', '#000000');
            $(this).find('#Reason').css('color', '#000000');
        }
        else {
            checkval = true;
            strmsg = 'ModuleName should be Assets or Item Master';
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');

        }

        if (strmsg != '') {
            if (ErrorLineNumber != '') {
                ErrorLineNumber += "," + i;
            }
            else {
                ErrorLineNumber = i;
            }
            $(this).find('#Reason').html(strmsg);
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');
            $(this).css("background-color", "pink");
        }
        else {
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }
        i++;

    });
    if (checkval == true) {
        return false;

    }
    else {
        return true;
    }
}

function ValidateSerialNumber(toolNameBlankSerial) {
    var checkval = false;
    $('#myImportDataTable tbody tr').each(function () {
        var strmsg = '';
        var tempToolID = $(this).find('#ToolID').text();
        var tempToolName = $(this).find('#aToolName_' + tempToolID).val();

        var tempSerial = $(this).find('#aSerial_' + tempToolID).val();

        if (tempSerial == null || tempSerial == '') {
            if (toolNameBlankSerial != '') {
                if (tempToolName != toolNameBlankSerial) {
                    $(this).find('#aSerial_' + tempToolID).css('border-color', 'red');
                    checkval = true;
                    strmsg = "ToolName " + toolNameBlankSerial + " is already contains blank serial. Please insert serial.";
                }
                else {
                    strmsg = '';
                    if ($(this).attr("class") == 'even') {
                        $(this).css("background-color", "#FFF");
                    }
                    else {
                        $(this).css("background-color", "#EFF7FF");
                    }
                    $(this).find('#Status').css('color', '#000000');
                    $(this).find('#Reason').css('color', '#000000');
                }
            }
            else {
                toolNameBlankSerial = tempToolName;
            }
        }


        if (strmsg != '') {
            $(this).find('#Reason').html(strmsg);
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');
            $(this).css("background-color", "pink");
        }
        else {
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }
    });

    // FillRowColor()

    if (checkval == true) {
        return false;

    }
    else {
        return true;
    }
}
function FillRowColor() {

    $('#myImportDataTable tbody tr').each(function () {

        var tempStatus = $(this).find('#Status').text();

        if (tempStatus == "Fail") {

            $(this).css("background-color", "pink");

        }
        else {

            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }

        }


    });
}
function ValidateImageName() {
    var checkval = false;
    $('#myImportDataTable tbody tr').each(function () {
        var strmsg = '';
        var tempItemID = $(this).find('#ItemID').text();

        var imagefilename = $(this).find('#aImagePath_' + tempItemID).val();

        if (imagefilename.toString().indexOf("&") >= 0 || imagefilename.toString().indexOf("<") >= 0 || imagefilename.toString().indexOf(">") >= 0
                || imagefilename.toString().indexOf("*") >= 0 || imagefilename.toString().indexOf(":") >= 0
                || imagefilename.toString().indexOf("?") >= 0) {
            $(this).find('#aImagePath_' + tempItemID).css('border-color', '#DDD');
            if (strmsg == '') {
                strmsg = ImagePathProper;
            }
            else {
                strmsg = strmsg + ' ' + ImagePathProper;
            }
            checkval = true;
        }
        if (strmsg != '') {

            $(this).find('#Reason').html(strmsg);
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');
            $(this).css("background-color", "pink");
        }
        else {
            if ($(this).find('#Status').html() != 'Success') {
                $(this).find('#Reason').html('N/A');
                $(this).find('#Status').html('N/A');
                $(this).find('#Status').css('color', '#000000');
                $(this).find('#Reason').css('color', '#000000');
            }
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }
    });
    if (checkval == true) {
        return false;

    }
    else {
        return true;
    }
}
function CheckDoubleValue(val) {
    var pattern = /^\d+.?\d*$/;

    if (val.match(pattern) == null)
        return false;
    else
        return true;

}

function CheckValidURLForImage(strURL) {
    if (strURL != '' && strURL != null) {
        $("<img>", {
            src: strURL,
            error: function () {
                return false;
            },
            load: function () {
                return true;
            }
        });
    }
    else {
        return true;
    }
}


function is_valid_url(url) {
    if (url != '' && url != null) {
        var IsValidURL = /^(http(s)?:\/\/)?(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/.test(url);
        if (IsValidURL == false) {
            return false;
        }
        else {
            return true;
        }
    }
}

function CheckIntegerValue(val) {
    var pattern = /^[0-9]+$/;

    if (val.match(pattern) == null)
        return false;
    else
        return true;

}

function uploadZIPFile(IDs) {
    $("#loading")
.ajaxStart(function () {
    $(this).show();
})
.ajaxComplete(function () {
    $(this).hide();
});

    $.ajaxFileUpload
(
    {
        url: '/api/fileupload/uploadZIPFILE',
        secureuri: false,
        type: "POST",

        fileElementId: 'uploadZIPFile',
        dataType: 'json',
        success: function (data, status) {
            //window.location.reload();
        },
        error: function (xhr) {

        }
    }
)
    return false;
}

function uploadZIPFileLink2(IDsWithLink2) {
    $("#loading")
.ajaxStart(function () {
    $(this).show();
})
.ajaxComplete(function () {
    $(this).hide();
});
  
    $.ajaxFileUpload
(
    {
        url: '/api/fileupload/uploadZIPFileLink2',
        secureuri: false,
        type: "POST",
        data:{'IDsWithLink2':IDsWithLink2},
        fileElementId: 'uploadZIPFileForLink2',
        dataType: 'json',
        success: function (data, status) {
            //window.location.reload();
        },
        error: function (xhr) {

        }
    }
)
    return false;
}

function parseMDY(txtDate) {


    var currVal = txtDate;

    if (currVal == '')

        return false;
    //Declare Regex

    var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;

    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)

        return false;

    //Checks for mm/dd/yyyy format.

    dtMonth = dtArray[1];

    dtDay = dtArray[3];

    dtYear = dtArray[5];
    if (dtMonth < 1 || dtMonth > 12)

        return false;

    else if (dtDay < 1 || dtDay > 31)

        return false;

    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)

        return false;

    else if (dtMonth == 2) {

        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));

        if (dtDay > 29 || (dtDay == 29 && !isleap))

            return false;

    }

    return true;

}
function ValidatePurchaseDate() {
    var checkval = false;
    $('#myImportDataTable tbody tr').each(function () {
        var strmsg = ''
        var tempAssetID = $(this).find('#AssetID').text();
        var tempPurchaseDate = $(this).find('#aPurchaseDate_' + tempAssetID).val();
        if (tempPurchaseDate != '') {

            if (parseMDY(tempPurchaseDate)) {
                $(this).find('#aPurchaseDate_' + tempAssetID).css('border-color', '#DDD');
            }
            else {
                checkval = true;
                $(this).find('#aPurchaseDate_' + tempAssetID).css('border-color', 'red');

                strmsg = 'Purchase Date should be in mm/dd/yyyy format.';
            }
        }
        if (strmsg != '') {

            $(this).find('#Reason').html(strmsg);
            $(this).find('#Status').html('Fail');
            $(this).find('#Status').css('color', 'red');
            $(this).find('#Reason').css('color', 'red');
            $(this).css("background-color", "pink");
        }
        else {
            if ($(this).find('#Status').html() != 'Success') {
                $(this).find('#Reason').html('N/A');
                $(this).find('#Status').html('N/A');
                $(this).find('#Status').css('color', '#000000');
                $(this).find('#Reason').css('color', '#000000');
            }
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }

    });
    if (checkval == true) {
        return false;

    }
    else {
        return true;
    }
}
$('#btnCancel').click(function () {

    $.ajax({
        url: ClearCurrentResourceListURL
                    , type: 'POST'
                    , cache: false
                    , success: function (response) {
                        oTable.fnDraw();
                        $("#divbtnBlock").hide();
                    }
                    , error: function (message) {

                    }
    });


});
$('#ddlModule').change(function () {

    var selectedText = $('#ddlModule option:selected').attr('text');
    var selectedVal = $('#ddlModule option:selected').attr('value');

    if (selectedVal != "") {
        $.ajax({
            url: SetSelectedModuleURL,
            type: 'POST',
            dataType: 'json',
            data: "{CurModule:'" + selectedVal + "' }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {

            },
            error: function (request, status, error) {

            }
        });
    }
});
function closeImportInfoModel() {
    $.modal.impl.close();
}

function closeFromOk() {
    $.modal.impl.close();
}

function closeErrorDialog() {
    $.modal.impl.close();
    return false;
}



function isValidEmailAddress(emailAddress) {

    //            var pattern = /^\b[A-Z0-9._%-]+[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
    //            return pattern.test(emailAddress);
};

function JSONDate(dateStr) {
    var m, day;
    jsonDate = dateStr;
    var d = new Date(parseInt(jsonDate.substr(6)));
    m = d.getMonth() + 1;
    if (m < 10)
        m = '0' + m
    if (d.getDate() < 10)
        day = '0' + d.getDate()
    else
        day = d.getDate();
    return (m + '/' + day + '/' + d.getFullYear())
}
function GenerateDataTable() {

    $("#divbtnBlock").show();

    var IsRefreshGrid = false;
    var IsDeletePopupOpen = false;
    var AllowDeletePopup = true;
    var SelectedHistoryRecordID = 0;
    $(document).ready(function () {
        $('#DivGridLoading').show();
        // alert(ColumnObject);
        var gaiSelected = [];

        oTable = $('#myImportDataTable').dataTable({
            "bJQueryUI": true,
            "bScrollCollapse": true,
            "sScrollX": "90%",
            "sDom": 'rti<"clear">>T',
            "oColVis": {},
            //"aaSorting": [[1, "asc"]],
            "bSort": false,
            "oColReorder": {},
            "sPaginationType": "full_numbers",
            "bProcessing": false,
            "bStateSave": true,
            "oLanguage": {
                "sLengthMenu": gridlbl,
                "sEmptyTable": NoDataAvailableInTable,
                "sInfo": ShowingNoOfEntries,
                "sInfoEmpty": ShowingZeroEntries
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                if (aData.IsDeleted == true && aData.IsArchived == true)
                    nRow.className = "GridDeleatedArchivedRow";
                else if (aData.IsDeleted == true)
                    nRow.className = "GridDeletedRow";
                else if (aData.IsArchived == true)
                    nRow.className = "GridArchivedRow";
                return nRow;
            },
            "fnStateSaveParams": function (oSettings, oData) {
                oData.oSearch.sSearch = "";
                $.ajax({
                    "url": SaveGridStateURL,
                    "type": "POST",
                    data: { Data: JSON.stringify(oData), ListName: 'ImportList' },
                    "async": false,
                    "dataType": "json",
                    "success": function (json) {
                        o = json;
                    }
                });
            },
            "fnStateLoad": function (oSettings) {
                var o;
                $.ajax({
                    "url": LoadGridStateURL,
                    "type": "POST",
                    data: { ListName: 'ImportList' },
                    "async": false,
                    cache: false,
                    "dataType": "json",
                    "success": function (json) {
                        if (json.jsonData != '')
                            o = JSON.parse(json.jsonData);
                    }
                });

                return o;
            },
            "bServerSide": true,
            "sAjaxSource": GetImportListURL,
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
                var arrCols = new Array();
                var objCols = this.fnSettings().aoColumns;
                for (var i = 0; i <= objCols.length - 1; i++) {
                    arrCols.push(objCols[i].mDataProp);
                }
                for (var j = 0; j <= aoData.length - 1; j++) {
                    if (aoData[j].name == "sColumns") {
                        aoData[j].value = arrCols.join("|");
                        break;
                    }
                }
                if (oSettings.aaSorting.length != 0)
                    aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
                else
                    aoData.push({ "name": "SortingField", "value": "0" });

                aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
                aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });

                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    cache: false,
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback,
                    beforeSend: function () {
                    },
                    complete: function () {
                        //ValidateValueWithDataType();
                        $('#DivGridLoading').hide();
                    }
                })
            },
            "fnInitComplete": function () {
                $('.ColVis').detach().appendTo(".setting-arrow");
                CheckValidation(false, false);
            },
            "aoColumns": ColumnObject
        }).makeEditable({
            sUpdateURL: UpdateURL
        });

        //HIDE PRINT CONTAINER
        $('.DTTT_container').css('z-index', '-1');

        $('#myImportDataTable tbody tr').click(function (e) {
            $(this).removeClass('row_selected');
        });
        setTimeout(function () {

            FillRowColor();

        }, 500);

    });

}
function CheckDateFormat(objval) {
    //alert(objval);
    var retval = true;
    //alert(CheckDateBYCompanyConfig(objval));
    //            if (GetDateInYYYYMMDDFormat(objval) == "") {
    //                retval = false;
    //            }
    return retval;
}
function isUniqueSerialQty() {
    // Collect all values in an array
    var values = [];
    //$('#myImportDataTable tbody tr td input[id^="aSerial_"]').each(function (idx, val) { values.push($(val).val()); });
    
    $('#myImportDataTable tbody tr').each(function (idx, val1) {
        // if ($(val1).find('input[id^="aBinNumber_"]').val() != '') {
        
            values.push(
                $(val1).find('input[id^="aItemNumber_"]').val() + '###' +
                $(val1).find('input[id^="aSerialNumber_"]').val()
               // $(val1).find('input[id^="aTechnician_"]').val()
                );
        //  }
    });
    // Sort it
    values.sort();
    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {
        var prevItemNumber = values[k].split('###')[0];
        var currentItemNumber = values[k - 1].split('###')[0];
        var prevSerial = values[k].split('###')[1];
        var currentSerial = values[k - 1].split('###')[1];
  

        if (prevSerial == currentSerial && prevItemNumber == currentItemNumber && prevSerial != '' && currentSerial != '') {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $(this).find('input[id^="aItemNumber_"]').val() + '###' + $(this).find('input[id^="aSerialNumber_"]').val() ;

                if (existing.indexOf(value) >= 0) {
                    if (ErrorLineNumber != '') {
                        ErrorLineNumber += "," + i;
                    }
                    else {
                        ErrorLineNumber = i;
                    }
                    $(this).closest('tr').css('background-color', 'pink');
                    $(this).find('input[id^="aSerialNumber_"]').css('border-color', 'red');
                    $(this).find('#Reason').html('Please enter Unique Serial');
                    $(this).find('#Status').html('Fail');
                }
                else {
                    $(this).find('input[id^="aBinNumber_"]').css('border-color', '#DDD');
                    if ($(this).attr("class") == 'even') {
                        $(this).css("background-color", "#FFF");
                    }
                    else {
                        $(this).css("background-color", "#EFF7FF");
                    }
                }
                existing.push(value);
                i++;
            });

            return false;
        }
    }
    return true;
}