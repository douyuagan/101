(function () {
    'use strict';

    angular.module('FIS2GoApp').controller('SelectPlantController', SelectPlantController);
    SelectPlantController.$inject = ['$scope'];

    function SelectPlantController($scope) {
        $scope.LoadDiagnostic = 'Loaded At: [' + new Date().toLocaleTimeString() + '] ';
        $scope.selectedPlant = '';
    }

    console.log("FIS (Loader Diagnostics): Loaded in FIS2GoController.js Closure");
})();
