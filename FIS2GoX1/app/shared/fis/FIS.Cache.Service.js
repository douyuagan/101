
(function () {
    'use strict';

    var obj_module = angular.module('FIS');
    var obj_service = obj_module.service('FIS.Cache', ['FIS.Exception', Service_Definition]);

    function Service_Definition(FIS_Exception) {

        //IndexedDB Utility
        this.DB =
        {
            DBConn: null,
            DBName: "FIS",
            DBVersion: 1,
            DBObjStore: "FISObjectStore2",
            DBPointer: null,
            DBInitializatized: false,
            IsSupported: function () {
                return "indexedDB" in window;
            },
            Init: function () {


                var _self = this;

                if (!_self.IsSupported()) {
                    throw new FIS_Exception.throw("00010", "FIS.Cache: Fatal Exception! IndexedDB is Not Supported");
                }


                return new Promise((resolve, reject) => {

                    if (_self.DBInitializatized) {
                        resolve();
                        return;
                    }

                    _self.DBConn = window.indexedDB.open(_self.DBName, _self.DBVersion);

                    _self.DBConn.onerror = function (event) {
                        _self.DBInitializatized = false;
                        //promise code here

                        console.log("FIS.Cache: IndexedDB Failed to Initialize, Completed with Error");
                        reject();
                    };
                    _self.DBConn.onsuccess = function (ev) {
                        _self.DBInitializatized = true;
                        //promise code here

                        var db = ev.target.result;
                        _self.DBPointer = db;

                        console.log("FIS.Cache: IndexedDB Successfully Initialized, Completed");
                        resolve();
                    };
                    _self.DBConn.onupgradeneeded = function (ev) {
                        var db = ev.target.result;
                        if (db.objectStoreNames.contains(_self.DBObjStore)) {
                            db.deleteObjectStore(_self.DBObjStore);
                        }

                        var store = db.createObjectStore(_self.DBObjStore, {
                            keyPath: 'var_id'
                        });


                        _self.DBPointer = db;


                        console.log("FIS.Cache: IndexedDB Upgrade Needed, Completed");
                        //resolve();
                    };


                });
            },
            push: function (key, value) {

                var _self = this;

                _self.Init().then(function () {

                    var transaction = _self.DBPointer.transaction([_self.DBObjStore], 'readwrite');
                    var store = transaction.objectStore(_self.DBObjStore);

                    var storage_container = { var_id: key, data: value };

                    var request = store.put(storage_container);

                    return new Promise((resolve, reject) => {
                        request.onsuccess = function () { console.log("FIS.Cache: Pushed '" + key + "' OK!"); resolve(); };
                        request.onerror = function () { console.log("FIS.Cache: Failed to Push '" + key + "'!"); reject(); };;
                    });
                });



            },
            fetch: function (key) {
                var _self = this;
                return new Promise((resolve, reject) => {
                    _self.Init().then(function () {
                        var transaction = _self.DBPointer.transaction([_self.DBObjStore], 'readonly');
                        var store = transaction.objectStore(_self.DBObjStore);

                        var request = store.get(key);

                        request.onsuccess = function (ev) {

                            if (typeof (request.result) != 'undefined' && request.result.data != null) {
                                console.log("FIS.Cache: Found '" + key + "' OK!");
                                resolve({ was_found: true, payload: request.result.data });
                                return;
                            }
                            console.log("FIS.Cache: Found '" + key + "' but Empty!");
                            resolve({ was_found: false, payload: null });
                        };
                        request.onerror = function () { console.log("FIS.Cache: Failed to retreive '" + key + "'!"); reject(); };
                    });
                });
            },
            clear: function () {
                var _self = this;

                _self.Init().then(function () {

                    var transaction = _self.DBPointer.transaction([_self.DBObjStore], 'readwrite');
                    var store = transaction.objectStore(_self.DBObjStore);
                    var request = store.clear();;

                    return new Promise((resolve, reject) => {
                        request.onsuccess = resolve;
                        request.onerror = reject;
                    });
                });

            }
        };

        //Public Functions
        this.Push = function (key, value) { var _self = this; return _self.DB.push(key, value); };
        this.Fetch = function (key) { var _self = this; return _self.DB.fetch(key); };
        this.Clear = function () { var _self = this; return _self.DB.clear(); };
    };


    console.log("FIS (Loader Diagnostics): Loaded in FIS.Cache.Service.js Closure");
})();