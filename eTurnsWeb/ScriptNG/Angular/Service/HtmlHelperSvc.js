/// <reference path="../../angular.js" />



eTurnsApp.factory('eTurnsHelper', function () {
    return {
        ShowMessageNotification: function (notificationMsg, msgCode) {
            switch (msgCode) {
                case 1:
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    break;
                case 2:
                    $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                    break;
                case 3:
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    break;
                default:
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    break;
            }
            $("#spanGlobalMessage").html(notificationMsg);
            $('div#target').fadeToggle();
            $("div#target").delay(2000).fadeOut(200);
        }
    };
});

eTurnsApp.service('eTurnsHelpersvc', function () {
    this.NotifyUser = function (msg) { alert(msg); }
});