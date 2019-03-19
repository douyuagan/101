(function () {
    'use strict';

    var obj_module = angular.module('FIS');
    var obj_service = obj_module.service('FIS.Business', ['$rootScope', 'FIS.WS', 'FIS.Cache', Service_Definition]);

    function Service_Definition($root, $WS, $Cache) {

        $WS.RootServiceURL = "/DataSource.asmx/";


        var _self = this;

        //Declare Service Variables
        this.Business = new FIS.Business.Types.Business();



        $Cache.Fetch('EnterpriseTree').then(function (payload_object) {
            if (payload_object.was_found && !_self._IsStale(payload_object.payload)) {
                Object.assign(_self.Business, payload_object.payload);
                _self.Business.ToPrototype();
                $root.$apply();
            } else {
                _self.Business.Load($WS).then(function () { $Cache.Push('EnterpriseTree', _self.Business) });
            }
        }, function () {
            _self.Business.Load($WS).then(function () { $Cache.Push('EnterpriseTree', _self.Business) });

        });


        //Declare Service Functions
        //[Public]
        this.Get = function (Superpath) {
            console.log("Received GET for " + Superpath);
            //Logic to Find and Return the Desired Object

            var _self = this;

            var objPath = this._ParseSuperpath(Superpath);
            var fn_find = function (objPath) {
                if (objPath.NodeID != null) {
                    return _self._FindNode(objPath);
                }
                if (objPath.AREA_SAKEY != null) {
                    return _self._FindArea(objPath);
                }
                if (objPath.SiteID != null) {
                    return _self._FindPlant(objPath);
                }
                return null;
            };
            var result = fn_find(objPath);



            return new Promise((resolve, reject) => {
                var _self = this;
                if (result == null) {
                    //we couldn't find anything, so we'll load the plant and try-again
                    console.log("Not Found, Attempting to Load from Server");
                    var obj_plant = _self._FindPlant(objPath);
                    obj_plant.Load($WS).then(resolve(fn_find(objPath))).then(function () { $Cache.Push('EnterpriseTree', _self.Business); });
                } else {
                    console.log("Found (Load from Memory): ", result);
                    resolve(result);
                }

            });
        };


        //[Private]
        this._ParseSuperpath = function (SuperPath) {
            var ret_obj = { "SiteID": null, "AREA_SAKEY": null, "NodeID": null };
            var parts = SuperPath.split("|");
            if (parts.length > 0) {
                ret_obj.SiteID = parts[0];
            }
            if (parts.length > 1) {
                ret_obj.AREA_SAKEY = parts[1];
            }
            if (parts.length > 2) {
                ret_obj.NodeID = parts[2];
            }
            return ret_obj;
        };
        this._FindPlant = function (objPath) {
            var _self = this;
            var obj = this.Find(this.Business.Plants, function (el) { return el.SiteID == objPath.SiteID; });
            if (_self._IsStale(obj)) {
                return null;
            }
            return obj;
        };
        this._FindArea = function (objPath) {
            var _self = this;
            var objPlant = this._FindPlant(objPath)
            if (objPlant != null) {
                var obj = this.Find(objPlant.Areas, function (el) { return el.AREA_SAKEY == objPath.AREA_SAKEY; });
                if (_self._IsStale(obj)) {
                    return null;
                }
                return obj;
            }
            return null;
        };
        this._FindNode = function (objPath) {
            var _self = this;
            var objArea = this._FindArea(objPath)
            if (objArea != null) {
                var obj = this.Find(objArea.Assets, function (el) { return el.NodeID == objPath.NodeID; });
                if (_self._IsStale(obj)) {
                    return null;
                }
                return obj;
            }
            return null;
        };
        this._IsStale = function (obj) {
            if (typeof (obj) == 'undefined'
                || obj == null
                || typeof (obj["LoadRecord"]) == 'undefined'
                || obj["LoadRecord"].IsLoaded == false) {
                return true;
            }

            //Check if it's older than 2 days
            var ms_offset = new Date() - obj["LoadRecord"].LoadedAt;

            if (ms_offset > (86400000) * 2) {
                return true;
            }

            return false;
        };



        //[Polyfill - May be removed Later]
        //function(currentValue, index, arr),thisValue
        this.Find = function (arr, FindFunction) {

            if (typeof (arr) == 'undefined' || arr == null) {
                return null;
            }

            for (var i = 0; i < arr.length; i++) {
                if (FindFunction(arr[i], i, arr)) {
                    return arr[i];
                }
            }
            return null;
        };

    }

    console.log("FIS (Loader Diagnostics): Loaded in FIS.Business.Factory.js Closure");

})();
