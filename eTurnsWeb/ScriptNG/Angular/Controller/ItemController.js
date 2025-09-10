
eTurnsApp.controller("ItemController", ["$scope", "$http", "$compile", function ($scope, $http, $compile, $timeout) {

}]);

eTurnsApp.controller("ItemDetailsController", ['$scope', '$http', '$compile', '$timeout', 'eTurnsHelper', 'eTurnsHelpersvc', function ($scope, $http, $compile, $timeout, eTurnsHelper, eTurnsHelpersvc) {

    $scope.ItemSuppliers = [];
    $scope.ItemMans = [];
    $scope.ItemLocs = [];

    $scope.ItemSupDirty = 0;
    $scope.ItemManDirty = 0;
    $scope.ItemLocDirty = 0;
    
    
    $http({
        method: 'POST',
        url: '/Inventory/GetItemSuppliers',        
        headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
        data: { ItemGUID: ItemGUID, ItemID: ItemID }
    }).then(function (response) {
        $scope.ItemSuppliers = response.data;
        $timeout(function () { $scope.ItemSupDirty = 0; }, 200);
    }, function (error) {
        eTurnsHelper.ShowMessageNotification(error, 3);

    });

    $http({
        method: 'POST',
        url: '/Inventory/GetitemMans',
        headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
        data: { ItemGUID: ItemGUID, ItemID: ItemID }
    }).then(function (response) {
        $scope.ItemMans = response.data;
        $timeout(function () { $scope.ItemManDirty = 0; }, 200);
    }, function (error) {
        eTurnsHelper.ShowMessageNotification(error, 3);
    });

    $http({
        method: 'POST',
        url: '/Inventory/GetItemLocations',
        headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
        data: { ItemGUID: ItemGUID, ItemID: ItemID }
    }).then(function (response) {
        $scope.ItemLocs = response.data;
        $timeout(function () { $scope.ItemLocDirty = 0; }, 200);
    }, function (error) {
        eTurnsHelper.ShowMessageNotification(error, 3);
    });


    $scope.addItemSup = function () {
        AddnewSup(true, true);
    };

    $scope.deleteItemSup = function (item) {
        var index = $scope.ItemSuppliers.indexOf(item);
        $scope.ItemSuppliers[index].IsDeleted = true;
        //$scope.ItemSuppliers.splice(index, 1);
    };


    $scope.addItemMan = function () {
        AddNewMan(true, true);
    };

    $scope.deleteItemMan = function (item) {
        var index = $scope.ItemMans.indexOf(item);
        $scope.ItemMans[index].IsDeleted = true;
    };

    $scope.addItemLoc = function () {
        AddNewLoc(true, true);
    };

    $scope.deleteItemLoc = function (item) {
        var index = $scope.ItemLocs.indexOf(item);
        $scope.ItemLocs[index].IsDeleted = true;
    };

    $scope.$watch('ItemSuppliers', function () {
        $scope.ItemSupDirty = 1;
    }, true);

    $scope.$watch('ItemMans', function () {
        $scope.ItemManDirty = 1;
    }, true);

    $scope.$watch('ItemLocs', function () {
        $scope.ItemLocDirty = 1;
    }, true);

    function AddnewSup(add, validate) {
        var val = 0;
        var arrinvalidrows = $scope.ItemSuppliers.filter(function (obj) {
            val = $.trim(obj.SupplierName);
            return $.trim(obj.SupplierName) === "" && obj.IsDeleted == false
        });
        var result = _.groupBy(_.filter($scope.ItemSuppliers, function (obj) { return obj.IsDeleted == false }), function (obj) { return $.trim(obj.SupplierName) });
        if (_.values(result).length == _.values(_.filter($scope.ItemSuppliers, function (obj) { return obj.IsDeleted == false })).length) {
            if (arrinvalidrows.length == 0) {
                if (add && validate) {
                    $scope.ItemSuppliers.push({ ID: 0, ItemGUID: ItemGUID, SupplierID: '', SupplierName: '', BlanketPO: '', SupplierNumber: '', BlanketPOID: '', GUID: '', IsDefault: '', IsDeleted: false });
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                eTurnsHelper.ShowMessageNotification("Supplier Name can not be blank", 3);
                return false;

            }
        }
        else {
            eTurnsHelper.ShowMessageNotification("Duplicate Supplier names found", 3);
            return false;
        }

    }
    function AddNewMan(add, validate) {
        var arrinvalidrows = $scope.ItemMans.filter(function (obj) {
            return ($.trim(obj.ManufacturerName) === "" || $.trim(obj.ManufacturerName) === "") && obj.IsDeleted == false
        });

        var result = _.groupBy(_.filter($scope.ItemMans, function (obj) { return obj.IsDeleted == false }), function (obj) { return $.trim(obj.ManufacturerName) });

        if (_.values(result).length == _.values(_.filter($scope.ItemMans, function (obj) { return obj.IsDeleted == false })).length) {
            if (arrinvalidrows.length == 0) {
                if (add && validate) {
                    $scope.ItemMans.push({ ManufacturerName: '', ManufacturerNumber: '', ID: 0, ItemGUID: ItemGUID, IsDefault: '', GUID: '', IsDeleted: false });
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                eTurnsHelper.ShowMessageNotification("Account name or number should not be blank.", 3);
                return false;

            }
        }
        else {
            eTurnsHelper.ShowMessageNotification("Duplicate Account name or number", 3);
            return false;

        }
    }
    function AddNewLoc(add, validate) {
        var arrinvalidrows = $scope.ItemLocs.filter(function (obj) {
            return ($.trim(obj.BinNumber) === "" || $.trim(obj.BinNumber) === "") && obj.IsDeleted == false
        });

        var result = _.groupBy(_.filter($scope.ItemLocs, function (obj) { return obj.IsDeleted == false }), function (obj) { return $.trim(obj.BinNumber) });


        if (_.values(result).length == _.values(_.filter($scope.ItemLocs, function (obj) { return obj.IsDeleted == false })).length) {
            if (arrinvalidrows.length == 0) {
                if (add && validate) {
                    $scope.ItemLocs.push({ ID: 0, IsDefault: '', GUID: '', IsDeleted: false, BinNumber: '', CriticalQuantity: 0, MinimumQuantity: 0, MaximumQuantity: 0, CustomerOwnedQuantity: 0, ConsignedQuantity: 0, eVMISensorID: '' });
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                eTurnsHelper.ShowMessageNotification("Please enter bin number", 3);
                return false;

            }
        }
        else {
            eTurnsHelper.ShowMessageNotification("Duplicate bin number", 3);
            return false;

        }
    }


    $scope.SaveitemDetails = function ($event) {

        if (AddNewMan(false, true)) {
            if (AddnewSup(false, true)) {
                if (AddNewLoc(false, true)) {
                    return true;
                }
                else {
                    $event.preventDefault();
                }
            }
            else {
                $event.preventDefault();
            }
        }
        else {
            $event.preventDefault();
        }
    }

}]);

eTurnsApp.controller("SupplierScheduleController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {

}]);


//eTurnsApp.controller("SupplierBPOController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {


//}]);

//eTurnsApp.controller("SupplierACController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {

//}]);