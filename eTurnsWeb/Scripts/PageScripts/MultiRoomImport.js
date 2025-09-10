var ErrorLineNumber = '';
var TotalCount = '0';
$(document).ready(function () {
    if (GenerateTableCall) {
        GenerateDataTable();
    }
    $("select[id='ddlModule']").find("option[value='-995']").html(ImportModuleReceiptAddNewRecipt);
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
        else if (selectedModuleVal == OrderMasterselval) {
            ModuleName = 'OrderMaster';
        }
        else if (selectedModuleVal == MoveMaterialval) {
            ModuleName = 'MoveMaterial';
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
        else if (selectedModuleVal == PullImportWithLotSerialVal) {
            ModuleName = 'PullImportWithLotSerial';
        }
        if (selectedModuleVal == ItemLocationChangeImport) {
            ModuleName = 'ItemLocationChangeImport';
        }
        else if (selectedModuleVal == PullImportWithSameQty) {
            ModuleName = 'PullImportWithSameQty';
        }
        else if (selectedModuleVal == AssetToolScheduler) {
            ModuleName = 'AssetToolScheduler';
        }
        else if (selectedModuleVal == AssetToolSchedulerMappingval) {
            ModuleName = 'AssetToolSchedulerMapping';
        }
        else if (selectedModuleVal == PastMaintenanceDue) {
            ModuleName = 'PastMaintenanceDue';
        }
        else if (selectedModuleVal == ToolAdjustmentCountval) {
            ModuleName = "ToolAdjustmentCount";
        }
        else if (selectedModuleVal == EnterpriseQuickListval) {
            ModuleName = 'EnterpriseQuickList';
        }
        else if (selectedModuleVal == Requisitionval) {
            ModuleName = 'Requisition';
        }
        else if (selectedModuleVal == QuoteMasterselval) {
            ModuleName = 'QuoteMaster';
        }
        else if (selectedModuleVal == SupplierCatalogVal) {
            ModuleName = 'SupplierCatalog';
        }
        else if (selectedModuleVal == ReturnOrderimportval) {
            ModuleName = 'ReturnOrder';
        }
        else if (selectedModuleVal == EditItemMasterselval) {
            ModuleName = 'SupplierPNList';
        }
       
        
        if (ModuleName == 'PullImport' || ModuleName == 'PullImportWithLotSerial' || ModuleName == 'PullImportWithSameQty') {
            ShowNotificationMessage(MsgDownloadDataNotAvailable)
            //alert(MsgDownloadDataNotAvailable);
            return false;
        } else {
            ExportAllData(ModuleName, 'CSV', 'import');
        }
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
            if (AllowToolOrderingval == 'True') {
                window.open(PathToCSv + "ToolsNew.csv?q=et&_" + randomNumber);
            }
            else {
                window.open(PathToCSv + "Tools.csv?q=et&_" + randomNumber);
            }
        }
        else if (selectedModuleVal == AssetToolSchedulerMappingval) {
            window.open(PathToCSv + "AssetToolSchedulerMapping.csv?q=et&_" + randomNumber);
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
        else if (selectedModuleVal == PullImportWithLotSerialVal) {
            window.open(PathToCSv + "PullImportWithLotSerial.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ItemLocationChangeImport) {
            window.open(PathToCSv + "ItemLocationChangeImport.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == PullImportWithSameQty) {
            window.open(PathToCSv + "PullImportWithSameQty.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == AssetToolScheduler) {
            window.open(PathToCSv + "AssetToolScheduler.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == PastMaintenanceDue) {
            window.open(PathToCSv + "PastMaintenanceDue.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ToolCheckInCheckOut) {
            if (isAllowToolOrdering.toLowerCase() == "true") {
                window.open(PathToCSv + "ToolCheckInCheckOutNew.csv?q=et&_" + randomNumber);
            }
            else {
                window.open(PathToCSv + "ToolCheckInCheckOut.csv?q=et&_" + randomNumber);
            }
        }
        else if (selectedModuleVal == ToolAdjustmentCountval) {
            window.open(PathToCSv + "ToolAdjustmentCount.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ToolCertificationImagesval) {
            window.open(PathToCSv + "ToolCertificationImages.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == OrderMasterselval) {
            window.open(PathToCSv + "Order.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == MoveMaterialval) {
            window.open(PathToCSv + "MoveMaterial.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == EnterpriseQuickListval) {
            window.open(PathToCSv + "EnterpriseQuickList.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == Requisitionval) {
            window.open(PathToCSv + "Requisition.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == QuoteMasterselval) {
            window.open(PathToCSv + "Quote.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == SupplierCatalogVal) {
            window.open(PathToCSv + "SupplierCatalog.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == ReturnOrderimportval) {
            window.open(PathToCSv + "ReturnOrder.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == EditItemMasterselval) {
            window.open(PathToCSv + "Item.csv?q=et&_" + randomNumber);
        }
        else if (selectedModuleVal == CommonBOMToItemimportval) {
            window.open(PathToCSv + "BOMItemToItem.csv?q=et&_" + randomNumber);
        }
    });



    $('#btnSave').click(function () {
        ErrorLineNumber = '';

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
        var isLoadAllItems = false;
        if (selectedModuleVal == BinMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $.trim($(this).find('#aSerialNumber_' + tempINVLocID).val());
                var tempLotNumber = $.trim($(this).find('#aLotNumber_' + tempINVLocID).val());
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                var tempItemDescription = $(this).find('#aItemDescription_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();
                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": $.trim(tempSerialNumber), "LotNumber": $.trim(tempLotNumber), "Expiration": tempExpiration,
                    "Cost": tempaCost, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, "ItemDescription": tempItemDescription
                };
                arrItems.push(data);
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
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $.trim($(this).find('#aSerialNumber_' + tempINVLocID).val());
                var tempLotNumber = $.trim($(this).find('#aLotNumber_' + tempINVLocID).val());
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                var tempReceived = $(this).find('#aReceived_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": $.trim(tempSerialNumber), "LotNumber": $.trim(tempLotNumber), "Expiration": tempExpiration,
                    "Cost": tempaCost, "Received": tempReceived
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == ManualCountval) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $.trim($(this).find('#aSerialNumber_' + tempINVLocID).val());
                var tempLotNumber = $.trim($(this).find('#aLotNumber_' + tempINVLocID).val());
                var tempExpiration = $(this).find('#aExpiration_' + tempINVLocID).val();
                var tempReceived = $(this).find('#aReceived_' + tempINVLocID).val();
                var tempaCost = $(this).find('#acost_' + tempINVLocID).val();
                var tempProjectSpend = $(this).find('#aProjectSpend_' + tempINVLocID).val();
                var tempItemDescription = $(this).find('#aItemDescription_' + tempINVLocID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "customerownedquantity": tempcustomerownedQTY, "consignedquantity": tempconsignedQTY,
                    "SerialNumber": $.trim(tempSerialNumber), "LotNumber": $.trim(tempLotNumber), "Expiration": tempExpiration,
                    "Cost": tempaCost, "Received": tempReceived,
                    "ProjectSpend": tempProjectSpend, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, "ItemDescription": tempItemDescription
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == eVMISetupselval) {
            $('#myImportDataTable tbody tr').each(function () {
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
                var tempisstaginglocation = $(this).find('#aisstaginglocation_' + tempINVLocID).is(':checked');
                var tempisedpq = $(this).find('#aisenforcedefaultpullquantity_' + tempINVLocID).is(':checked');
                var tempisedrq = $(this).find('#aisenforcedefaultreorderquantity_' + tempINVLocID).is(':checked');
                var tempdpq = $(this).find('#adefaultpullquantity_' + tempINVLocID).val();
                var tempdrq = $(this).find('#adefaultreorderquantity_' + tempINVLocID).val();
                var tempbinudf1 = $(this).find('#aBinUDF1_' + tempINVLocID).val();
                var tempbinudf2 = $(this).find('#aBinUDF2_' + tempINVLocID).val();
                var tempbinudf3 = $(this).find('#aBinUDF3_' + tempINVLocID).val();
                var tempbinudf4 = $(this).find('#aBinUDF4_' + tempINVLocID).val();
                var tempbinudf5 = $(this).find('#aBinUDF5_' + tempINVLocID).val();


                var data = {
                    "ID": tempINVLocID, "ItemNumber": tempItemname, "BinNumber": tempLocationName,
                    "MinimumQuantity": tempminimumquantity, "MaximumQuantity": tempmaximumquantity,
                    "CriticalQuantity": tempcriticalquantity, "SensorId": tempSensorId, "SensorPort": tempSensorPort, "IsDefault": tempisdefault, "IsDeleted": tempisdeleted,
                    "IsStagingLocation": tempisstaginglocation, "DefaultPullQuantity": tempdpq, "IsEnforceDefaultPullQuantity": tempisedpq, "DefaultReorderQuantity": tempdrq, "IsEnforceDefaultReorderQuantity": tempisedrq,
                    "BinUDF1": tempbinudf1, "BinUDF2": tempbinudf2, "BinUDF3": tempbinudf3, "BinUDF4": tempbinudf4, "BinUDF5": tempbinudf5,
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == LocationMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempLocationID = $(this).find('#LocationID').text();
                var tempLocation = $(this).find('#aLocation_' + tempLocationID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempLocationID, "Location": tempLocation, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == ToolCategoryselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempToolCategoryID = $(this).find('#ToolCategoryID').text();
                var tempToolCategory = $(this).find('#aToolCategory_' + tempToolCategoryID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempToolCategoryID, "ToolCategory": tempToolCategory, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == CategoryMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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

            });
        }
        else if (selectedModuleVal == CostUOMMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
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

            });
        }
        else if (selectedModuleVal == InventoryClassificationMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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
            });
        }
        else if (selectedModuleVal == CustomerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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
            });
        }
        else if (selectedModuleVal == FreightTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempFreightID = $(this).find('#FreightID').text();
                var tempFreightType = $(this).find('#aFreightType_' + tempFreightID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempFreightID, "FreightType": tempFreightType, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == GLAccountsMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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

            });
        }
        else if (selectedModuleVal == GXPRConsignedJobMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempGXPRID = $(this).find('#GXPRID').text();
                var tempGXPRConsigmentJob = $(this).find('#aGXPRConsinged_' + tempGXPRID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempGXPRID, "GXPRConsigmentJob": tempGXPRConsigmentJob, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == JobTypeMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempJobTypeID = $(this).find('#JobTypeID').text();
                var tempJobType = $(this).find('#aJobType_' + tempJobTypeID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempJobTypeID, "JobType": tempJobType, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == ShipViaMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempShipViaID = $(this).find('#ShipViaID').text();
                var tempShipVia = $(this).find('#aShipVia_' + tempShipViaID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempShipViaID, "ShipVia": tempShipVia, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == TechnicianMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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

            });
        }
        else if (selectedModuleVal == ManufacturerMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempManufacturerID = $(this).find('#ManufacturerID').text();
                var tempManufacturer = $(this).find('#aManufacturer_' + tempManufacturerID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempManufacturerID, "Manufacturer": tempManufacturer, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == MeasurementTermMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempMeasurementTermID = $(this).find('#MeasurementTermID').text();
                var tempMeasurementTerm = $(this).find('#aMeasurementTerm_' + tempMeasurementTermID).val();
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();

                var data = { "ID": tempMeasurementTermID, "MeasurementTerm": tempMeasurementTerm, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5 };
                arrItems.push(data);

            });
        }
        else if (selectedModuleVal == UnitMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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

            });
        }
        else if (selectedModuleVal == SupplierMasterselval) {
            if (!ValidateSupplierAccountDetails()) {
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }


            $('#myImportDataTable tbody tr').each(function () {
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
                var tempEmail = $(this).find('#aEmail_' + tempSupplierID).val();

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
                var vUDF6 = $(this).find('#aUDF6').val();
                var vUDF7 = $(this).find('#aUDF7').val();
                var vUDF8 = $(this).find('#aUDF8').val();
                var vUDF9 = $(this).find('#aUDF9').val();
                var vUDF10 = $(this).find('#aUDF10').val();

                var tempAccountNumber = $(this).find('#aAccountNumber_' + tempSupplierID).val();
                var tempAccountName = $(this).find('#aAccountName_' + tempSupplierID).val();
                var tempAccountAddress = $(this).find('#aAccountAddress_' + tempSupplierID).val();
                var tempAccountCity = $(this).find('#aAccountCity_' + tempSupplierID).val();
                var tempAccountState = $(this).find('#aAccountState_' + tempSupplierID).val();
                var tempAccountZip = $(this).find('#aAccountZip_' + tempSupplierID).val();

                var tempAccountCountry = $(this).find('#aAccountCountry_' + tempSupplierID).val();
                var tempAccountShipToID = $(this).find('#aAccountShipToID_' + tempSupplierID).val();

                var tempAccountIsDefault = $(this).find('#aAccountIsDefault_' + tempSupplierID).is(':checked');

                var tempBlanketPONumber = $(this).find('#aBlanketPONumber_' + tempSupplierID).val();
                var tempStartDate = $(this).find('#aStartDate_' + tempSupplierID).val();
                var tempEndDate = $(this).find('#aEndDate_' + tempSupplierID).val();
                var tempMaxLimit = $(this).find('#aMaxLimit_' + tempSupplierID).val();
                var tempDoNotExceed = $(this).find('#aDoNotExceed_' + tempSupplierID).is(':checked');

                var tempMaxLimitQty = $(this).find('#aMaxLimitQty_' + tempSupplierID).val();
                var tempDoNotExceedQty = $(this).find('#aDoNotExceedQty_' + tempSupplierID).is(':checked');

                var tempPullPurchaseNumberFixed = $(this).find('#aPullPurchaseNumberFixed_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberBlanketOrder = $(this).find('#aPullPurchaseNumberBlanketOrder_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberDateIncrementing = $(this).find('#aPullPurchaseNumberDateIncrementing_' + tempSupplierID).is(':checked');
                var tempPullPurchaseNumberDate = $(this).find('#aPullPurchaseNumberDate_' + tempSupplierID).is(':checked');
                var tempLastPullPurchaseNumberUsed = $(this).find('#aLastPullPurchaseNumberUsed_' + tempSupplierID).val();
                var tempIsBlanketDeleted = $(this).find('#aIsBlanketDeleted_' + tempSupplierID).is(':checked');
                var tempSupplierImage = $(this).find('#aSupplierImage_' + tempSupplierID).val();
                var tempImageExternalURL = $(this).find('#aImageExternalURL_' + tempSupplierID).val();
                var data = {
                    "ID": tempSupplierID, "SupplierName": tempSupplier, "SupplierColor": tempSupplierColor, "Description": tempDescription,
                    "BranchNumber": tempBranchNumber, "MaximumOrderSize": tempMaximumOrderSize, //"ReceiverID": tempReceiverID,
                    "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry,
                    "Contact": tempContact, "Phone": tempPhone, "Fax": tempFax, "IsSendtoVendor": tempIsSendtoVendor,
                    "IsVendorReturnAsn": tempIsVendorReturnAsn, "IsSupplierReceivesKitComponents": tempIsSupplierReceivesKitComponents,
                    "Email": tempEmail, //"IsEmailPOInBody": tempIsEmailPOInBody, "IsEmailPOInPDF": tempIsEmailPOInPDF, "IsEmailPOInCSV": tempIsEmailPOInCSV,
                    //"IsEmailPOInX12": tempIsEmailPOInX12,
                    "OrderNumberTypeBlank": tempOrderNumberTypeBlank, "OrderNumberTypeFixed": tempOrderNumberTypeFixed, "OrderNumberTypeBlanketOrderNumber": tempOrderNumberTypeBlanketOrderNumber,
                    "OrderNumberTypeIncrementingOrderNumber": tempOrderNumberTypeIncrementingOrderNumber, "OrderNumberTypeIncrementingbyDay": tempOrderNumberTypeIncrementingbyDay,
                    "OrderNumberTypeDateIncrementing": tempOrderNumberTypeDateIncrementing, "OrderNumberTypeDate": tempOrderNumberTypeDate,
                    "UDF1": vUDF1, "UDF2": vUDF2, "UDF3": vUDF3, "UDF4": vUDF4, "UDF5": vUDF5, "UDF6": vUDF6, "UDF7": vUDF7, "UDF8": vUDF8, "UDF9": vUDF9, "UDF10": vUDF10,
                    "AccountNumber": tempAccountNumber, "AccountName": tempAccountName,
                    "AccountAddress": tempAccountAddress, "AccountCity": tempAccountCity, "AccountState": tempAccountState, "AccountZip": tempAccountZip,
                    "AccountCountry": tempAccountCountry, "AccountShipToID": tempAccountShipToID, "AccountIsDefault": tempAccountIsDefault,
                    "BlanketPONumber": tempBlanketPONumber, "StartDate": tempStartDate, "EndDate": tempEndDate,
                    "MaxLimit": tempMaxLimit, "IsNotExceed": tempDoNotExceed,
                    "MaxLimitQty": tempMaxLimitQty, "IsNotExceedQty": tempDoNotExceedQty,
                    "PullPurchaseNumberFixed": tempPullPurchaseNumberFixed, "PullPurchaseNumberBlanketOrder": tempPullPurchaseNumberBlanketOrder,
                    "PullPurchaseNumberDateIncrementing": tempPullPurchaseNumberDateIncrementing, "PullPurchaseNumberDate": tempPullPurchaseNumberDate, "LastPullPurchaseNumberUsed": tempLastPullPurchaseNumberUsed,
                    "IsBlanketDeleted": tempIsBlanketDeleted,
                    "SupplierImage": tempSupplierImage, "ImageExternalURL": tempImageExternalURL,
                };
                arrItems.push(data);

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
            if (IsCurrentRoomConsigned == "False") {
                if (!ValidateConsignedItem()) {
                    //ErrorMessage += " :" + ErrorLineNumber;
                    $('#DivLoading').hide();
                    $('#btnSave').removeProp("disabled");
                    closeImportInfoModel();
                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                    $('#ImportInfoDialog').modal();
                    return;
                }
            }
            if (IsCurrentRoomOrderCostUOM == "False") {
                if (!ValidateOrderCostUOMItem()) {
                    //ErrorMessage += " :" + ErrorLineNumber;
                    $('#DivLoading').hide();
                    $('#btnSave').removeProp("disabled");
                    closeImportInfoModel();
                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                    $('#ImportInfoDialog').modal();
                    return;
                }
            }

            $('#myImportDataTable tbody tr').each(function () {
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
                var Link1 = '';//$(this).find('#aLink1_' + tempItemID).val();
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
                if (SerialNumberTracking) {
                    LotNumberTracking = false;
                }
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
                var ItemIsActive = false;
                if ($(this).find('#aItemIsActive_' + tempItemID).length > 0) {
                    ItemIsActive = $(this).find('#aItemIsActive_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsActive = true;
                }
                var vUDF6 = $(this).find('#aUDF6').val();
                var vUDF7 = $(this).find('#aUDF7').val();
                var vUDF8 = $(this).find('#aUDF8').val();
                var vUDF9 = $(this).find('#aUDF9').val();
                var vUDF10 = $(this).find('#aUDF10').val();
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
                var ItemIsAllowOrderCostuom = false;
                if ($(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).length > 0) {
                    ItemIsAllowOrderCostuom = $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsAllowOrderCostuom = false;
                }
                var eLabelKey = $(this).find('#aeLabelKey_' + tempItemID).val();
                var EnrichedProductData = $(this).find('#aEnrichedProductData_' + tempItemID).val();
                var EnhancedDescription = $(this).find('#aEnhancedDescription_' + tempItemID).val();
                var POItemLineNumber = $(this).find('#aPOItemLineNumber_' + tempItemID).val();
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
                    "ItemLink2ExternalURL": ItemLink2ExternalURL,
                    "IsActive": true,
                    "IsOrderable": ItemIsActive,
                    "UDF6": vUDF6,
                    "UDF7": vUDF7,
                    "UDF8": vUDF8,
                    "UDF9": vUDF9,
                    "UDF10": vUDF10,
                    "IsAllowOrderCostuom": ItemIsAllowOrderCostuom,
                    "eLabelKey": eLabelKey,
                    "EnrichedProductData": EnrichedProductData,
                    "EnhancedDescription": EnhancedDescription,
                    "POItemLineNumber": POItemLineNumber
                };


                //                var data = { "ID": tempItemID, "SupplierName": tempSupplier, "Description": tempDescription, "AccountNo": tempAccountNo,
                //                    "ReceiverID": tempReceiverID, "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry,
                //                    "Contact": tempContact, "Phone": tempPhone, "Fax": tempFax,
                //                    "Email": tempEmail, "IsEmailPOInBody": tempIsEmailPOInBody, "IsEmailPOInPDF": tempIsEmailPOInPDF, "IsEmailPOInCSV": tempIsEmailPOInCSV,
                //                    "IsEmailPOInX12": tempIsEmailPOInX12, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                //                };
                arrItems.push(data);

            });

        }
        else if (selectedModuleVal == EditItemMasterselval) {
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
            if (IsCurrentRoomConsigned == "False") {
                if (!ValidateConsignedItem()) {
                    //ErrorMessage += " :" + ErrorLineNumber;
                    $('#DivLoading').hide();
                    $('#btnSave').removeProp("disabled");
                    closeImportInfoModel();
                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                    $('#ImportInfoDialog').modal();
                    return;
                }
            }
            if (IsCurrentRoomOrderCostUOM == "False") {
                if (!ValidateOrderCostUOMItem()) {
                    //ErrorMessage += " :" + ErrorLineNumber;
                    $('#DivLoading').hide();
                    $('#btnSave').removeProp("disabled");
                    closeImportInfoModel();
                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                    $('#ImportInfoDialog').modal();
                    return;
                }
            }

            $('#myImportDataTable tbody tr').each(function () {
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
                var Link1 = '';//$(this).find('#aLink1_' + tempItemID).val();
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
                if (SerialNumberTracking) {
                    LotNumberTracking = false;
                }
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
                var ItemIsActive = false;
                if ($(this).find('#aItemIsActive_' + tempItemID).length > 0) {
                    ItemIsActive = $(this).find('#aItemIsActive_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsActive = true;
                }
                var vUDF6 = $(this).find('#aUDF6').val();
                var vUDF7 = $(this).find('#aUDF7').val();
                var vUDF8 = $(this).find('#aUDF8').val();
                var vUDF9 = $(this).find('#aUDF9').val();
                var vUDF10 = $(this).find('#aUDF10').val();
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
                var ItemIsAllowOrderCostuom = false;
                if ($(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).length > 0) {
                    ItemIsAllowOrderCostuom = $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsAllowOrderCostuom = false;
                }
                var eLabelKey = $(this).find('#aeLabelKey_' + tempItemID).val();
                var EnrichedProductData = $(this).find('#aEnrichedProductData_' + tempItemID).val();
                var EnhancedDescription = $(this).find('#aEnhancedDescription_' + tempItemID).val();
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
                    "ItemLink2ExternalURL": ItemLink2ExternalURL,
                    "IsActive": true,
                    "IsOrderable": ItemIsActive,
                    "UDF6": vUDF6,
                    "UDF7": vUDF7,
                    "UDF8": vUDF8,
                    "UDF9": vUDF9,
                    "UDF10": vUDF10,
                    "IsAllowOrderCostuom": ItemIsAllowOrderCostuom,
                    "eLabelKey": eLabelKey,
                    "EnrichedProductData": EnrichedProductData,
                    "EnhancedDescription": EnhancedDescription
                };


                //                var data = { "ID": tempItemID, "SupplierName": tempSupplier, "Description": tempDescription, "AccountNo": tempAccountNo,
                //                    "ReceiverID": tempReceiverID, "Address": tempAddress, "City": tempCity, "State": tempState, "ZipCode": tempZipCode, "Country": tempCountry,
                //                    "Contact": tempContact, "Phone": tempPhone, "Fax": tempFax,
                //                    "Email": tempEmail, "IsEmailPOInBody": tempIsEmailPOInBody, "IsEmailPOInPDF": tempIsEmailPOInPDF, "IsEmailPOInCSV": tempIsEmailPOInCSV,
                //                    "IsEmailPOInX12": tempIsEmailPOInX12, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                //                };
                arrItems.push(data);

            });

        }
        else if (selectedModuleVal == BOMItemMasterselval) {
            if (!BOMValidateCriticalMinMax()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }
            //if (!ValidateImageName()) {
            //    //ErrorMessage += " :" + ErrorLineNumber;
            //    $('#DivLoading').hide();
            //    $('#btnSave').removeProp("disabled");
            //    closeImportInfoModel();
            //    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

            //    $('#ImportInfoDialog').modal();
            //    return;
            //}
            $('#myImportDataTable tbody tr').each(function () {
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
                if (SerialNumberTracking) {
                    LotNumberTracking = false;
                }
                var DateCodeTracking = $(this).find('#aBOMDateCodeTracking_' + tempItemID).is(':checked');
                var ItemType = $(this).find('#aBOMItemType_' + tempItemID).val();
                //var vUDF1 = $(this).find('#aUDF1').val();
                //var vUDF2 = $(this).find('#aUDF2').val();
                //var vUDF3 = $(this).find('#aUDF3').val();
                //var vUDF4 = $(this).find('#aUDF4').val();
                //var vUDF5 = $(this).find('#aUDF5').val();
                var CriticalQuantity = 0;//$(this).find('#aBOMCriticalQuantity_' + tempItemID).val();
                var MinimumQuantity = $(this).find('#aBOMMinimumQuantity_' + tempItemID).val();
                var MaximumQuantity = $(this).find('#aBOMMaximumQuantity_' + tempItemID).val();

                var Cost = $(this).find('#aBOMCost_' + tempItemID).val();
                var Markup = $(this).find('#aBOMMarkup_' + tempItemID).val();
                var SellPrice = $(this).find('#aBOMSellPrice_' + tempItemID).val();

                var tempCostUOM = $(this).find('#aBOMCostUOM_' + tempItemID).val();
                var DefaultReorderQuantity = $(this).find('#aBOMDefaultReorderQuantity_' + tempItemID).val();
                var DefaultPullQuantity = $(this).find('#aBOMDefaultPullQuantity_' + tempItemID).val();
                var Link1 = '';//$(this).find('#aLink1_' + tempItemID).val();
                var Link2 = $(this).find('#aBOMLink2_' + tempItemID).val();
                // var InventoryClassification = $(this).find('#aInventoryClassification_' + tempItemID).val();
                var vUDF1 = $(this).find('#aBOMUDF1').val();
                var vUDF2 = $(this).find('#aBOMUDF2').val();
                var vUDF3 = $(this).find('#aBOMUDF3').val();
                var vUDF4 = $(this).find('#aBOMUDF4').val();
                var vUDF5 = $(this).find('#aBOMUDF5').val();
                var ItemLevelMinMaxQtyRequired = $(this).find('#aBOMItemLevelMinMaxQtyRequired_' + tempItemID).is(':checked');
                var PullQtyScanOverride = $(this).find('#aBOMPullQtyScanOverride_' + tempItemID).is(':checked');
                var EnforceDefaultReorderQuantity = $(this).find('#aBOMEnforceDefaultReorderQuantity_' + tempItemID).is(':checked');
                var tempItemImageExternalURL = $(this).find('#aBOMItemImageExternalURL_' + tempItemID).val();
                var tempItemDocExternalURL = $(this).find('#aBOMItemDocExternalURL_' + tempItemID).val();

                var ItemLink2ExternalURL = $(this).find('#aBOMItemLink2ExternalURL_' + tempItemID).val();
                var ItemIsActive = false;
                if ($(this).find('#aBOMItemIsActive_' + tempItemID).length > 0) {
                    ItemIsActive = $(this).find('#aBOMItemIsActive_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsActive = true;
                }

                if (CriticalQuantity == null || CriticalQuantity == '') {
                    CriticalQuantity = 0;
                }
                if (MinimumQuantity == null || MinimumQuantity == '') {
                    MinimumQuantity = 0;
                }
                if (MaximumQuantity == null || MaximumQuantity == '') {
                    MaximumQuantity = 0;
                }
                var ImagePath = $(this).find('#aImagePath_' + tempItemID).val();
                var LongDescription = $(this).find('#aBOMItemLongDescription_' + tempItemID).val();
                var EnrichedProductData = $(this).find('#aBOMItemEnrichedProductData_' + tempItemID).val();
                var EnhancedDescription = $(this).find('#aBOMItemEnhancedDescription_' + tempItemID).val();
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
                    "CriticalQuantity": parseFloat(CriticalQuantity),
                    "MinimumQuantity": parseFloat(MinimumQuantity),
                    "MaximumQuantity": parseFloat(MaximumQuantity),
                    "Cost": parseFloat(Cost),
                    "Markup": parseFloat(Markup),
                    "SellPrice": parseFloat(SellPrice),
                    "CostUOMName": tempCostUOM,

                    "DefaultReorderQuantity": parseFloat(DefaultReorderQuantity),
                    "DefaultPullQuantity": parseFloat(DefaultPullQuantity),
                    "Link1": Link1,
                    "Link2": Link2,
                    //"InventoryClassificationName": InventoryClassification,
                    "UDF1": vUDF1,
                    "UDF2": vUDF2,
                    "UDF3": vUDF3,
                    "UDF4": vUDF4,
                    "UDF5": vUDF5,
                    "IsItemLevelMinMaxQtyRequired": ItemLevelMinMaxQtyRequired,
                    "PullQtyScanOverride": PullQtyScanOverride,
                    "IsEnforceDefaultReorderQuantity": EnforceDefaultReorderQuantity,
                    "ItemImageExternalURL": tempItemImageExternalURL,
                    "ItemDocExternalURL": tempItemDocExternalURL,
                    "ItemLink2ExternalURL": ItemLink2ExternalURL,
                    "IsActive": ItemIsActive,
                    "ImagePath": ImagePath,
                    "LongDescription": LongDescription,
                    "EnrichedProductData": EnrichedProductData,
                    "EnhancedDescription": EnhancedDescription
                    //UDF1: vUDF1,
                    //UDF2: vUDF2,
                    //UDF3: vUDF3,
                    //UDF4: vUDF4,
                    //UDF5: vUDF5
                };

                arrItems.push(data);

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

                var tempTImagePath = $(this).find('#aImagePath_' + tempToolID).val();
                var tempToolImageExternalURL = $(this).find('#aToolImageExternalURL_' + tempToolID).val();
                var tempSerialNumberTracking = false;

                var tempPastMnts = null;
                var tempDueNoticeDays = null;

                if (AllowToolOrderingval == 'True') {
                    tempSerialNumberTracking = $(this).find('#aSerialNumberTracking_' + tempToolID).is(':checked');
                    var tempPastMnts = $(this).find('#aPastMnts_' + tempToolID).val();
                    var tempDueNoticeDays = $(this).find('#aDueNoticeDays_' + tempToolID).val();
                }

                if (tempSerialNumberTracking == true) {
                    tempSerial = '';
                    tempQuantity = '0';
                }

                if ($(this).find("input[type='checkbox'][id*='aisgroupofitems']").is(':checked'))
                    tempIsGrpofItems = 1;
                else
                    tempIsGrpofItems = 0;
                var data = {
                    "ID": tempToolID, "ToolName": tempToolName, "Serial": tempSerial, "Description": tempDescription,
                    "Cost": parseFloat(tempCost), "Quantity": parseFloat(tempQuantity), "ToolCategory": tempToolCategory, "Location": tempLocation,
                    UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, IsGroupOfItems: tempIsGrpofItems, "Technician": vTechnician, "CheckOutQuantity": vCheckOutQty,
                    "CheckInQuantity": vCheckInQty, "CheckOutUDF1": vCheckOutUDF1, "CheckOutUDF2": vCheckOutUDF2, "CheckOutUDF3": vCheckOutUDF3, "CheckOutUDF4": vCheckOutUDF4, "CheckOutUDF5": vCheckOutUDF5,
                    "ImagePath": tempTImagePath, "NoOfPastMntsToConsider": tempPastMnts, "MaintenanceDueNoticeDays": tempDueNoticeDays,
                    "ToolImageExternalURL": tempToolImageExternalURL, "SerialNumberTracking": tempSerialNumberTracking
                };
                arrItems.push(data);



            });

        }
        else if (selectedModuleVal == AssetToolSchedulerMappingval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempMappingID = $(this).find('#MappingID').text();


                var tempScheduleFor = $(this).find('#aScheduleFor_' + tempMappingID).val();
                var tempSchedulerName = $(this).find('#aSchedulerName_' + tempMappingID).val();
                var tempAssetName = $(this).find('#aAssetName_' + tempMappingID).val();
                var tempToolName = $(this).find('#aToolName_' + tempMappingID).val();
                var tempSerial = $(this).find('#aSerial_' + tempMappingID).val();

                var tempUDF1 = $(this).find('#aUDF1_' + tempMappingID).val();
                var tempUDF2 = $(this).find('#aUDF2_' + tempMappingID).val();
                var tempUDF3 = $(this).find('#aUDF3_' + tempMappingID).val();
                var tempUDF4 = $(this).find('#aUDF4_' + tempMappingID).val();
                var tempUDF5 = $(this).find('#aUDF5_' + tempMappingID).val();


                var data = {
                    "ID": tempMappingID, "AssetName": tempAssetName, "ToolName": tempToolName, "Serial": tempSerial,
                    "SchedulerName": tempSchedulerName, "ScheduleForName": tempScheduleFor,
                    "UDF1": tempUDF1, "UDF2": tempUDF2, "UDF3": tempUDF3, "UDF4": tempUDF4, "UDF5": tempUDF5

                };

                arrItems.push(data);
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

                var tempImagePath = $(this).find('#aImagePath_' + tempAssetID).val();
                var tempAssetImageExternalURL = $(this).find('#aAssetImageExternalURL_' + tempAssetID).val();


                var data = {
                    "ID": tempAssetID, "AssetName": tempAssetName, "Serial": tempSerial, "Description": tempDescription,
                    "Make": tempMake, "Model": tempModel, "ToolCategory": tempToolCategory, "PurchaseDateString": tempPurchaseDate,
                    "PurchasePrice": tempPurchasePrice, "DepreciatedValue": tempDepreciatedValue, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5,
                    UDF6: vUDF6, UDF7: vUDF7, UDF8: vUDF8, UDF9: vUDF9, UDF10: vUDF10,
                    "AssetCategory": tempAssetCategory,
                    "ImagePath": tempImagePath,
                    "AssetImageExternalURL": tempAssetImageExternalURL
                };

                arrItems.push(data);

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
            });
        }
        else if (selectedModuleVal == OrderMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempQuickListID = $(this).find('#OrderID').text();
                var tempSupplier = $(this).find('#aSupplier_' + tempQuickListID).val();
                var tempOrderNumber = $(this).find('#aOrderNumber_' + tempQuickListID).val();
                var tempRequiredDate = $(this).find('#aRequiredDate_' + tempQuickListID).val();
                var tempOrderStatus = $(this).find('#aOrderStatus_' + tempQuickListID).val();
                var tempStagingName = $(this).find('#aStagingName').val();
                var tempOrderComment = $(this).find('#aOrderComment').val();
                var tempCustomerName = $(this).find('#aCustomerName').val();
                var tempPackSlipNumber = $(this).find('#aPackSlipNumber').val();
                var tempShippingTrackNumber = $(this).find('#aShippingTrackNumber').val();
                var tempOrderUdf1 = $(this).find('#aOrderUDF1').val();
                var tempOrderUdf2 = $(this).find('#aOrderUDF2').val();
                var tempOrderUdf3 = $(this).find('#aOrderUDF3').val();
                var tempOrderUdf4 = $(this).find('#aOrderUDF4').val();
                var tempOrderUdf5 = $(this).find('#aOrderUDF5').val();
                var tempShipVia = $(this).find('#aShipVia').val();
                var tempOrderType = $(this).find('#aOrderType').val();
                var tempShippingVendor = $(this).find('#aShippingVendor').val();
                //var tempAccountNumber = $(this).find('#aAccountNumber').val();
                var tempSupplierAccount = $(this).find('#aSupplierAccount').val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempQuickListID).val();
                var tempBinNumber = $(this).find('#aBin_' + tempQuickListID).val();
                var tempRequestedQTY = $(this).find('#aRequestedQty_' + tempQuickListID).val();
                var tempReceivedQty = $(this).find('#aReceivedQty_' + tempQuickListID).val();
                var tempASNNumber = $(this).find('#aASNNumber').val();
                var tempApprovedQty = $(this).find('#aApprovedQty_' + tempQuickListID).val();
                var tempInTransitQty = $(this).find('#aInTransitQty_' + tempQuickListID).val();
                var tempIsCloseItem = $(this).find('#aIsCloseItem').is(':checked');
                var tempLineNumber = $(this).find('#aLineNumber').val();
                var tempControlNumber = $(this).find('#aControlNumber').val();
                var tempItemComment = $(this).find('#aItemComment').val();
                var tempLineItemUDF1 = $(this).find('#aLineItemUDF1').val();
                var tempLineItemUDF2 = $(this).find('#aLineItemUDF2').val();
                var tempLineItemUDF3 = $(this).find('#aLineItemUDF3').val();
                var tempLineItemUDF4 = $(this).find('#aLineItemUDF4').val();
                var tempLineItemUDF5 = $(this).find('#aLineItemUDF5').val();
                var tempOrderCost = $(this).find('#aOrderCost_' + tempQuickListID).val();
                var tempSalesOrder = $(this).find('#aSalesOrder_' + tempQuickListID).val();

                var data = {
                    "ID": tempQuickListID, "Supplier": tempSupplier, "OrderNumber": tempOrderNumber, "RequiredDate": tempRequiredDate, "OrderStatus": tempOrderStatus,
                    "StagingName": tempStagingName, "OrderComment": tempOrderComment, "CustomerName": tempCustomerName, "PackSlipNumber": tempPackSlipNumber,
                    "ShippingTrackNumber": tempShippingTrackNumber, "OrderUdf1": tempOrderUdf1, "OrderUdf2": tempOrderUdf2, "OrderUdf3": tempOrderUdf3, "OrderUdf4": tempOrderUdf4, "OrderUdf5": tempOrderUdf5,
                    "ShipVia": tempShipVia, "OrderType": tempOrderType, "ShippingVendor": tempShippingVendor, //"AccountNumber": tempAccountNumber, 
                    "SupplierAccount": tempSupplierAccount, "ItemNumber": tempItemNumber, "Bin": tempBinNumber, "RequestedQTY": tempRequestedQTY,
                    "ReceivedQty": tempReceivedQty, "ASNNumber": tempASNNumber, "ApprovedQty": tempApprovedQty, "InTransitQty": tempInTransitQty,
                    "IsCloseItem": tempIsCloseItem, "LineNumber": tempLineNumber, "ControlNumber": tempControlNumber, "ItemComment": tempItemComment,
                    "LineItemUDF1": tempLineItemUDF1, "LineItemUDF2": tempLineItemUDF2, "LineItemUDF3": tempLineItemUDF3, "LineItemUDF4": tempLineItemUDF4,
                    "LineItemUDF5": tempLineItemUDF5,
                    "OrderCost": tempOrderCost, "SalesOrder": tempSalesOrder
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == Kitsselval) {

            $('#myImportDataTable tbody tr').each(function () {
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
                var tempMaximumQuantity = $(this).find('#aMaximumQuantity_' + tempKitID).val();
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
                var tempIsActive = 1;
                if ($(this).find("input[type='checkbox'][id*='aKitIsActive_" + tempKitID + "']").is(':checked'))
                    tempIsActive = true;
                else
                    tempIsActive = false;
                var tempSerialNumber = 1;
                if ($(this).find('#aKitSerialNumber_' + tempKitID).is(':checked'))
                    tempSerialNumber = true;
                else
                    tempSerialNumber = false;

                var tempLotNumber = 1;
                if ($(this).find('#aKitLotNumber_' + tempKitID).is(':checked'))
                    tempLotNumber = true;
                else
                    tempLotNumber = false;

                var tempExpirationDate = 1;

                if ($(this).find('#aKitExpirationDate_' + tempKitID).is(':checked'))
                    tempExpirationDate = true;
                else
                    tempExpirationDate = false;


                var data = {
                    "ID": tempKitID, "KitPartNumber": tempKitPartNumber, "ItemNumber": tempKitItemNumber, "QuantityPerKit": tempQuantityPerKit, "IsBuildBreak": tempIsBuildBreak, "IsDeleted": tempIsDelete
                    , "Description": tempDescription, "CriticalQuantity": tempCriticalQuantity, "MinimumQuantity": tempMinimumQuantity, "AvailableKitQuantity": tempAvailableKitQuantity,
                    "MaximumQuantity": tempMaximumQuantity, "ReOrderType": tempReOrderType, "KitCategory": tempKitCategory, "KitDemand": tempKitDemand,
                    "SupplierName": tempSupplierName, "SupplierPartNo": tempSupplierPartNo, "DefaultLocationName": tempDefaultLocation, "CostUOMName": tempCostUOMName,
                    "UOM": tempUOM, "DefaultReorderQuantity": tempDefaultReorderQuantity, "DefaultPullQuantity": tempDefaultPullQuantity, "IsItemLevelMinMaxQtyRequired": tempIsItemLevelMinMaxQtyRequired, "SerialNumberTracking": tempSerialNumber, "LotNumberTracking": tempLotNumber, "DateCodeTracking": tempExpirationDate, "IsActive": tempIsActive

                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == ItemManufacturerselval) {
            $('#myImportDataTable tbody tr').each(function () {
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
            });
        }
        else if (selectedModuleVal == ItemSupplierselval) {
            if (isUniqueItemAndSupplierNumber()) {
                //ErrorMessage += " :" + ErrorLineNumber;
                $('#DivLoading').hide();
                $('#btnSave').removeProp("disabled");
                closeImportInfoModel();
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + ErrorMessage + "</b>");

                $('#ImportInfoDialog').modal();
                return;
            }

            $('#myImportDataTable tbody tr').each(function () {
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
                var tempBarcodeID = $(this).find('#BarcodeID').text();
                var tempModuleName = $(this).find('#aModuleName_' + tempBarcodeID).val();
                var tempItemnumber = $(this).find('#aItemNumber_' + tempBarcodeID).val();
                var tempBarCodeString = $(this).find('#aBarCodeString_' + tempBarcodeID).val();
                var tempBinNumber = $(this).find('#aBinNumber_' + tempBarcodeID).val();



                var data = {
                    "ID": tempBarcodeID, "Itemnumber": tempItemnumber, "ModuleName": tempModuleName, "BarCodeString": tempBarCodeString, "BinNumber": tempBinNumber
                };
                arrItems.push(data);
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
            });
        }
        else if (selectedModuleVal == ProjectMasterselval) {
            $('#myImportDataTable tbody tr').each(function () {
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
            });
        }
        else if (selectedModuleVal == WorkOrderval) {
            $('#myImportDataTable tbody tr').each(function () {
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
                var tempSupplierName = $(this).find('#aSupplierName_' + tempWOID).val();
                var tempAsset = $(this).find('#aAsset_' + tempWOID).val();
                var tempOdometer = $(this).find('#aOdometer_' + tempWOID).val();
                var tempSupplierAccount = $(this).find('#aSupplierAccount_' + tempWOID).val();

                var data = {
                    "ID": tempWOID, "WOName": tempWOName, "Description": tempDescription,
                    "WOStatus": tempWOStatus, "Technician": tempTechnician,
                    "Customer": tempCustomer, "WOType": tempWOType, "UDF1": tempUDF1, "UDF2": tempUDF2,
                    "UDF3": tempUDF3, "UDF4": tempUDF4, "UDF5": tempUDF5, "ReleaseNumber": tempReleaseNumber,
                    "SupplierName": tempSupplierName, "Asset": tempAsset, "Odometer_OperationHours": tempOdometer,
                    "SupplierAccount": tempSupplierAccount
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == PullImport) {
            $('#myImportDataTable tbody tr').each(function () {
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
                var tempAsset = $(this).find('#aAsset_' + tempPIID).val();
                var tempPIActionType = $(this).find('#aActionType_' + tempPIID).val();
                var tempWOItemSellPrice = null;
                if (tempPIWorkOrder != "") {
                    tempWOItemSellPrice = $(this).find('#aWOItemSellPrice_' + tempPIID).val();
                }
                var data = {
                    "ID": tempPIID, "ItemNumber": tempPIItemNumber, "PullQuantity": tempPIPullQuantity, "Location": tempPILocationName,
                    "UDF1": tempPIUDF1, "UDF2": tempPIUDF2, "UDF3": tempPIUDF3, "UDF4": tempPIUDF4, "UDF5": tempPIUDF5,
                    "ProjectSpendName": tempPIProjectSpendName, "PullOrderNumber": tempPIPullOrderNumber, "WorkOrder": tempPIWorkOrder,
                    "Asset": tempAsset, "ActionType": tempPIActionType, "ItemSellPrice": tempWOItemSellPrice
                };

                arrItems.push(data);
            });
        }

        else if (selectedModuleVal == PullImportWithLotSerialVal) {
            $('#myImportDataTable tbody tr').each(function () {
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
                var tempAsset = $(this).find('#aAsset_' + tempPIID).val();
                var tempPIActionType = $(this).find('#aActionType_' + tempPIID).val();

                var tempLotNumber = $(this).find('#aLotNumber_' + tempPIID).val();
                var tempSerialNumber = $(this).find('#aSerialNumber_' + tempPIID).val();
                var tempExpirationDate = $(this).find('#aExpirationDate_' + tempPIID).val();
                var tempWOItemSellPrice = null;
                if (tempPIWorkOrder != "") {
                    tempWOItemSellPrice = $(this).find('#aWOItemSellPrice_' + tempPIID).val();
                }

                var data = {
                    "ID": tempPIID, "ItemNumber": tempPIItemNumber, "PullQuantity": tempPIPullQuantity, "Location": tempPILocationName,
                    "UDF1": tempPIUDF1, "UDF2": tempPIUDF2, "UDF3": tempPIUDF3, "UDF4": tempPIUDF4, "UDF5": tempPIUDF5,
                    "ProjectSpendName": tempPIProjectSpendName, "PullOrderNumber": tempPIPullOrderNumber, "WorkOrder": tempPIWorkOrder,
                    "Asset": tempAsset, "LotNumber": tempLotNumber, "SerialNumber": tempSerialNumber, "ExpirationDate": tempExpirationDate, "ActionType": tempPIActionType
                    , "ItemSellPrice": tempWOItemSellPrice
                };

                arrItems.push(data);
            });
        }

        else if (selectedModuleVal == ItemLocationChangeImport) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempPIID = $(this).find('#InvLocId').text();
                var tempPIItemNumber = $(this).find('#aItemNumber_' + tempPIID).val();
                var tempOldLocationName = $(this).find('#aOldLocationName_' + tempPIID).val();
                var tempNewLocationName = $(this).find('#aNewLocationName_' + tempPIID).val();

                var data = {
                    "ID": tempPIID, "ItemNumber": tempPIItemNumber, "OldLocationName": tempOldLocationName, "NewLocationName": tempNewLocationName
                };

                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == PullImportWithSameQty) {
            $('#myImportDataTable tbody tr').each(function () {
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
                var tempCreated = $(this).find('#atempCreated_' + tempPIID).val();
                var tempItemCost = $(this).find('#aItemCost_' + tempPIID).val();
                var tempCostUOMValue = $(this).find('#aCostUOMValue_' + tempPIID).val();
                var tempAsset = $(this).find('#aAsset_' + tempPIID).val();

                var data = {
                    "ID": tempPIID, "ItemNumber": tempPIItemNumber, "PullQuantity": tempPIPullQuantity, "BinNumber": tempPILocationName,
                    "UDF1": tempPIUDF1, "UDF2": tempPIUDF2, "UDF3": tempPIUDF3, "UDF4": tempPIUDF4, "UDF5": tempPIUDF5,
                    "ProjectSpendName": tempPIProjectSpendName, "PullOrderNumber": tempPIPullOrderNumber, "WorkOrder": tempPIWorkOrder,
                    "Asset": tempAsset, "ActionType": tempPIActionType, "Created": tempCreated, "ItemCost": tempItemCost, "CostUOMValue": tempCostUOMValue
                };

                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == AssetToolScheduler) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempScheID = $(this).find('#SchedulerID').text();

                var tempItemNumber = $(this).find('#aItemNumber_' + tempScheID).val();
                var tempQuantity = $(this).find('#aQuantity_' + tempScheID).val();

                var tempScheduleFor = $(this).find('#aScheduleFor_' + tempScheID).val();
                var tempSchedulerName = $(this).find('#aSchedulerName_' + tempScheID).val();
                var tempDescription = $(this).find('#aDescription_' + tempScheID).val();

                var tempUDF1 = $(this).find('#aUDF1_' + tempScheID).val();
                var tempUDF2 = $(this).find('#aUDF2_' + tempScheID).val();
                var tempUDF3 = $(this).find('#aUDF3_' + tempScheID).val();
                var tempUDF4 = $(this).find('#aUDF4_' + tempScheID).val();
                var tempUDF5 = $(this).find('#aUDF5_' + tempScheID).val();


                var tempSchedulerTypeName = $(this).find('#aSchedulerType_' + tempScheID).val();
                var tempTBU = $(this).find('#aTimeBasedUnit_' + tempScheID).val();
                var tempTBF = $(this).find('#aTimeBasedFrequency_' + tempScheID).val();
                var tempCheckOuts = $(this).find('#aCheckouts_' + tempScheID).val();
                var tempOperationalHours = $(this).find('#aOperationalHours_' + tempScheID).val();
                var tempMileage = $(this).find('#aMileage_' + tempScheID).val();
                var tempisdeleted = $(this).find('#aisdeleted_' + tempScheID).is(':checked');

                var data = {
                    "ID": tempScheID, "ItemNumber": tempItemNumber, "Quantity": tempQuantity, "Description": tempDescription,
                    "SchedulerName": tempSchedulerName, "ScheduleForName": tempScheduleFor,
                    "UDF1": tempUDF1, "UDF2": tempUDF2, "UDF3": tempUDF3, "UDF4": tempUDF4, "UDF5": tempUDF5,
                    "SchedulerTypeName": tempSchedulerTypeName, "TimeBasedUnitName": tempTBU, "TimeBasedFrequency": tempTBF,
                    "CheckOuts": tempCheckOuts, "OperationalHours": tempOperationalHours, "Mileage": tempMileage, "IsDeleted": tempisdeleted
                };

                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == ToolCheckInCheckOut) {

            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).find('#Id').text();
                var toolName = "";
                var vLocation = "";
                if (isAllowToolOrdering.toLowerCase() == "true") {
                    toolName = $(this).find('#aToolName_' + id).val();
                    vLocation = $(this).find('#aLocation_' + id).val();
                }
                var serial = $(this).find('#aSerial_' + id).val();

                var technicianCode = $(this).find('#aTechnicianCode_' + id).val();
                var quantity = $(this).find('#aQuantity_' + id).val();
                var operation = $(this).find('#aOperation_' + id).val();
                var checkOutUDF1 = $(this).find('#aCheckOutUDF1').val();
                var checkOutUDF2 = $(this).find('#aCheckOutUDF2').val();
                var checkOutUDF3 = $(this).find('#aCheckOutUDF3').val();
                var checkOutUDF4 = $(this).find('#aCheckOutUDF4').val();
                var checkOutUDF5 = $(this).find('#aCheckOutUDF5').val();

                var data = {
                    "ID": id,
                    "ToolName": toolName,
                    "Serial": serial,
                    "Location": vLocation,
                    "TechnicianCode": technicianCode,
                    "Quantity": parseFloat(quantity),
                    "Operation": operation,
                    "CheckOutUDF1": checkOutUDF1,
                    "CheckOutUDF2": checkOutUDF2,
                    "CheckOutUDF3": checkOutUDF3,
                    "CheckOutUDF4": checkOutUDF4,
                    "CheckOutUDF5": checkOutUDF5
                };
                arrItems.push(data);
            });

        }
        else if (selectedModuleVal == ToolAdjustmentCountval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempItemname = $(this).find('#aToolName_' + tempINVLocID).val();
                var tempLocationName = $(this).find('#aLocationName_' + tempINVLocID).val();
                var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
                var tempSerialNumber = $.trim($(this).find('#aSerialNumber_' + tempINVLocID).val());
                var vUDF1 = $(this).find('#aUDF1').val();
                var vUDF2 = $(this).find('#aUDF2').val();
                var vUDF3 = $(this).find('#aUDF3').val();
                var vUDF4 = $(this).find('#aUDF4').val();
                var vUDF5 = $(this).find('#aUDF5').val();
                var data = {
                    "ID": tempINVLocID, "ToolName": tempItemname, "BinNumber": tempLocationName,
                    "Quantity": tempcustomerownedQTY, "SerialNumber": $.trim(tempSerialNumber),
                    UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == ToolCertificationImagesval) {
            if (!CheckZIPFileNameForToolCertificationImages()) {
                return;
            }
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).find('#Id').text();
                var toolName = $(this).find('#aToolName_' + id).val();
                var serialNumber = $(this).find('#aSerialNumber_' + id).val();
                var imageName = $(this).find('#aImageName_' + id).val();
                var data = {
                    "Id": id,
                    "ToolName": toolName,
                    "Serial": serialNumber,
                    "ImageName": imageName
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == MoveMaterialval) {

            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).find('#Id').text();
                var ItemNumber = $(this).find('#aItemNumber_' + id).val();
                var SourceBin = $(this).find('#aSourceBin_' + id).val();
                var DestinationBin = $(this).find('#aDestinationBin_' + id).val();
                var MoveType = $(this).find('#aMoveType_' + id).val();
                var Quantity = $(this).find('#aQuantity_' + id).val();
                var DestinationStagingHeader = $(this).find('#aDestinationStagingHeader_' + id).val();

                var data = {
                    "Id": id,
                    "ItemNumber": ItemNumber,
                    "SourceBin": SourceBin,
                    "DestinationBin": DestinationBin,
                    "MoveType": MoveType,
                    "Quantity": parseFloat(Quantity),
                    "DestinationStagingHeader": DestinationStagingHeader
                };
                arrItems.push(data);
            });

        }
        else if (selectedModuleVal == Requisitionval) {
            isLoadAllItems = true;
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).find('#Id').text();
                var RequisitionNumber = $(this).find('#aRequisitionNumber_' + id).val();
                var Workorder = $(this).find('#aWorkorder_' + id).val();
                var RequiredDate = $(this).find('#aRequiredDate_' + id).val();
                var RequisitionStatus = $(this).find('#aRequisitionStatus_' + id).val();
                var CustomerName = $(this).find('#aCustomerName_' + id).val();
                var ReleaseNumber = $(this).find('#aReleaseNumber_' + id).val();
                var ProjectSpend = $(this).find('#aProjectSpend_' + id).val();
                var Description = $(this).find('#aDescription_' + id).val();
                var StagingName = $(this).find('#aStagingName_' + id).val();
                var Supplier = $(this).find('#aSupplier_' + id).val();
                var SupplierAccount = $(this).find('#aSupplierAccount_' + id).val();
                var BillingAccount = $(this).find('#aBillingAccount_' + id).val();
                var Technician = $(this).find('#aTechnician_' + id).val();
                var RequisitionUDF1 = $(this).find('#aRequisitionUDF1_' + id).val();
                var RequisitionUDF2 = $(this).find('#aRequisitionUDF2_' + id).val();
                var RequisitionUDF3 = $(this).find('#aRequisitionUDF3_' + id).val();
                var RequisitionUDF4 = $(this).find('#aRequisitionUDF4_' + id).val();
                var RequisitionUDF5 = $(this).find('#aRequisitionUDF5_' + id).val();
                var ItemNumber = $(this).find('#aItemNumber_' + id).val();
                var Tool = $(this).find('#aTool_' + id).val();
                var ToolSerial = $(this).find('#aToolSerial_' + id).val();
                var Bin = $(this).find('#aBin_' + id).val();
                var QuantityRequisitioned = $(this).find('#aQuantityRequisitioned_' + id).val();
                var QuantityApproved = $(this).find('#aQuantityApproved_' + id).val();
                var QuantityPulled = $(this).find('#aQuantityPulled_' + id).val();
                var LineItemRequiredDate = $(this).find('#aLineItemRequiredDate_' + id).val();
                var LineItemProjectSpend = $(this).find('#aLineItemProjectSpend_' + id).val();
                var LineItemSupplierAccount = $(this).find('#aLineItemSupplierAccount_' + id).val();
                var PullOrderNumber = $(this).find('#aPullOrderNumber_' + id).val();
                var LineItemTechnician = $(this).find('#aLineItemTechnician_' + id).val();
                var PullUDF1 = $(this).find('#aPullUDF1_' + id).val();
                var PullUDF2 = $(this).find('#aPullUDF2_' + id).val();
                var PullUDF3 = $(this).find('#aPullUDF3_' + id).val();
                var PullUDF4 = $(this).find('#aPullUDF4_' + id).val();
                var PullUDF5 = $(this).find('#aPullUDF5_' + id).val();
                var ToolCheckoutUDF1 = $(this).find('#aToolCheckoutUDF1_' + id).val();
                var ToolCheckoutUDF2 = $(this).find('#aToolCheckoutUDF2_' + id).val();
                var ToolCheckoutUDF3 = $(this).find('#aToolCheckoutUDF3_' + id).val();
                var ToolCheckoutUDF4 = $(this).find('#aToolCheckoutUDF4_' + id).val();
                var ToolCheckoutUDF5 = $(this).find('#aToolCheckoutUDF5_' + id).val();

                var data = {
                    "Id": id,
                    "RequisitionNumber": RequisitionNumber,
                    "Workorder": Workorder,
                    "RequiredDate": RequiredDate,
                    "RequisitionStatus": RequisitionStatus,
                    "CustomerName": CustomerName,
                    "ReleaseNumber": ReleaseNumber,
                    "ProjectSpend": ProjectSpend,
                    "Description": Description,
                    "StagingName": StagingName,
                    "Supplier": Supplier,
                    "SupplierAccount": SupplierAccount,
                    "BillingAccount": BillingAccount,
                    "Technician": Technician,
                    "RequisitionUDF1": RequisitionUDF1,
                    "RequisitionUDF2": RequisitionUDF2,
                    "RequisitionUDF3": RequisitionUDF3,
                    "RequisitionUDF4": RequisitionUDF4,
                    "RequisitionUDF5": RequisitionUDF5,
                    "ItemNumber": ItemNumber,
                    "Tool": Tool,
                    "ToolSerial": ToolSerial,
                    "Bin": Bin,
                    "QuantityRequisitioned": parseFloat(QuantityRequisitioned),
                    "QuantityApproved": parseFloat(QuantityApproved),
                    "QuantityPulled": parseFloat(QuantityPulled),
                    "LineItemRequiredDate": LineItemRequiredDate,
                    "LineItemProjectSpend": LineItemProjectSpend,
                    "LineItemSupplierAccount": LineItemSupplierAccount,
                    "PullOrderNumber": PullOrderNumber,
                    "LineItemTechnician": LineItemTechnician,
                    "PullUDF1": PullUDF1,
                    "PullUDF2": PullUDF2,
                    "PullUDF3": PullUDF3,
                    "PullUDF4": PullUDF4,
                    "PullUDF5": PullUDF5,
                    "ToolCheckoutUDF1": ToolCheckoutUDF1,
                    "ToolCheckoutUDF2": ToolCheckoutUDF2,
                    "ToolCheckoutUDF3": ToolCheckoutUDF3,
                    "ToolCheckoutUDF4": ToolCheckoutUDF4,
                    "ToolCheckoutUDF5": ToolCheckoutUDF5
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == EnterpriseQuickListval) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).find('#Id').text();
                var Name = $(this).find('#aName_' + id).val();
                var QLDetailNumber = $(this).find('#aQLDetailNumber_' + id).val();
                var Quantity = $(this).find('#aQuantity_' + id).val();

                var data = {
                    "Id": id,
                    "Name": Name,
                    "QLDetailNumber": QLDetailNumber,
                    "Quantity": parseFloat(Quantity)
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == QuoteMasterselval) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempQuickListID = $(this).find('#QuoteID').text();
                var tempSupplierName = $(this).find('#aSupplierName_' + tempQuickListID).val();
                var tempQuoteNumber = $(this).find('#aQuoteNumber_' + tempQuickListID).val();
                var tempRequiredDate = $(this).find('#aRequiredDate_' + tempQuickListID).val();
                var tempQuoteStatus = $(this).find('#aQuoteStatus_' + tempQuickListID).val();
                var tempQuoteComment = $(this).find('#aQuoteComment').val();
                var tempCustomerName = $(this).find('#aCustomerName').val();
                var tempPackSlipNumber = $(this).find('#aPackSlipNumber').val();
                var tempShippingTrackNumber = $(this).find('#aShippingTrackNumber').val();
                var tempQuoteUdf1 = $(this).find('#aQuoteUDF1').val();
                var tempQuoteUdf2 = $(this).find('#aQuoteUDF2').val();
                var tempQuoteUdf3 = $(this).find('#aQuoteUDF3').val();
                var tempQuoteUdf4 = $(this).find('#aQuoteUDF4').val();
                var tempQuoteUdf5 = $(this).find('#aQuoteUDF5').val();
                var tempShipVia = $(this).find('#aShipVia').val();
                var tempShippingVendor = $(this).find('#aShippingVendor').val();
                var tempSupplierAccount = $(this).find('#aSupplierAccount').val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempQuickListID).val();
                var tempBinNumber = $(this).find('#aBin_' + tempQuickListID).val();
                var tempRequestedQTY = $(this).find('#aRequestedQty_' + tempQuickListID).val();
                var tempASNNumber = $(this).find('#aASNNumber').val();
                var tempApprovedQty = $(this).find('#aApprovedQty_' + tempQuickListID).val();
                var tempInTransitQty = $(this).find('#aInTransitQty_' + tempQuickListID).val();
                var tempIsCloseItem = $(this).find('#aIsCloseItem').is(':checked');
                var tempLineNumber = $(this).find('#aLineNumber').val();
                var tempControlNumber = $(this).find('#aControlNumber').val();
                var tempItemComment = $(this).find('#aItemComment').val();
                var tempLineItemUDF1 = $(this).find('#aLineItemUDF1').val();
                var tempLineItemUDF2 = $(this).find('#aLineItemUDF2').val();
                var tempLineItemUDF3 = $(this).find('#aLineItemUDF3').val();
                var tempLineItemUDF4 = $(this).find('#aLineItemUDF4').val();
                var tempLineItemUDF5 = $(this).find('#aLineItemUDF5').val();
                var tempQuoteCost = $(this).find('#aQuoteCost_' + tempQuickListID).val();

                var data = {
                    "ID": tempQuickListID, "QuoteNumber": tempQuoteNumber, "RequiredDate": tempRequiredDate, "QuoteStatus": tempQuoteStatus,
                    "QuoteComment": tempQuoteComment, "CustomerName": tempCustomerName, "PackSlipNumber": tempPackSlipNumber,
                    "ShippingTrackNumber": tempShippingTrackNumber, "QuoteUdf1": tempQuoteUdf1, "QuoteUdf2": tempQuoteUdf2, "QuoteUdf3": tempQuoteUdf3, "QuoteUdf4": tempQuoteUdf4, "QuoteUdf5": tempQuoteUdf5,
                    "ShipVia": tempShipVia, "ShippingVendor": tempShippingVendor,
                    "SupplierAccount": tempSupplierAccount, "ItemNumber": tempItemNumber, "Bin": tempBinNumber, "RequestedQTY": tempRequestedQTY,
                    "ASNNumber": tempASNNumber, "ApprovedQty": tempApprovedQty, "InTransitQty": tempInTransitQty,
                    "IsCloseItem": tempIsCloseItem, "LineNumber": tempLineNumber, "ControlNumber": tempControlNumber, "ItemComment": tempItemComment,
                    "LineItemUDF1": tempLineItemUDF1, "LineItemUDF2": tempLineItemUDF2, "LineItemUDF3": tempLineItemUDF3, "LineItemUDF4": tempLineItemUDF4,
                    "LineItemUDF5": tempLineItemUDF5,
                    "QuoteCost": tempQuoteCost, "SupplierName": tempSupplierName
                };
                arrItems.push(data);
            });
        }
        else if (selectedModuleVal == SupplierCatalogVal) {
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
                var tempItemID = $(this).find('#ItemID').text();

                var tempItemNumber = $(this).find('#aItemNumber_' + tempItemID).val();
                var tempDescription = $(this).find('#aDescription_' + tempItemID).val();
                var tmpSellPrice = $(this).find('#aSellPrice_' + tempItemID).val();
                //var tmpPackingQuantity = $(this).find('#aPackingQuantity_' + tempItemID).val();
                var tempManufacturerNumber = $(this).find('#aManufacturerPartNumber_' + tempItemID).val();
                var ImagePath = $(this).find('#aImagePath_' + tempItemID).val();
                var tempUPC = $(this).find('#aUPC_' + tempItemID).val();
                var tempSupplierPartNo = $(this).find('#aSupplierPartNumber_' + tempItemID).val();
                var tempSupplierName = $(this).find('#aSupplierName_' + tempItemID).val();
                var tempManufacturer = $(this).find('#aManufacturerName_' + tempItemID).val();
                //var tempConcatedColumnText = $(this).find('#aConcatedColumnText_' + tempItemID).val();
                var tempUOM = $(this).find('#aUOM_' + tempItemID).val();
                var tempCostUOM = $(this).find('#aCostUOM_' + tempItemID).val();
                var tmpCost = $(this).find('#aCost_' + tempItemID).val();

                var data = {
                    "ID": tempItemID,
                    "ItemNumber": tempItemNumber,
                    "Description": tempDescription,
                    "SellPrice": parseFloat(tmpSellPrice),
                    //"PackingQuantity": parseFloat(tmpPackingQuantity),
                    "ManufacturerPartNumber": tempManufacturerNumber,
                    "ImagePath": ImagePath,
                    "UPC": tempUPC,
                    "SupplierPartNumber": tempSupplierPartNo,
                    "SupplierName": tempSupplierName,
                    "ManufacturerName": tempManufacturer,
                    //"ConcatedColumnText": tempConcatedColumnText,
                    "UOM": tempUOM,
                    "CostUOM": tempCostUOM,
                    "Cost": parseFloat(tmpCost)
                };

                arrItems.push(data);

            });

        }
        else if (selectedModuleVal == ReturnOrderimportval) {

            $('#myImportDataTable tbody tr').each(function () {
                var tempQuickListID = $(this).find('#OrderID').text();
                var tempSupplier = $(this).find('#aSupplier_' + tempQuickListID).val();
                var tempOrderNumber = $(this).find('#aReturnOrderNumber_' + tempQuickListID).val();
                var tempRequiredDate = $(this).find('#aReturnedDate_' + tempQuickListID).val();
                var tempOrderStatus = $(this).find('#aReturnOrderStatus_' + tempQuickListID).val();
                var tempStagingName = $(this).find('#aStagingName').val();
                var tempOrderComment = $(this).find('#aReturnOrderComment').val();
                var tempPackSlipNumber = $(this).find('#aPackSlipNumber').val();
                var tempShippingTrackNumber = $(this).find('#aShippingTrackNumber').val();
                var tempOrderUdf1 = $(this).find('#aReturnOrderUDF1').val();
                var tempOrderUdf2 = $(this).find('#aReturnOrderUDF2').val();
                var tempOrderUdf3 = $(this).find('#aReturnOrderUDF3').val();
                var tempOrderUdf4 = $(this).find('#aReturnOrderUDF4').val();
                var tempOrderUdf5 = $(this).find('#aReturnOrderUDF5').val();
                var tempShipVia = $(this).find('#aShipVia').val();
                var tempOrderType = $(this).find('#aOrderType').val();
                var tempShippingVendor = $(this).find('#aShippingVendor').val();
                //var tempAccountNumber = $(this).find('#aAccountNumber').val();
                var tempSupplierAccount = $(this).find('#aSupplierAccount').val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempQuickListID).val();
                var tempBinNumber = $(this).find('#aBin_' + tempQuickListID).val();
                var tempRequestedQTY = $(this).find('#aRequestedReturnQty_' + tempQuickListID).val();
                var tempReceivedQty = $(this).find('#aReceivedReturnedQty_' + tempQuickListID).val();
                var tempASNNumber = $(this).find('#aASNNumber').val();
                var tempApprovedQty = $(this).find('#aApprovedQty_' + tempQuickListID).val();
                var tempInTransitQty = $(this).find('#aInTransitQty_' + tempQuickListID).val();
                var tempIsCloseItem = $(this).find('#aIsCloseItem').is(':checked');
                var tempLineNumber = $(this).find('#aLineNumber').val();
                var tempControlNumber = $(this).find('#aControlNumber').val();
                var tempItemComment = $(this).find('#aItemComment').val();
                var tempLineItemUDF1 = $(this).find('#aLineItemUDF1').val();
                var tempLineItemUDF2 = $(this).find('#aLineItemUDF2').val();
                var tempLineItemUDF3 = $(this).find('#aLineItemUDF3').val();
                var tempLineItemUDF4 = $(this).find('#aLineItemUDF4').val();
                var tempLineItemUDF5 = $(this).find('#aLineItemUDF5').val();
                var tempOrderCost = $(this).find('#aOrderCost_' + tempQuickListID).val();

                var data = {
                    "ID": tempQuickListID, "Supplier": tempSupplier, "OrderNumber": tempOrderNumber, "RequiredDate": tempRequiredDate, "OrderStatus": tempOrderStatus,
                    "StagingName": tempStagingName, "OrderComment": tempOrderComment, "PackSlipNumber": tempPackSlipNumber,
                    "ShippingTrackNumber": tempShippingTrackNumber, "OrderUdf1": tempOrderUdf1, "OrderUdf2": tempOrderUdf2, "OrderUdf3": tempOrderUdf3, "OrderUdf4": tempOrderUdf4, "OrderUdf5": tempOrderUdf5,
                    "ShipVia": tempShipVia, "OrderType": tempOrderType, "ShippingVendor": tempShippingVendor, //"AccountNumber": tempAccountNumber, 
                    "SupplierAccount": tempSupplierAccount, "ItemNumber": tempItemNumber, "Bin": tempBinNumber, "RequestedQTY": tempRequestedQTY,
                    "ReceivedQty": tempReceivedQty, "ASNNumber": tempASNNumber, "ApprovedQty": tempApprovedQty, "InTransitQty": tempInTransitQty,
                    "IsCloseItem": tempIsCloseItem, "LineNumber": tempLineNumber, "ControlNumber": tempControlNumber, "ItemComment": tempItemComment,
                    "LineItemUDF1": tempLineItemUDF1, "LineItemUDF2": tempLineItemUDF2, "LineItemUDF3": tempLineItemUDF3, "LineItemUDF4": tempLineItemUDF4,
                    "LineItemUDF5": tempLineItemUDF5,
                    "OrderCost": tempOrderCost
                };
                arrItems.push(data);
            });
        }

        else if (selectedModuleVal == CommonBOMToItemimportval) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempItemID = $(this).find('#BOMItemID').text();
                var tempItemNumber = $(this).find('#aBOMItemNumber_' + tempItemID).val();
                var tempRoomName = $(this).find('#aBOMRoomName_' + tempItemID).val();
                var BlanketOrderNumber = $(this).find('#aBlanketOrderNumber_' + tempItemID).val();
                var DefaultReorderQuantity = $(this).find('#aBOMDefaultReorderQuantity_' + tempItemID).val();
                var DefaultPullQuantity = $(this).find('#aBOMDefaultPullQuantity_' + tempItemID).val();
                var LeadTimeInDays = $(this).find('#aLeadTimeInDays_' + tempItemID).val();
                var BOMLink1 = $(this).find('#aBOMLink1_' + tempItemID).val();
                var BOMLink2 = $(this).find('#aBOMLink2_' + tempItemID).val();
                var Taxable = $(this).find('#aBOMTaxable_' + tempItemID).is(':checked');
                var Consignment = $(this).find('#aBOMConsignment_' + tempItemID).is(':checked');
                var ISNullConsignment = $(this).find('#aBOMConsignmentBlank_' + tempItemID).length > 0;
                var DefaultOnHandQuantity = $(this).find('#aBOMDefaultOnHandQuantity_' + tempItemID).val();
                var BOMDefaultCriticalQuantity = $(this).find('#aBOMDefaultCriticalQuantity_' + tempItemID).val();
                var BOMMinimumQuantity = $(this).find('#aBOMMinimumQuantity_' + tempItemID).val();
                var BOMMaximumQuantity = $(this).find('#aBOMMaximumQuantity_' + tempItemID).val();
                var WeightPerPiece = $(this).find('#aBOMWeightPerPiece_' + tempItemID).val();
                var ItemUniqueNumber = $(this).find('#aBOMItemUniqueNumber_' + tempItemID).val();
                var IsTransfer = $(this).find('#aBOMIsTransfer_' + tempItemID).is(':checked');
                var IsPurchase = $(this).find('#aBOMIsPurchase_' + tempItemID).is(':checked');
                var BOMInventryLocation = $(this).find('#aBOMInventryLocation_' + tempItemID).val();
                var BOMInventoryClassification = $(this).find('#aBOMInventoryClassification_' + tempItemID).val();
                var BOMSerialNumberTracking = $(this).find('#aBOMSerialNumberTracking_' + tempItemID).is(':checked');
                var BOMLotNumberTracking = $(this).find('#aBOMLotNumberTracking_' + tempItemID).is(':checked');
                var BOMDateCodeTracking = $(this).find('#aBOMDateCodeTracking_' + tempItemID).is(':checked');
                var ItemType = $(this).find('#aBOMItemType_' + tempItemID).val();
                var ImagePath = $(this).find('#aImagePath_' + tempItemID).val();
                var vUDF1 = $(this).find('#aBOMUDF1').val();
                var vUDF2 = $(this).find('#aBOMUDF2').val();
                var vUDF3 = $(this).find('#aBOMUDF3').val();
                var vUDF4 = $(this).find('#aBOMUDF4').val();
                var vUDF5 = $(this).find('#aBOMUDF5').val();
                var IsLotSerialExpiryCost = $(this).find('#aIsLotSerialExpiryCost_' + tempItemID).val();
                var ItemLevelMinMaxQtyRequired = $(this).find('#aBOMItemLevelMinMaxQtyRequired_' + tempItemID).is(':checked');
                var TrendingSetting = $(this).find('#atrendingsetting_' + tempItemID).val();
                var PullQtyScanOverride = $(this).find('#aBOMPullQtyScanOverride_' + tempItemID).is(':checked');
                var EnforceDefaultReorderQuantity = $(this).find('#aBOMEnforceDefaultReorderQuantity_' + tempItemID).is(':checked');
                var IsAutoInventoryClassification = $(this).find('#aIsAutoInventoryClassification_' + tempItemID).is(':checked');
                var IsBuildBreak = $(this).find('#aIsBuildBreak_' + tempItemID).is(':checked');
                var IsPackslipMandatoryAtReceive = $(this).find('#aIsPackslipMandatoryAtReceive_' + tempItemID).is(':checked');
                var tempItemImageExternalURL = $(this).find('#aBOMItemImageExternalURL_' + tempItemID).val();
                var tempItemDocExternalURL = $(this).find('#aBOMItemDocExternalURL_' + tempItemID).val();
                var ItemIsDeleted = $(this).find('#aItemIsDeleted_' + tempItemID).is(':checked');
                var ItemLink2ExternalURL = $(this).find('#aBOMItemLink2ExternalURL_' + tempItemID).val();
                var ItemIsActive = false;
                if ($(this).find('#aBOMItemIsActive_' + tempItemID).length > 0) {
                    ItemIsActive = $(this).find('#aBOMItemIsActive_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsActive = true;
                }
                var vUDF6 = $(this).find('#aUDF6').val(); 
                var vUDF7 = $(this).find('#aUDF7').val();
                var vUDF8 = $(this).find('#aUDF8').val();
                var vUDF9 = $(this).find('#aUDF9').val();
                var vUDF10 = $(this).find('#aUDF10').val();
                var ItemIsAllowOrderCostuom = false;
                if ($(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).length > 0) {
                    ItemIsAllowOrderCostuom = $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).is(':checked');
                }
                else {
                    ItemIsAllowOrderCostuom = false;
                }
                var eLabelKey = $(this).find('#aeLabelKey_' + tempItemID).val();
                var POItemLineNumber = $(this).find('#aPOItemLineNumber_' + tempItemID).val();

                var data = {
                    "ItemNumber": tempItemNumber, "RoomName": tempRoomName, "BlanketOrderNumber": BlanketOrderNumber, "DefaultReorderQuantity": DefaultReorderQuantity, "DefaultPullQuantity": DefaultPullQuantity, "LeadTimeInDays": LeadTimeInDays,
                    "Link1": BOMLink1, "Link2": BOMLink2, "Taxable": Taxable, "Consignment": Consignment, "ISNullConsignment": ISNullConsignment, "OnHandQuantity": DefaultOnHandQuantity, "CriticalQuantity": BOMDefaultCriticalQuantity, "MinimumQuantity": BOMMinimumQuantity,
                    "MaximumQuantity": BOMMaximumQuantity, "WeightPerPiece": WeightPerPiece, "ItemUniqueNumber": ItemUniqueNumber, "IsTransfer": IsTransfer, "IsPurchase": IsPurchase, "InventryLocation": BOMInventryLocation,
                    "InventoryClassificationName": BOMInventoryClassification, "SerialNumberTracking": BOMSerialNumberTracking, "LotNumberTracking": BOMLotNumberTracking, "DateCodeTracking": BOMDateCodeTracking, "ItemTypeName": ItemType,
                    "ImagePath": ImagePath, "UDF1": vUDF1, "UDF2": vUDF2, "UDF3": vUDF3, "UDF4": vUDF4, "UDF5": vUDF5, "UDF6": vUDF6, "UDF7": vUDF7, "UDF8": vUDF8, "UDF9": vUDF9, "UDF10": vUDF10,
                    "IsLotSerialExpiryCost": IsLotSerialExpiryCost, "IsItemLevelMinMaxQtyRequired": ItemLevelMinMaxQtyRequired, "TrendingSettingName": TrendingSetting, "PullQtyScanOverride": PullQtyScanOverride,
                    "IsEnforceDefaultReorderQuantity": EnforceDefaultReorderQuantity, "IsAutoInventoryClassification": IsAutoInventoryClassification, "IsBuildBreak": IsBuildBreak, "IsPackslipMandatoryAtReceive": IsPackslipMandatoryAtReceive,
                    "ItemImageExternalURL": tempItemImageExternalURL, "ItemDocExternalURL": tempItemDocExternalURL, "IsDeleted": ItemIsDeleted, "ItemLink2ExternalURL": ItemLink2ExternalURL, "IsActive": ItemIsActive,
                    "IsAllowOrderCostuom": ItemIsAllowOrderCostuom, "eLabelKey": eLabelKey, "POItemLineNumber": POItemLineNumber
                };
                arrItems.push(data);
            });
        }
        if (selectedModuleVal == PastMaintenanceDue) {
            $('#myImportDataTable tbody tr').each(function () {
                var tempINVLocID = $(this).find('#InvLocId').text();
                var tempSchedulerFor = $(this).find('#aSchedulerFor_' + tempINVLocID).val();
                var tempMaintenanceDate = $(this).find('#aMaintenanceDate_' + tempINVLocID).val();
                var tempAssetName = $(this).find('#aAssetName_' + tempINVLocID).val();
                var tempToolName = $(this).find('#aToolName_' + tempINVLocID).val();
                var tempSerial = $(this).find('#aSerial_' + tempINVLocID).val();
                var tempSchedulerName = $(this).find('#aSchedulerName_' + tempINVLocID).val();
                var tempItemNumber = $(this).find('#aItemNumber_' + tempINVLocID).val();
                var tempItemCost = $(this).find('#aItemCost_' + tempINVLocID).val();
                var tempQuantity = $(this).find('#aQuantity_' + tempINVLocID).val();
                var data = {
                    "ID": tempINVLocID, "ScheduleFor": tempSchedulerFor, "displayMaitenanceDate": tempMaintenanceDate,
                    "AssetName": tempAssetName, "ToolName": tempToolName,
                    "Serial": tempSerial, "SchedulerName": tempSchedulerName, "ItemNumber": tempItemNumber,
                    "ItemCost": tempItemCost, "Quantity": tempQuantity
                };
                arrItems.push(data);
            });
        }

        $('#ImportProcessing').modal();
        IDs = '';
        IDsWithLink2 = '';
        savedOnlyitemIds = '';
        savedItemIdsWithLink2 = '';
        allSuccesfulRecords = true;
        TotalCount = arrItems.length;
        ImportAjaxCall(arrItems, true, isLoadAllItems);

    });

    bindRoomList(selectedCompanyId, selectedRoomId);


});

