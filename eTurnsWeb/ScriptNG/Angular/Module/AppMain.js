var eTurnsApp = angular.module("eTurnsAppModule", []);
eTurnsApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);
