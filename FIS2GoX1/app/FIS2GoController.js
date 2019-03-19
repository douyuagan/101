(function () {
    'use strict';

    angular.module('FIS2GoApp').controller('FIS2GoController', FIS2GoController);
    FIS2GoController.$inject = ['$scope', '$state','FIS.Business', 'FIS.Cache'];

    function FIS2GoController($scope, $state,FIS_Business, FIS_Cache) {
        $scope.LoadDiagnostic = 'Loaded At: [' + new Date().toLocaleTimeString() + '] ';
  
        $scope.$business = FIS_Business;
        $scope.reset = FIS_Cache.Clear;      
        $scope.goDetail = function (selectedPlantCode) {
           
            // $state.go('plantDetail', { plantCode: selectedPlantCode, hasValidPlant: true });
            $scope.$parent.selectedPlant = selectedPlantCode;
        };
        

        //$scope.$state = FIS2Go_State;



        $scope.display = {};
        $scope.display.SplitPlants = [];
        $scope.display.UnsplitPlants = [];
        $scope.display.Search = '';



      

        var fnProcessPlants = function () {
            $scope.display.SplitPlants = [];
            $scope.display.UnsplitPlants = [];


            var ProcessedList = [];
            for (var i = 0; i < FIS_Business.Business.Plants.length; i++) {
                if ($scope.display.Search.length == 0 || (FIS_Business.Business.Plants[i].PlantCode.includes($scope.display.Search) || FIS_Business.Business.Plants[i].PlantName.includes($scope.display.Search))) {
                    ProcessedList.push(FIS_Business.Business.Plants[i]);
                   
                }
            }


            var ret_obj = [];

            var count = 0;
            var ret_loc = []

            while (count < ProcessedList.length) {
                ret_obj.push(ProcessedList.slice(count, count + 4));
                count = count + 4;
                
            }

            $scope.display.SplitPlants = ret_obj;
            $scope.display.UnsplitPlants = ProcessedList;

            console.log($scope.display.SplitPlants);
            console.log($scope.display.UnsplitPlants);
        };
        $scope.Load = function (SiteID, AREA_SAKEY, NodeID) {
            var path = SiteID + "|" + AREA_SAKEY;
            if (typeof (NodeID) != 'undefined' && NodeID != null) {
                path = path + "|" + NodeID;
            }

            $scope.$business.Get(path);
        };
        $scope.Reset = function () {
            FIS_Cache.Clear();
        };



        $scope.$watch('$business.Business.Plants', fnProcessPlants);
        $scope.$watch('display.Search', fnProcessPlants);

       


    }

    console.log("FIS (Loader Diagnostics): Loaded in FIS2GoController.js Closure");
})();
