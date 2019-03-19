
(function () {
    'use strict';

    var obj_module = angular.module('FIS');
    var obj_service = obj_module.service('FIS.Exception', [Service_Definition]);

    function Service_Definition() {

        this.throw = function (ExceptionCode, ExceptionText) {
            console.log("Exception Thrown: (" + ExceptionCode + ") " + ExceptionText);
            return ExceptionText;
        };

    };


    console.log("FIS (Loader Diagnostics): Loaded in FIS.Exception.Service.js Closure");
})();