


//--------------------------------------------------AngularJS--------------------------------------------------
var CompleteTestApp = angular.module('CompleteTestApp', []);

CompleteTestApp.controller('CompleteTestController', function ($scope, $http) {
    $scope.test = {};
    $scope.multis = {};
    $scope.answers = [];
    
    $scope.objToArr = function (obj, objId) {
        $scope.answers.length = 0;
        var keys = Object.keys(obj);
        for (var i = 0; i < keys.length; i++) {
            var val = obj[keys[i]];
            if (val != undefined) {
                $scope.answers.push(val);
            }
            $scope.test[objId] = $scope.answers.slice();
        }
    }
    
});
//-------------------------------------------------------------------------------------------------------------