
eTurnsApp.controller("EnterpriseController", ["$scope", "$http", "$compile", function ($scope, $http, $compile, $timeout) {

}]);

eTurnsApp.controller("EnterpriseDetailsController", ['$scope', '$http', '$compile', '$timeout', 'eTurnsHelper', 'eTurnsHelpersvc', function ($scope, $http, $compile, $timeout, eTurnsHelper, eTurnsHelpersvc) {
    $scope.EnterpriseSuperAdmins = [];
    $scope.EnterpriseSuperAdminDirty = 0;

    if (EnterpriseID > 0) {
        $http({
            method: 'POST',
            url: '/Master/GetEnterpriseSuperAdmins',
            headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
            data: { EnterpriseID: EnterpriseID }
        }).then(function (response) {
            $scope.EnterpriseSuperAdmins = response.data;
            $timeout(function () { $scope.EnterpriseSuperAdminDirty = 0; }, 200);
        }, function (error) {
            eTurnsHelper.ShowMessageNotification(error, 3);

        });

    }


    $scope.AddNewSuperAdmin = function () {
        AddNewSuperAdmin(true, true);
    };

    $scope.DeleteSuperAdmin = function (item) {
        
        var index = $scope.EnterpriseSuperAdmins.indexOf(item);
        $scope.EnterpriseSuperAdmins[index].MarkDeleted = true;
        //$scope.EnterpriseSuperAdmins.splice(index, 1);
    };

    $scope.AssignRevokeAdmin = function (item) {
        if (item.IsEPSuperAdmin) {
            item.RoleID = -2;
            item.RoleName = "Enterprise Super admin";
        }
        else {
            item.RoleID = 0;
            item.RoleName = "";
        }

    }

    $scope.$watch('EnterpriseSuperAdmins', function () {
        $scope.EnterpriseSuperAdminDirty = 1;
    }, true);


    function AddNewSuperAdmin(add, validate) {
        
        var val = 0;
        var arrinvalidrows = $scope.EnterpriseSuperAdmins.filter(function (obj) {
            val = $.trim(obj.UserName);
            return $.trim(obj.UserName) === "" && obj.MarkDeleted == false
        });
        var result = _.groupBy(_.filter($scope.EnterpriseSuperAdmins, function (obj) { return obj.MarkDeleted == false }), function (obj) { return $.trim(obj.UserName) });
        if (_.values(result).length == _.values(_.filter($scope.EnterpriseSuperAdmins, function (obj) { return obj.MarkDeleted == false })).length) {
            if (arrinvalidrows.length == 0) {
                if (add && validate) {
                    $scope.EnterpriseSuperAdmins.push({ ID: 0, UserID: '', UserName: '', Email: '', RoleID: -2, UserType: 2, UserTypeName: '', RoleName: 'Enterprise Super admin', IsDefault: false, MarkDeleted: false, IsEPSuperAdmin: true });
                    return true;
                }
                else {
                    return true;
                }
            }
            else {
                eTurnsHelper.ShowMessageNotification("User name should not be blank.", 3);
                return false;

            }
        }
        else {
            eTurnsHelper.ShowMessageNotification("Duplicate user name found", 3);
            return false;
        }

    }

    $scope.SaveEnterprise = function ($event) {

        if (AddNewSuperAdmin(false, true)) {
            return true;
        }
        else {
            $event.preventDefault();
        }
    }

}]);

