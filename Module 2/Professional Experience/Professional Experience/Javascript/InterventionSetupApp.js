var InterventionSetupApp = angular.module('InterventionSetupApp', []);

InterventionSetupApp.controller('InterventionSetupController', function ($scope, $http) {
    $scope.index = 1;

    $scope.check = function (i) {
        if ($scope.index == i)
            return true;
        else
            return false;
    };

    $scope.setIndex = function (i) {
        $scope.index = i;
    };
});