var IDs = '';
var IDsWithLink2 = '';
var savedOnlyitemIds = '';
var savedItemIdsWithLink2 = '';
var saveGuidList = '';
var allSuccesfulRecords = true;
var DataBatch = new Array();
var guidList = '';
function ImportAjaxCall(_arrItems, IsFirstCall, isLoadAllItems) {

    var BatchSize = 50;
    var BatchIndex = 0;
    var isImgZipAvail = false;
    var isLink2ZipAvail = false;
    if (CurModuleval == 'CommonBOMToItem') {
        BatchSize = 200;
    }
    
    if ($('#uploadZIPFile').val() != '' && $('#uploadZIPFile').val() != null) {
        isImgZipAvail = true;
    }
    if ($('#uploadZIPFileForLink2').val() != '' && $('#uploadZIPFileForLink2').val() != null) {
        isLink2ZipAvail = true;
    }

    if (isLoadAllItems != "undefined" && isLoadAllItems != undefined && isLoadAllItems != null && isLoadAllItems == true) {
        BatchSize = _arrItems.length;
    }
    else {
        BatchSize = (BatchSize < _arrItems.length ? BatchSize : _arrItems.length);
    }
    DataBatch = [];

    for (BatchIndex = 0; BatchIndex < BatchSize; BatchIndex++) {
        DataBatch.push(_arrItems[0]);
        _arrItems.splice(0, 1);
    }
    $.ajax({
        url: SaveImportURL,
        type: 'POST',
        data: { 'TableName': CurModuleval, 'para': JSON.stringify(DataBatch), 'HasMoreRecords': true, 'IsFirstCall': IsFirstCall, 'TotalCount': TotalCount, 'isImgZipAvail': isImgZipAvail, 'isLink2ZipAvail': isLink2ZipAvail, 'importRoomIds': getSelectedID('ddlImportRoom') },
        cache: false,
        timeout: 0,
        success: function (response) {
            savedOnlyitemIds = savedOnlyitemIds + ',' + response.savedOnlyitemIds;
            savedItemIdsWithLink2 = savedItemIdsWithLink2 + ',' + response.savedItemIdsWithLink2;
            saveGuidList = saveGuidList + ',' + response.savedItemGuids;
            var selectedVal = $('#ddlModule option:selected').attr('value');
            if (selectedVal == ItemMasterselval || selectedVal == EditItemMasterselval) {
                SetItemCountInMenu();
            }

            if (response.allSuccesfulRecords == false) {
                //alert('This is testing');
                allSuccesfulRecords = false;

            }
            if (_arrItems.length <= 0) {
                oTable.fnDraw();
                //$('div#target').fadeToggle();
                IDs = savedOnlyitemIds;
                IDsWithLink2 = savedItemIdsWithLink2;
                guidList = saveGuidList;
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime, function () {
                    CheckValidation(true, allSuccesfulRecords);
                    $('#btnSave').removeProp("disabled");
                    $('#DivLoading').hide();
                });
                if (selectedVal == ItemMasterselval
                    || selectedVal == EditItemMasterselval
                    || selectedVal == Assetsselval
                    || selectedVal == ToolMasterselval
                    || selectedVal == SupplierMasterselval
                    || selectedVal == ToolCertificationImagesval) {
                    if (allSuccesfulRecords == true) {              
                        //$("input#uploadZIPFile,").hide();
                        //$("input#uploadZIPFileForLink2").hide();
                        $("span#Imapth,span#Link2,#CustomuploadZIPFile,#CustomuploadZIPFileForLink2").hide();
                        //$("span#Link2").hide();
                        //CustomuploadZIPFile,CustomuploadZIPFileForLink2
                    }
                    else {
                        //$("input#uploadZIPFile").show();
                        //$("input#uploadZIPFileForLink2").show();
                        //$("span#Imapth").hide();
                        //$("span#Link2").hide();
                        $("span#Imapth,span#Link2,#CustomuploadZIPFile,#CustomuploadZIPFileForLink2").show();
                    }
                }
                //$("input#uploadZIPFile").hide();
                //$("input#uploadZIPFileForLink2").hide();
                if (selectedVal == ItemMasterselval || selectedVal == EditItemMasterselval) {
                    $.ajax({
                        url: BinChangeHistoryURL,
                        type: 'POST',
                        data: { 'ItemGUID': guidList },
                        timeout: 0,
                        success: function (responseHistory) {

                            var historyString = "";
                            if (responseHistory.errorMessage == "") {
                                if (responseHistory.isRecordAvail == true) {
                                    $.each(responseHistory.historyData, function (i, val) {
                                        historyString += WarnNewBinCreatedRes.replace("{0}", val.ItemNumber).replace("{1}", "<b>" + val.BinNumber + "</b>") + "</br>";
                                    });

                                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color:green'>" + MsgImportedSuccessfully + "</b></br>" + historyString);
                                    closeImportInfoModel();
                                    $('#ImportInfoDialog').modal();
                                }
                                else {
                                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color:green'>" + MsgImportedSuccessfully + "</b>");
                                    closeImportInfoModel();
                                    $('#ImportInfoDialog').modal();
                                }
                            }
                            else {
                                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color:green'>" + MsgImportedSuccessfully + "</b>");
                                closeImportInfoModel();
                                $('#ImportInfoDialog').modal();
                            }
                        }
                    });
                }
                else {
                    $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color:green'>" + MsgImportedSuccessfully + "</b>");
                    closeImportInfoModel();
                    $('#ImportInfoDialog').modal();
                }
                //                            $("#spanGlobalMessage").text(response.Message);
                //                            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            }
            else {
                ImportAjaxCall(_arrItems, false);
            }
        },
        error: function (xhr, status, error) {
            if (_arrItems.length <= 0) {
                var FinalErr = xhr.responseText;

                //$('#ImportInfoDialog').find("#ImportMSG").html(FinalErr);
                $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + MsgImportError + "</b>");
                closeImportInfoModel();
                $('#ImportInfoDialog').modal();
                //                            $("#spanGlobalMessage").text(message.statusText);
                //                            $('div#target').fadeToggle();
                //                            $("div#target").delay(2000).fadeOut(200);
                $('#btnSave').removeProp("disabled");
                //$('#DivLoading').hide();
            }
            else {
                ImportAjaxCall(_arrItems, false);
            }
        },
        complete: function () {

            if (_arrItems.length <= 0) {
                var tmpselectedVal = $('#ddlModule option:selected').attr('value');
                if (tmpselectedVal == ItemMasterselval
                    || tmpselectedVal == EditItemMasterselval
                    || tmpselectedVal == Assetsselval
                    || tmpselectedVal == ToolMasterselval
                    || tmpselectedVal == SupplierMasterselval
                    || tmpselectedVal == BOMItemMasterselval
                    || tmpselectedVal == ToolCertificationImagesval
                    || tmpselectedVal == SupplierCatalogVal) {
                    if (document.getElementById("uploadZIPFile").value != '') {
                        uploadZIPFile(IDs, tmpselectedVal);
                    }
                    if (tmpselectedVal == ItemMasterselval
                        || tmpselectedVal == EditItemMasterselval
                        || tmpselectedVal == BOMItemMasterselval) {
                        if (document.getElementById("uploadZIPFileForLink2").value != '') {
                            uploadZIPFileLink2(IDsWithLink2, tmpselectedVal);
                        }
                    }
                }
            }
        }
    });
}

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
        else if (selectedModuleVal == PullImportWithSameQty) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('InvLocId');
                var tempToolCategory = $(this).find('#aItemNumber_' + id).val();

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
        else if (selectedModuleVal == ItemMasterselval || selectedModuleVal == EditItemMasterselval) {
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
            if (typeof checkRestrictedCharchterList !== "undefined" && typeof checkRestrictedCharchterList == 'function') {
                checkRestrictedCharchterList();
            }
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
        } else if (selectedModuleVal == PastMaintenanceDue) {
            $('#myImportDataTable tbody tr').each(function () {
                var id = $(this).attr('id');
                var aMaintenanceDate = $(this).find('#aMaintenanceDate_' + id).val();
                if (aMaintenanceDate == "") {
                    allSuccessRecords = false;
                    $(this).find('#aMaintenanceDate_' + id).addClass("importtext-box validationerror");
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
            $.post('/ImportMultiRoom/ResetSession',
                { key: "CurModulevalue", value: AssetMasterval }, function (data) {

                    $('#myImportDataTable').html('');

                    //    $("select#ddlModule")[0].selectedIndex = 0;
                    //    $("select#ddlModule").attr('selected', true);

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

function isNumberWithDecimalKey(evt) {


    var keyCode = [evt.keyCode || evt.which];
    if (keyCode == 8 || keyCode == 13 || keyCode == 9 || keyCode == 46)
        return true;
    if (keyCode > 57 || keyCode < 48)
        return false;
    else
        return true;
}

function CheckFileName() {

    var fileName = document.getElementById("uploadFile").value;
    if (fileName == "") {
        ShowNotificationMessage(ImportBrowseFileMsg);
        //alert(ImportBrowseFileMsg);
        return false;
    }
    var strfilename = fileName.split(".");
    if (strfilename[strfilename.length - 1].toUpperCase() == "CSV" || strfilename[strfilename.length - 1].toUpperCase() == "XLS")
        return true;
    else {
        ShowNotificationMessage(MsgInvalidFileValidation.replace("{0}", filename));
        return false;
    }
    return true;
}
function CheckZIPFileName() {
    if ($('#ddlModule option:selected').attr('value') == ItemMasterselval || $('#ddlModule option:selected').attr('value') == EditItemMasterselval) {
        var fileName = document.getElementById("uploadZIPFile").value;
        if (fileName == "") {
            return true;
        }
        var strfilename = fileName.split(".");
        if (strfilename[strfilename.length - 1].toUpperCase() == "ZIP")
            return true;
        else {
            fileName = $('#uploadZIPFile')[0].files[0].name;
            ShowNotificationMessage(MsgInvalidFileZipValidation.replace("{0}", fileName))
            document.getElementById("uploadZIPFile").value = '';
            $("#lblnouploadedZIPFile").html(textNofilechosen);
            return false;
        }
    }
    return true;
}

function CheckZIPFileNameForToolCertificationImages() {
    if ($('#ddlModule option:selected').attr('value') == ToolCertificationImagesval) {
        var fileName = document.getElementById("uploadZIPFile").value;
        if (fileName == "") {
            $('#DivLoading').hide();
            $('#btnSave').removeProp("disabled");
            closeImportInfoModel();
            $('#ImportInfoDialog').modal();
            $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + msgZipFileIsRequired + "</b>");
            return false;
            //return true;
        }
        var strfilename = fileName.split(".");
        if (strfilename[strfilename.length - 1].toUpperCase() == "ZIP")
            return true;
        else {
            //alert("File with " + fileName + " is invalid. Upload a valid file with zip extension");
            //document.getElementById("uploadZIPFile").value = '';
            $('#DivLoading').hide();
            $('#btnSave').removeProp("disabled");
            closeImportInfoModel();
            $('#ImportInfoDialog').modal();
            $('#ImportInfoDialog').find("#ImportMSG").html("<b style='color: Red;'>" + msgInvalidFileType.replace("{0}", fileName) + "</b>");
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
function ValidateConsignedItem() {
    var checkval = false;
    var i = 1;
    $('#myImportDataTable tbody tr').each(function () {

        var strmsg = '';
        var tempItemID = $(this).find('#ItemID').text();
        var Consignment = $(this).find('#aConsignment_' + tempItemID).is(':checked');
        if (!Consignment) {
            $(this).find('#aConsignment_' + tempItemID).css('border-color', '#DDD');
        }
        else {
            $(this).find('#' + tempItemID).css('border-color', 'red');
            $(this).find('#aConsignment_' + tempItemID).css('border-color', 'red');

            checkval = true;

            strmsg = ConsignedItemMessage;
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
function ValidateOrderCostUOMItem() {

    var checkval = false;
    var i = 1;
    $('#myImportDataTable tbody tr').each(function () {

        var strmsg = '';
        var tempItemID = $(this).find('#ItemID').text();
        var IsAllowOrderCostuom = $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).is(':checked');
        if (!IsAllowOrderCostuom) {
            $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).css('border-color', '#DDD');
        }
        else {
            $(this).find('#' + tempItemID).css('border-color', 'red');
            $(this).find('#aItemIsAllowOrderCostuom_' + tempItemID).css('border-color', 'red');

            checkval = true;

            strmsg = OrderCostuomItem;
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
function BOMValidateCriticalMinMax() {
    var checkval = false;
    var i = 1;
    $('#myImportDataTable tbody tr').each(function () {
        var strmsg = '';
        var tempItemID = $(this).find('#BOMItemID').text();
        var CriticalQuantity = $(this).find('#aBOMCriticalQuantity_' + tempItemID).val();
        var MinimumQuantity = $(this).find('#aBOMMinimumQuantity_' + tempItemID).val();
        var MaximumQuantity = $(this).find('#aBOMMaximumQuantity_' + tempItemID).val();
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
            $(this).find('#aBOMCriticalQuantity_' + tempItemID).css('border-color', '#DDD');
            $(this).find('#aBOMMinimumQuantity_' + tempItemID).css('border-color', '#DDD');

        }
        else {
            $(this).find('#aBOMCriticalQuantity_' + tempItemID).css('border-color', 'red');
            $(this).find('#aBOMMinimumQuantity_' + tempItemID).css('border-color', 'red');

            checkval = true;

            strmsg = minmaxmsg;
        }
        if ((parseFloat(MinimumQuantity) <= parseFloat(MaximumQuantity))) {

            $(this).find('#aBOMMinimumQuantity_' + tempItemID).css('border-color', '#DDD');
            $(this).find('#aBOMMaximumQuantity_' + tempItemID).css('border-color', '#DDD');
        }
        else {

            $(this).find('#aBOMMinimumQuantity_' + tempItemID).css('border-color', 'red');
            $(this).find('#aBOMMaximumQuantity_' + tempItemID).css('border-color', 'red');
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
            $.trim($(val1).find('input[id^="aSerial_"]').val()) + '###' +
            $.trim($(val1).find('input[id^="aToolName_"]').val())
            // $(val1).find('input[id^="aTechnician_"]').val()


        );
    });

    // Sort it
    values.sort();

    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {

        var prevSerial = $.trim(values[k].split('###')[0]);
        var currentSerial = $.trim(values[k - 1].split('###')[0]);
        var prevTool = $.trim(values[k].split('###')[1]);
        var currentTool = $.trim(values[k - 1].split('###')[1]);
        var RowNumber = $.trim(values[k].split('###')[2]);
        if (prevSerial == currentSerial && prevTool != currentTool) {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $.trim($(this).find('input[id^="aSerial_"]').val());

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
                    $(this).find('#Reason').html(MsgEnterUniqueBinNymber);
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
function ValidateSupplierAccountDetails() {
    // Collect all values in an array
    var valuesSupplierDetails = [];
    var invalidSupplierDetails = [];

    var valueSupplierName = [];


    $('#myImportDataTable tbody tr td input[id^="aSupplier_"]').each(function (idx, val) {
        var valueSupplierIsVendor = $(this).parent().parent().find('input[id^="aIsSendtoVendor_"]').is(':checked');
        if (valueSupplierIsVendor == true) {
            if (valueSupplierName.length > 0) {
                if (valueSupplierName.filter(function (supplier) { return supplier == $(val).val() }).length <= 0) {
                    valueSupplierName.push($(val).val());
                }
            }
            else {
                valueSupplierName.push($(val).val());
            }
        }
    });

    for (var i = 0; i < valueSupplierName.length; i++) {

        var DefaultAccountCount = 0;
        var isValidDefaulAccount = false;

        $('#myImportDataTable tbody tr td input[id^="aSupplier_"]').each(function (idx, val) {
            if ($(val).val() == valueSupplierName[i]) {
                var valueSupplierIsVendor = $(this).parent().parent().find('input[id^="aIsSendtoVendor_"]').is(':checked');
                var valueSupplierAccountIsDefault = $(this).parent().parent().find('input[id^="aAccountIsDefault_"]').is(':checked');
                if (valueSupplierIsVendor == true && valueSupplierAccountIsDefault == true) {
                    isValidDefaulAccount = true;
                    DefaultAccountCount++;
                    if (DefaultAccountCount > 1) {
                        if (ErrorLineNumber != '') {
                            ErrorLineNumber += "," + idx;
                        }
                        else {
                            ErrorLineNumber = idx;
                        }
                        $($('#myImportDataTable tbody tr')[idx]).css('background-color', 'pink');
                        $($('#myImportDataTable tbody tr')[idx]).find('input[id^="aSupplier_"]').css('border-color', 'red');
                        $($('#myImportDataTable tbody tr')[idx]).find('#Reason').html(MsgSupplierDefaultAccountValidation);
                        $($('#myImportDataTable tbody tr')[idx]).find('#Status').html('Fail');

                        invalidSupplierDetails.push({ SupplierName: valueSupplierName[i] });
                    }
                }
            }
        });

        if (isValidDefaulAccount == false) {

            $('#myImportDataTable tbody tr td input[id^="aSupplier_"]').each(function (idx, val) {
                if ($(val).val() == valueSupplierName[i]) {
                    if (ErrorLineNumber != '') {
                        ErrorLineNumber += "," + idx;
                    }
                    else {
                        ErrorLineNumber = idx;
                    }
                    $($('#myImportDataTable tbody tr')[idx]).css('background-color', 'pink');
                    $($('#myImportDataTable tbody tr')[idx]).find('input[id^="aSupplier_"]').css('border-color', 'red');
                    $($('#myImportDataTable tbody tr')[idx]).find('#Reason').html(MsgSelectDefaultAccount);
                    $($('#myImportDataTable tbody tr')[idx]).find('#Status').html('Fail');

                    invalidSupplierDetails.push({ SupplierName: valueSupplierName[i] });
                    return false;
                }
            });
        }
    }

    if (invalidSupplierDetails.length > 0) {
        return false;
    }
    else {
        return true;
    }
}

function isUniqueItemAndSupplierNumber() {
    var existing = [];
    var isError = false;
    $('#myImportDataTable tbody tr').each(function () {
        var i = 1;
        var suppliernumber = $(this).find('input[id^="aSupplierNumber_"]').val();
        var itemnumber = $(this).find('input[id^="aItemNumber_"]').val();
        var isavailable = existing.filter(function (item) {
            return item.suppliernumber == suppliernumber && item.itemnumber === itemnumber
        });
        if (isavailable.length > 0) {
            if (ErrorLineNumber != '') {
                ErrorLineNumber += "," + i;
            }
            else {
                ErrorLineNumber = i;
            }
            $(this).closest('tr').css('background-color', 'pink');
            $(this).find('input[id^="aSupplierNumber_"]').css('border-color', 'red');
            $(this).find('#Reason').html(MsgEnterUniqueSupplierNumber);
            $(this).find('#Status').html('Fail');
            isError = true;
        } else {
            $(this).find('input[id^="aSupplierNumber_"]').css('border-color', '#DDD');
            if ($(this).attr("class") == 'even') {
                $(this).css("background-color", "#FFF");
            }
            else {
                $(this).css("background-color", "#EFF7FF");
            }
        }
        i++;
        existing.push({ suppliernumber, itemnumber });
    });
    return isError;
}

//function isUniqueSupplierPartNumber() {
//    // Collect all values in an array
//    var values = [];
//    $('#myImportDataTable tbody tr td input[id^="aSupplierNumber_"]').each(function (idx, val) { values.push($(val).val()); });

//    // Sort it
//    values.sort();

//    // Check whether there are two equal values next to each other
//    for (var k = 1; k < values.length; ++k) {
//        if (values[k] == values[k - 1]) {
//            var serialNumber = '';
//            var existing = [];
//            var i = 1;
//            var duplicates = $('#myImportDataTable tbody tr').each(function () {
//                var value = $(this).find('input[id^="aSupplierNumber_"]').val();

//                if (existing.indexOf(value) >= 0) {
//                    if (ErrorLineNumber != '') {
//                        ErrorLineNumber += "," + i;
//                    }
//                    else {
//                        ErrorLineNumber = i;
//                    }
//                    $(this).closest('tr').css('background-color', 'pink');
//                    $(this).find('input[id^="aSupplierNumber_"]').css('border-color', 'red');
//                    $(this).find('#Reason').html(MsgEnterUniqueSupplierNumber);
//                    $(this).find('#Status').html('Fail');
//                }
//                else {
//                    $(this).find('input[id^="aSupplierNumber_"]').css('border-color', '#DDD');
//                    if ($(this).attr("class") == 'even') {
//                        $(this).css("background-color", "#FFF");
//                    }
//                    else {
//                        $(this).css("background-color", "#EFF7FF");
//                    }
//                }
//                i++;
//                existing.push(value);
//            });

//            return false;
//        }
//    }
//    return true;
//}

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

        if ($.trim(tempaModule).toLowerCase() == "assets" || $.trim(tempaModule).toLowerCase() == "item master" || $.trim(tempaModule).toLowerCase() == "tool master") {

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
            strmsg = MsgModuleNameValidation;
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

        var tempSerial = $.trim($(this).find('#aSerial_' + tempToolID).val());

        if (tempSerial == null || tempSerial == '') {
            if (toolNameBlankSerial != '') {
                if (tempToolName != toolNameBlankSerial) {
                    $(this).find('#aSerial_' + tempToolID).css('border-color', 'red');
                    checkval = true;
                    strmsg = MsgSerialblankValidation.replace("{0}", toolNameBlankSerial);
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

function is_valid_filename(uploadedfilename) {
    if (uploadedfilename != '' && uploadedfilename != null) {
        var regexForSpecialCharNotAllowed = /\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\[|\{|\]|\}|\||\\|\'|\<|\,|\>|\?|\/|\""|\;|\:/;
        var IsValidFileName = regexForSpecialCharNotAllowed.test(uploadedfilename);
        if (IsValidFileName == true) {
            return false;
        }
        else {
            var regex = /\.(jpe?g|png|gif|bmp)$/i;
            var validFileName = regex.test(uploadedfilename);
            if (validFileName == true) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}

function is_valid_Link2filename(uploadedfilename) {
    if (uploadedfilename != '' && uploadedfilename != null) {
        var regexForSpecialCharNotAllowed = /\`|\!|\@|\#|\$|\%|\^|\&|\*|\(|\)|\+|\=|\[|\{|\]|\}|\||\\|\'|\<|\,|\>|\?|\/|\""|\;|\:/;
        var IsValidFileName = regexForSpecialCharNotAllowed.test(uploadedfilename);
        if (IsValidFileName == true) {
            return false;
        }
        else {
            var regex = /\.(jpe?g|png|gif|bmp|pdf|doc|xls|docx|xlsx)$/i;
            var validFileName = regex.test(uploadedfilename);
            if (validFileName == true) {
                return true;
            }
            else {
                return false;
            }
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

function uploadZIPFile(IDs, tableName) {

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
                data: { 'ItemIDs': IDs, 'TableName': tableName },
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

function uploadZIPFileLink2(IDsWithLink2, tableName) {
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
                data: { 'IDsWithLink2': IDsWithLink2, 'TableName': tableName },
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

                strmsg = MsgPurchaseDateFormat;
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

    if ($("#btnAllRecordDownload") !== undefined) {
        if (selectedVal == ToolCheckInCheckOut
            || selectedVal == PullImport
            || selectedVal == PullImportWithLotSerialVal
            || selectedVal == PullImportWithSameQty
            || selectedVal == ToolCertificationImagesval
            || selectedVal == MoveMaterialval
            || selectedVal == EnterpriseQuickListval
            || selectedVal == Requisitionval
            || selectedVal == SupplierCatalogVal
            || selectedVal == CommonBOMToItemimportval) {
            $("#btnAllRecordDownload").hide();
        }
        else {
            $("#btnAllRecordDownload").show();
        }
    }

    if (selectedVal == OrderMasterselval || selectedVal == BinMasterselval || selectedVal == ManualCountval
        || selectedVal == MoveMaterialval || selectedVal == PullImport || selectedVal == PullImportWithLotSerialVal
        || selectedVal == PullImportWithSameQty || selectedVal == ItemLocationChangeImport || selectedVal == ToolCheckInCheckOut
        || selectedVal == WorkOrderval || selectedVal == ToolMasterselval || selectedVal == AssetToolScheduler
        || selectedVal == AssetToolSchedulerMappingval || selectedVal == Assetsselval || selectedVal == BarcodeMasterselval
        || selectedVal == CategoryMasterselval || selectedVal == BOMItemMasterselval || selectedVal == CostUOMMasterselval
        || selectedVal == CustomerMasterselval || selectedVal == GLAccountsMasterselval || selectedVal == InventoryClassificationMasterselval
        || selectedVal == eVMISetupselval || selectedVal == ItemManufacturerselval || selectedVal == ItemLocationQtyval
        || selectedVal == ItemSupplierselval || selectedVal == Kitsselval || selectedVal == ManufacturerMasterselval
        || selectedVal == MeasurementTermMasterselval || selectedVal == PastMaintenanceDue || selectedVal == ProjectMasterselval
        || selectedVal == QuickListPermissionselval || selectedVal == ShipViaMasterselval || selectedVal == SupplierMasterselval
        || selectedVal == TechnicianMasterselval || selectedVal == ToolAdjustmentCountval || selectedVal == ToolCategoryselval
        || selectedVal == LocationMasterselval || selectedVal == UDFselval || selectedVal == UnitMasterselval || selectedVal == ReturnOrderimportval
        || selectedVal == CommonBOMToItemimportval) {
        $("#dvOffline").show();
    }
    else
        $("#dvOffline").hide();

    if (selectedVal == CommonBOMToItemimportval) {
        $("#divddlImportRoom").hide();
        //$("#divddlImportRoomBlank").show();
    }
    else {
        $("#divddlImportRoom").show();
        //$("#divddlImportRoomBlank").hide();
    }
    if (selectedVal != "") {
        var _isGridFill = false;

        if ($('#myImportDataTable tbody tr').length > 0 || $('#myImportDataTable thead tr').length > 0) {
            _isGridFill = true;
        }

        $.ajax({
            url: SetSelectedModuleURL,
            type: 'POST',
            dataType: 'json',
            data: "{CurModule:'" + selectedVal + "',isGridFill:" + _isGridFill + " }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (_isGridFill) {
                    //location.reload(true);
                    window.location.href = ImportLoadURL;
                    return false;
                }
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
        //alert(ColumnObject);
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

                if ((typeof (o)) != "undefined" && o != undefined && o != null) {
                    o.iLength = 2147483647;
                }
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
                    "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
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
    var isvalid = true;
    //$('#myImportDataTable tbody tr td input[id^="aSerial_"]').each(function (idx, val) { values.push($(val).val()); });
    var i = 1;
    $('#myImportDataTable tbody tr').each(function (idx, val1) {
        // if ($(val1).find('input[id^="aBinNumber_"]').val() != '') {
        if ($.trim($(val1).find('input[id^="aSerialNumber_"]').val()) != '') {
            values.push(
                $(val1).find('input[id^="aItemNumber_"]').val() + '###' +
                $.trim($(val1).find('input[id^="aSerialNumber_"]').val())
                // $(val1).find('input[id^="aTechnician_"]').val()
            );

            var tempINVLocID = $(this).find('#InvLocId').text();
            var tempcustomerownedQTY = $(this).find('#acustomerownedquantity_' + tempINVLocID).val();
            var tempconsignedQTY = $(this).find('#aconsignedquantity_' + tempINVLocID).val();
            if (tempcustomerownedQTY <= 0 && tempconsignedQTY <= 0) {
                if (ErrorLineNumber != '') {
                    ErrorLineNumber += "," + i;
                }
                else {
                    ErrorLineNumber = i;
                }
                $(this).closest('tr').css('background-color', 'pink');
                $(this).find('input[id^="aSerialNumber_"]').css('border-color', 'red');
                $(this).find('#Reason').html(MsgEnterQuantitySerialTypeItem);
                $(this).find('#Status').html('Fail');
                isvalid = false;
            }
            if (tempcustomerownedQTY > 1) {
                if (ErrorLineNumber != '') {
                    ErrorLineNumber += "," + i;
                }
                else {
                    ErrorLineNumber = i;
                }
                $(this).closest('tr').css('background-color', 'pink');
                $(this).find('input[id^="aSerialNumber_"]').css('border-color', 'red');
                $(this).find('#Reason').html(MsgEnterCustomerOwnQuantity);
                $(this).find('#Status').html('Fail');
                isvalid = false;
            }
            if (tempconsignedQTY > 1) {
                if (ErrorLineNumber != '') {
                    ErrorLineNumber += "," + i;
                }
                else {
                    ErrorLineNumber = i;
                }
                $(this).closest('tr').css('background-color', 'pink');
                $(this).find('input[id^="aSerialNumber_"]').css('border-color', 'red');
                $(this).find('#Reason').html(MsgEnterConsignedOwnQuantity);
                $(this).find('#Status').html('Fail');
                isvalid = false;
            }

            if (isvalid == true) {
                $(this).find('input[id^="aBinNumber_"]').css('border-color', '#DDD');
                if ($(this).attr("class") == 'even') {
                    $(this).css("background-color", "#FFF");
                }
                else {
                    $(this).css("background-color", "#EFF7FF");
                }
                $(this).find('#Reason').html('N/A');
                $(this).find('#Status').html('N/A');
            }
        }

        i++;
        //  }
    });
    // Sort it
    values.sort();
    // Check whether there are two equal values next to each other
    for (var k = 1; k < values.length; ++k) {
        var prevItemNumber = values[k].split('###')[0];
        var currentItemNumber = values[k - 1].split('###')[0];
        var prevSerial = $.trim(values[k].split('###')[1]);
        var currentSerial = $.trim(values[k - 1].split('###')[1]);


        if (prevSerial == currentSerial && prevItemNumber == currentItemNumber && prevSerial != '' && currentSerial != '') {
            var serialNumber = '';
            var existing = [];
            var i = 1;
            var duplicates = $('#myImportDataTable tbody tr').each(function () {
                var value = $(this).find('input[id^="aItemNumber_"]').val() + '###' + $.trim($(this).find('input[id^="aSerialNumber_"]').val());
                if ($.trim($(this).find('input[id^="aSerialNumber_"]').val()) != '') {



                    if (existing.indexOf(value) >= 0) {
                        if (ErrorLineNumber != '') {
                            ErrorLineNumber += "," + i;
                        }
                        else {
                            ErrorLineNumber = i;
                        }
                        $(this).closest('tr').css('background-color', 'pink');
                        $(this).find('input[id^="aSerialNumber_"]').css('border-color', 'red');
                        $(this).find('#Reason').html(MsgEnterUniqueSerial);
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
                }
                existing.push(value);
                i++;
            });

            return false;
        }
    }

    if (isvalid == false)
        return false;
    return true;
}

$(document).ready(function () {
    var selectedVal = $('#ddlModule option:selected').attr('value');
    if ($("#btnAllRecordDownload") !== undefined) {
        if (selectedVal == ToolCheckInCheckOut
            || selectedVal == PullImport
            || selectedVal == PullImportWithLotSerialVal
            || selectedVal == PullImportWithSameQty
            || selectedVal == ToolCertificationImagesval
            || selectedVal == MoveMaterialval
            || selectedVal == EnterpriseQuickListval
            || selectedVal == Requisitionval
            || selectedVal == SupplierCatalogVal
            || selectedVal == CommonBOMToItemimportval) {
            $("#btnAllRecordDownload").hide();
        }
        else {
            $("#btnAllRecordDownload").show();
        }
    }

    if (selectedVal == OrderMasterselval || selectedVal == BinMasterselval || selectedVal == ManualCountval
        || selectedVal == MoveMaterialval || selectedVal == PullImport || selectedVal == PullImportWithLotSerialVal
        || selectedVal == PullImportWithSameQty || selectedVal == ItemLocationChangeImport || selectedVal == ToolCheckInCheckOut
        || selectedVal == WorkOrderval || selectedVal == ToolMasterselval || selectedVal == AssetToolScheduler
        || selectedVal == AssetToolSchedulerMappingval || selectedVal == Assetsselval || selectedVal == BarcodeMasterselval
        || selectedVal == CategoryMasterselval || selectedVal == BOMItemMasterselval || selectedVal == CostUOMMasterselval
        || selectedVal == CustomerMasterselval || selectedVal == GLAccountsMasterselval || selectedVal == InventoryClassificationMasterselval
        || selectedVal == eVMISetupselval || selectedVal == ItemManufacturerselval || selectedVal == ItemLocationQtyval
        || selectedVal == ItemSupplierselval || selectedVal == Kitsselval || selectedVal == ManufacturerMasterselval
        || selectedVal == MeasurementTermMasterselval || selectedVal == PastMaintenanceDue || selectedVal == ProjectMasterselval
        || selectedVal == QuickListPermissionselval || selectedVal == ShipViaMasterselval || selectedVal == SupplierMasterselval
        || selectedVal == TechnicianMasterselval || selectedVal == ToolAdjustmentCountval || selectedVal == ToolCategoryselval
        || selectedVal == LocationMasterselval || selectedVal == UDFselval || selectedVal == UnitMasterselval || selectedVal == ReturnOrderimportval
        || selectedVal == CommonBOMToItemimportval) {
        $("#dvOffline").show();
    }
    else
        $("#dvOffline").hide();

    if (selectedVal == CommonBOMToItemimportval) {
        $("#divddlImportRoom").hide();
        //$("#divddlImportRoomBlank").show();
    }
    else {
        $("#divddlImportRoom").show();
        //$("#divddlImportRoomBlank").hide();
    }
});

function ShowHideControl() {
    if ($('#isOffline').is(':checked')) { $('#btnimport').show(); $('#btnupload').hide(); }
    else { $('#btnimport').hide(); $('#btnupload').show(); }
}

$('#uploadFile').change(function () {
    var fileName = $('#uploadFile')[0].files[0].name;
    $("#lblnofilechoosen").html(fileName);
});


$('#uploadZIPFile').change(function () {
    var fileName = $('#uploadZIPFile')[0].files[0].name;
    $("#lblnouploadedZIPFile").html(fileName);
});


$('#uploadZIPFileForLink2').change(function () {
    var fileName = $('#uploadZIPFileForLink2')[0].files[0].name;
    var strfilename = fileName.split(".");

    if (strfilename[strfilename.length - 1].toUpperCase() == "ZIP") {
        $("#lblnouploadedZIPFileForLink2").html(fileName);
        return true;
    }
    else {
        ShowNotificationMessage(MsgInvalidFileZipValidation.replace("{0}", fileName))
        document.getElementById("uploadZIPFileForLink2").value = '';
        $("#lblnouploadedZIPFileForLink2").html(textNofilechosen);
        return false;
    }

});

function bindRoomList(objCompanyID, objExistSelected) {


    var objList = '';
    var objSelect = '';
    var objExistselect = objExistSelected.split(',');

    $.ajax({
        type: "POST",
        url: "/ReportBuilder/GetRoomList",
        data: JSON.stringify({ 'CompanyID': objCompanyID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (message) {
            objList = message.RoomList;
            objSelect = message.Selected;
        },
        error: function (response) {

        }
    });

    $("#ddlImportRoom > option").remove();
    $("#ddlImportRoom").multiselect("refresh");
    var s = '';
    for (var i = 0; i < objList.length; i++) {
        s += '<option value=' + objList[i].key + '  >' + objList[i].value + '</option>';
    }
    $("#ddlImportRoom").append(s);
    $("#ddlImportRoom").multiselect({
        noneSelectedText: 'Room', selectedList: 5,
        selectedText: function (numChecked, numTotal, checkedItems) {
            return 'Room' + ' ' + numChecked + ' ' + selected;
        }
    }).unbind("multiselectclick multiselectcheckall multiselectuncheckall").bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
        if (ui.checked) {
            $("#ddlImportRoom").multiselect("widget").find(":checkbox[value='" + ui.value + "']").attr("checked", "checked");
            $("#ddlImportRoom option[value='" + ui.value + "']").attr("selected", 1);
        }
        $("#selectedRoomsToImport").val(getSelectedID('ddlImportRoom'));
    });
    $("#ddlImportRoom").multiselect("refresh");
    if (objExistselect.length <= 1) {
        for (var i = 0; i < objList.length; i++) {

            if (objList[i].key == objSelect) {
                $("#ddlImportRoom").multiselect("widget").find(":checkbox[value='" + objList[i].key + "']").attr("checked", "checked");
                $("#ddlImportRoom option[value='" + objList[i].key + "']").attr("selected", 1);
                $("#ddlImportRoom").multiselect("refresh");

            }
        }
    }
    for (var i = 0; i < objExistselect.length; i++) {

        for (var j = 0; j < objList.length; j++) {

            if (objList[j].key == objExistselect[i]) {
                $("#ddlImportRoom").multiselect("widget").find(":checkbox[value='" + objList[j].key + "']").attr("checked", "checked");
                $("#ddlImportRoom option[value='" + objList[j].key + "']").attr("selected", 1);
                $("#ddlImportRoom").multiselect("refresh");

            }
        }
    }

    $("#selectedRoomsToImport").val(getSelectedID('ddlImportRoom'));
}

function getSelectedID(objID) {
    var selectedlist = '';
    var selectedrecs = $("#" + objID).multiselect("getChecked");
    if (selectedrecs.length > 0) {
        $(selectedrecs).each(function (indx, obj) {
            selectedlist += obj.value + ',';
        });
    }
    return selectedlist;
}