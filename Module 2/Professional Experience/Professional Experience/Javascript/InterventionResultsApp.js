onStart();


function onStart()
{
    var div = document.getElementById('main');
    var divs = div.getElementsByTagName('div');
    for (var i = 0; i < divs.length; i++) {
        divs[i].style.display = "block";
    }
}
//--------------------------------------------------AngularJS--------------------------------------------------
var InterventionResultsApp = angular.module('InterventionResultsApp', []);

InterventionResultsApp.controller('InterventionResultsController', function ($scope, $http) {
    $scope.index = 0;
    $scope.check = function (i) {
        if ($scope.index == i)
            return true;
        else
            return false;
    };

    $scope.setIndex = function (i) {
        $scope.index = i;
    };

    $scope.goToTest = function (testName) {
        $http.post('/Participant/GenerateTest', {Test: testName}).
            then(function (response) {
                alert("redirect");
            }, function (error) {
            });
    };
});
//-------------------------------------------------------------------------------------------------------------