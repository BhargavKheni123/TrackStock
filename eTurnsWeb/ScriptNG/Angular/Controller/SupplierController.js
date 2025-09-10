
eTurnsApp.controller("SupplierController", ["$scope", "$http", "$compile", function ($scope, $http, $compile, $timeout) {

}]);

eTurnsApp.controller("SupplierDetailsController", ['$scope', '$http', '$compile', '$timeout', 'eTurnsHelper', 'eTurnsHelpersvc', function ($scope, $http, $compile, $timeout, eTurnsHelper, eTurnsHelpersvc) {
    $scope.SupplierBPO = [];
    $scope.SupplierACS = [];
    $scope.SupplierBlanketDirty = 0;
    $scope.SupplierAccountDirty = 0;

    if (SupplierID > 0) {
        $http({
            method: 'POST',
            url: '/Master/GetSupplierBPOS',
            headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
            data: { SupplierID: SupplierID }
        }).then(function (response) {
            $scope.SupplierBPO = response.data;
            $timeout(function () {
                $scope.SupplierBlanketDirty = 0;
                //var table = $('#SupplierBlanketPO').DataTable({
                //    "sDom": 'Rfrt',                    
                //    "sScrollX": "100%",
                //    "sScrollY": "200px",
                //    "oColReorder": {
                //        "bAddFixed": false
                //    },

                //});
            }, 200);
        }, function (error) {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(error);
            //eTurnsHelper.ShowMessageNotification(error, 3);

        });

        $http({
            method: 'POST',
            url: '/Master/GetSupplierACS',
            headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
            data: { SupplierID: SupplierID }
        }).then(function (response) {
            $scope.SupplierACS = response.data;
            $timeout(function () { $scope.SupplierAccountDirty = 0; }, 200);
        }, function (error) {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(error);
            //eTurnsHelper.ShowMessageNotification(error, 3);
        });

    }


    $scope.AddNewBPO = function () {
        AddnewBPO(true, true);
    };

    $scope.DeleteSBPO = function (item) {
        var index = $scope.SupplierBPO.indexOf(item);
        $scope.SupplierBPO[index].IsDeleted = true;
        //$scope.SupplierBPO.splice(index, 1);
    };


    $scope.AddNewSAC = function () {
        AddNewAccount(true, true);

        var currentRow = 0;


    };

    $scope.DeleteSAC = function (item) {
        var index = $scope.SupplierACS.indexOf(item);
        $scope.SupplierACS[index].IsDeleted = true;
    };
    $scope.DefaultSAC = function (item) {
        //var index = $scope.SupplierACS.indexOf(item);
        //for (var i = 0; i < $scope.SupplierACS.length; i++) {
        //    if (i == index) {
        //        $scope.SupplierACS[index].IsDefault = true;
        //    }
        //    else
        //    {
        //        $scope.SupplierACS[index].IsDefault = false;
        //    }
        //}
    };
    $scope.$watch('SupplierBPO', function () {
        $scope.SupplierBlanketDirty = 1;
    }, true);
    $scope.$watch('SupplierACS', function () {
        $scope.SupplierAccountDirty = 1;
    }, true);

    function AddnewBPO(add, validate) {
        var val = 0;
        var arrinvalidrows = $scope.SupplierBPO.filter(function (obj) {
            val = $.trim(obj.BlanketPO);
            return $.trim(obj.BlanketPO) === "" && obj.IsDeleted == false
        });
        var result = _.groupBy(_.filter($scope.SupplierBPO, function (obj) { return obj.IsDeleted == false }), function (obj) { return $.trim(obj.BlanketPO) });
        if (_.values(result).length == _.values(_.filter($scope.SupplierBPO, function (obj) { return obj.IsDeleted == false })).length) {
            if (arrinvalidrows.length == 0) {
                if (add && validate) {
                    $scope.SupplierBPO.push({ ID: 0, SupplierID: SupplierID, BlanketPO: '', ValidStartDate: '', ValidEndDate: '', GUID: '', MaxLimit: '', IsNotExceed: false, MaxLimitQty: '', IsNotExceedQty: false, PulledQty: 0, OrderedQty: 0, OrderedUseCost: 0, OrderRemainCost: NotApplicable, IsDeleted: false, OrderUsed: 0, TotalOrder : 0 });
                    //var oTable = $('#SupplierBlanketPO').dataTable();
                    //oTable.fnAddData(['', SupplierID, '', '', '', '', '', false, '', false, 0, 0, false], true);
                    //oTable.fnDraw();
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                //eTurnsHelper.ShowMessageNotification(MsgBlanketPOValidation, 3);
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(MsgBlanketPOValidation);
                return false;

            }
        }
        else {
            //eTurnsHelper.ShowMessageNotification(MsgDuplicatePOBlanket, 3);
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgDuplicatePOBlanket);
            return false;
        }

    }
    function AddNewAccount(add, validate) {
        
        var arrinvalidrows = $scope.SupplierACS.filter(function (obj) {
            return ($.trim(obj.AccountNo) === "" || $.trim(obj.AccountName) === "") && obj.IsDeleted == false
        });
        var arrinvalidrowsDefault = $scope.SupplierACS.filter(function (obj) {
            return obj.IsDefault == true && obj.IsDeleted == false
        });
        var arrNonDeleted = $scope.SupplierACS.filter(function (obj) {
            return obj.IsDeleted == false
        });

        var flag = 0;
        var error = '';
        var arrDefaultrows = $scope.SupplierACS.filter(function (obj1) {
            if ((flag == 1 && obj1.IsDefault == true) && obj1.IsDeleted == false)
            {
                error = true;
            }
            if (obj1.IsDefault && obj1.IsDeleted == false)
            {
                flag = 1;
            }
        });

        var NotDelRecords = _.filter($scope.SupplierACS, function (obj) { return obj.IsDeleted == false });
        var result = _.groupBy(NotDelRecords, function (obj) { return $.trim(obj.AccountNo) });
        var result1 = _.groupBy(NotDelRecords, function (obj) { return $.trim(obj.AccountName) });

        //if (_.values(result).length == _.values(NotDelRecords).length && _.values(result1).length == _.values(NotDelRecords).length) {
        if (arrinvalidrows.length == 0) {

            if (add && validate) {
                if (NotDelRecords.length > 0) {
                    $scope.SupplierACS.push({ ID: 0, SupplierID: SupplierID, AccountNo: '', IsDefault: false, AccountName: '', GUID: '', IsDeleted: false, Address: '', City: '', State: '', ZipCode: '', ShipToID: '', Country: '', IsViewable: true, IsEditable: true, IsDeleteable: true});
                }
                else {
                    $scope.SupplierACS.push({ ID: 0, SupplierID: SupplierID, AccountNo: '', IsDefault: true, AccountName: '', GUID: '', IsDeleted: false, Address: '', City: '', State: '', ZipCode: '', ShipToID: '', Country: '', IsViewable: true, IsEditable: true, IsDeleteable: true });
                }
                return true;
            }
            else {
                if ($("input#IsSendtoVendor").is(":checked")) {
                    if (arrNonDeleted == null || arrNonDeleted == undefined || arrNonDeleted.length <= 0) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgSelectDefaultAccount);
                        //eTurnsHelper.ShowMessageNotification(MsgSelectDefaultAccount, 3);
                        return false;
                    }
                    if (error || (flag == 0 && arrNonDeleted.length > 0) || (arrinvalidrowsDefault.length != 1 && arrNonDeleted.length > 0)) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgSelectDefaultAccount);
                        //eTurnsHelper.ShowMessageNotification(MsgSelectDefaultAccount, 3);
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }

        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgAccountNameNumber);
            //eTurnsHelper.ShowMessageNotification(MsgAccountNameNumber, 3);
            return false;

        }
        //}
        //else {
        //    eTurnsHelper.ShowMessageNotification("Duplicate Account name or number", 3);
        //    return false;

        //}

    }
    $scope.SaveSupplier = function ($event) {
        
        if ($("#hdValidatePhoneNumber").val() != "" && ($("#hdValidatePhoneNumber").val() == "YES" || $("#hdValidatePhoneNumber").val() == "yes")) {

            if ($("#Phone").val() != "" && $("#Country").val() != "" && $("#hdPhoneRegex").val() != "") {
                var regexpattern = new RegExp($("#hdPhoneRegex").val());
                var inputVal = $("#Phone").val();
                if (!regexpattern.test(inputVal)) {

                    showNotificationDialog();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgPhoneNumberValid);
                    $("#Phone").focus();
                    $event.preventDefault();
                    return false;
                }
            }
            else {
                if ($("#Phone").val() != "" && $("#hdPhoneRegex").val() != "") {
                    var regexpattern = new RegExp($("#hdPhoneRegex").val());
                    var inputVal = $("#Phone").val();
                    if (!regexpattern.test(inputVal)) {

                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgPhoneNumberValid);
                        $("#Phone").focus();
                        $event.preventDefault();
                        return false;
                    }
                }
            }
        }

        if (AddnewBPO(false, true)) {
            if (AddNewAccount(false, true)) {
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

} ]);

eTurnsApp.controller("SupplierScheduleController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {

}]);


//eTurnsApp.controller("SupplierBPOController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {


//}]);

//eTurnsApp.controller("SupplierACController", ["$scope", "$http", "$compile", function ($scope, $http, $compile) {

//}]);