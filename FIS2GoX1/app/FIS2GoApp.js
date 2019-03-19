(function () {
    'use strict';
    angular.module('FIS2GoApp', [
       'ui.router',
       'FIS'
    ]).config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
        var plants = {
            parent:'select-plant',
            name:'plants',
            url: '/plants',
            templateUrl:'app/views/plants.html',
            controller: 'FIS2GoController'
        };
        var about = {
            name: 'about',
            url: '/about',
            templateUrl: 'app/views/about.html'         
        };
        var plantDetail = {
            name: 'plantDetail',
            url: '/detailInfo',
            params: {
                plantCode: null,
                hasValidPlant: false    
            },
            templateUrl: 'app/views/plantDetail.html',
            controller: 'FIS2GoPlantDetailController',
            resolve: {
                message: function ($stateParams) {
                    if ($stateParams.hasValidPlant == true)
                        return "This is a valid Plant Code"
                    else 
                        return "This Plant Code is not valid"
                }
            }

        };

        var selectPlant = {
            name: 'select-plant',
            abstract: true,
            templateUrl: 'app/views/SelectPlant.html',
            controller: 'SelectPlantController',
            controllerAS:'selectPlantController'
        };
        var report1 = {
            name: 'report1',
            parent: 'select-plant',
            templateUrl: 'app/views/report1.html',
            controller: 'Report1Controller',
            controllerAS: 'report1Controller'
        }
        
        $stateProvider.state(plants);
        $stateProvider.state(about);
        $stateProvider.state(plantDetail);
        $stateProvider.state(selectPlant);
        $stateProvider.state(report1);

    }]).run(function () { // instance-injector

    });

    console.log("FIS (Loader Diagnostics): Loaded in FIS2GoApp.js Closure");

})();