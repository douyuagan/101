(function () {
    'use strict';

    angular.module('FIS2GoApp').controller('Report1Controller', Report1Controller);
    Report1Controller.$inject = ['$scope'];

    function Report1Controller($scope) {
        $scope.LoadDiagnostic = 'Loaded At: [' + new Date().toLocaleTimeString() + '] ';
        $scope.selectedPlant = $scope.$parent.selectedPlant;
    }

    console.log("FIS (Loader Diagnostics): Loaded in Report1Controller.js Closure");
})();
