(function () {
    'use strict';

    var obj_module = angular.module('FIS');

    var plant_directive = obj_module.directive('fisBusinessDirectivesPlant', ['$window', function ($window) {
        return {
            restrict: 'E',
            scope: { plant: "=" },
            template: '<div>-{{plant.PlantCode}} </div>'
        };
    }]);

    var area_directive = obj_module.directive('fisBusinessDirectivesArea', ['$window', function ($window) {
        return {
            restrict: 'E',
            scope: { area: "=" },
            controller: ['$scope', 'FIS.Business', function ($scope, FIS_Business) {
                $scope.$business = FIS_Business;

                $scope.Get = function (SiteID, AREA_SAKEY, NodeID) {

                    var path = SiteID + "|" + AREA_SAKEY;
                    if (typeof (NodeID) != 'undefined' && NodeID != null) {
                        path = path + "|" + NodeID;
                    }

                    $scope.$business.Get(path);
                };
            }],
            template: '<div>&nbsp;+->{{area.AreaName}} <button ng-click="Get(area.SiteID, area.AREA_SAKEY, 0)">Load Assets</button><button ng-click="Get(area.SiteID, area.AREA_SAKEY)">Load Area</button></div>'
        };
    }]);

    var node_directive = obj_module.directive('fisBusinessDirectivesAsset', ['$window', function ($window) {
        return {
            restrict: 'E',
            scope: { node: "=" },
            controller: ['$scope', 'FIS.Business', function ($scope, FIS_Business) {
                $scope.$business = FIS_Business;

                $scope.Get = function (SiteID, AREA_SAKEY, NodeID) { $scope.$business.Get(SiteID + "|" + AREA_SAKEY + "|" + NodeID); };
            }],
            template: '<div>&nbsp;++++->{{node.Path}} <button ng-click="Get(node.SiteID, node.AREA_SAKEY, node.NodeID)">Load</button></div>'
        };
    }]);





})();