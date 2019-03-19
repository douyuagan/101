
(function () {
    'use strict';

    var obj_module = angular.module('FIS');
    var obj_service = obj_module.service('FIS.WS', ['$http', Service_Definition]);

    function Service_Definition($http) {

        this.RootServiceURL = "./DataSource.asmx/";

        this.Query = function (strServiceName, objParameters) {

            var str_data = "";
            try {
                //handle IE bombing out on trying to handle empty obj
                str_data = JSON.stringify(objParameters);
            } catch (err) {
                str_data = "";
            }

            return $http({
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                url: this.RootServiceURL + strServiceName,
                data: str_data,
            })
                .success(function (data) { console.log('success', data) })
                .error(function (xhr) { console.log('error', xhr); })
        };
    };


    console.log("FIS (Loader Diagnostics): Loaded in FIS.WS.Factory.js Closure");
})();