
(function () {
    'use strict';

    var obj_module = angular.module('FIS2GoApp');
    var obj_service = obj_module.service('FIS2Go.State', ['$rootScope', 'FIS.Cache', 'FIS.Business', Service_Definition]);

    function Service_Definition($root, $FIS_Cache, $FIS_Business) {

        this.State = {};

        //Navigation
        this.State.Page = "JPH";

        //Node Selections
        this.State.Superpaths = [];
        this.State.Nodes = [];

        //Report Criteria
        this.State.ProductiveTimeFiltering = "PT";
        this.State.Period = "CS";


        this.AddNode = function (SuperPath) {
            var _self = this;
            _self.Superpaths.push(SuperPath);
            return _self.Nod_BuildNodeRef(SuperPath);
        };
        this._BuildNodeRef = function (SuperPath) {
            var _self = this;

            var p = FIS_Business.Get(SuperPath).then(function (data) {
                _self.Nodes.push(data);
            });
            return p;

        };




        //Private
        this.Save = function () {
            return $FIS_Cache.Push("State", this.State);
        };
        this._Load = function () {
            var _self = this;

            return new Promise((resolve, reject) => {
                $FIS_Cache.Fetch('State').then(function (payload_object) {
                    if (payload_object.was_found) {
                        _self.State = payload_object.payload;
                        _self.State.Nodes = [];
                        var promises = [];
                        for (var i = 0; i < _self.State.Superpaths; i++) {
                            var p = $FIS_Business.Get(_self.State.Superpaths[i]).then(function (data) { _self.State.Nodes.push(data); });
                            promises.push(p);
                        }
                        $root.$apply();
                        Promise.all(promises).then(resolve, reject);
                    } else {
                        reject();
                    }
                });
            });
        };

        this._Load(function () { console.log("FIS2GoApp.State: Load Complete"); }, function () { console.log("FIS2GoApp.State: Load Failed"); });
    };


    console.log("FIS (Loader Diagnostics): Loaded in FIS2GoApp.State.Service.js Closure");
})();