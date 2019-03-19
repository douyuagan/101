(function () {
    'use strict';

    angular.module('FIS', [
        // Angular modules 

        // Custom modules 

        // 3rd Party Modules
        
    ]).config(function() { // provider-injector
        // This is an example of config block.
        // You can have as many of these as you want.
        // You can only inject Providers (not instances)
        // into config blocks.
    }).run(function() { // instance-injector
        // This is an example of a run block.
        // You can have as many of these as you want.
        // You can only inject instances (not Providers)
        // into run blocks
    });

    console.log("FIS (Loader Diagnostics): Loaded in FIS.Module.js Closure");
})();