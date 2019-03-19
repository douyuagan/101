(function () {
    'use strict';

    angular.module('FIS2GoApp').controller('FIS2GoPlantDetailController', FIS2GoPlantDetailController);
    FIS2GoPlantDetailController.$inject = ['$scope', '$stateParams', 'message'];

    function FIS2GoPlantDetailController($scope, $stateParams, message) {

        $scope.plantCode = $stateParams.plantCode;
        alert(message);
    }
    console.log("FIS (Loader Diagnostics): Loaded in FIS2GoPlantDetailController.js Closure");
})();
