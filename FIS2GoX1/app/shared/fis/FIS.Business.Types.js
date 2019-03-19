var FIS = FIS || {};
FIS.Business = FIS.Business || {};

FIS.Business.Types = {};

//Node
{
    FIS.Business.Types.Node = function (WebRecord)
    {
        this.NodeID = null;
        this.ParentGroupID = null;
        this.AREA_SAKEY = null;
        this.SiteID = null;

        this.Name = null;
        this.AreaName = null;
        this.PlantName = null;
        this.Path = null;
        this.Type = null;
        this.Alias = null;

        this.Active = false;
        this.IsAreaPaypoint = false;
        this.IsLinePaypoint = false;
        this.IsBottleneckPoint = false;
        this.IsSelectable = false;
        this.IsNode = false;
        this.IsGroup = false;
        this.IsRoot = false;

        this.Ordinal = null;
        this.Children = [];
        this.EPs = {};

        this.LoadRecord = new FIS.Business.Types.LoadRecord();

        if (typeof (WebRecord) != 'undefined' && WebRecord != null) {
            for (var propertyName in WebRecord) {
                this[propertyName] = WebRecord[propertyName];
            }

            this.LoadRecord.IsLoaded = true;
            this.LoadRecord.LoadedAt = new Date();
        }

        return this;
    };
}

//Area
{
    FIS.Business.Types.Area = function (WebRecord) {

        this.SiteID = null;
        this.AreaID = null;
        this.AreaName = null;
        this.SortOrder = null;
        this.AREA_SAKEY = null;
        this.Ordinal = null;

        this.Assets = [];

        this.LoadRecord = new FIS.Business.Types.LoadRecord();

        if (typeof (WebRecord) != 'undefined' && WebRecord != null) {
            for (var propertyName in WebRecord) {
                if (WebRecord[propertyName] != null) {
                    this[propertyName] = WebRecord[propertyName];
                }
            }

            this.LoadRecord.IsLoaded = true;
            this.LoadRecord.LoadedAt = new Date();
        }

        return this;
    };
    FIS.Business.Types.Area.prototype.ToPrototype = function () {
        var _self = this;
        var typed_arr = [];
        for (var i = 0; i < _self.Assets.length; i++) {
            var typed_o = angular.extend(new FIS.Business.Types.Node(), _self.Assets[i]);
            //typed_o.ToPrototype();
            typed_arr.push(typed_o);
        }
        _self.Assets = typed_arr;
    };

}

//Plant
{
    FIS.Business.Types.Plant = function (WebRecord) {

        

        this.PlantName = null;
        this.PlantCode = null;
        this.Region = null;
        this.Division = null;
        this.SiteID = null;
        this.Ordinal = null;
        this.Visible = null;

        this.Areas = [];

        this.LoadRecord = new FIS.Business.Types.LoadRecord();

        if (typeof (WebRecord) != 'undefined' && WebRecord != null)
        {
            for (var propertyName in WebRecord) {
                this[propertyName] = WebRecord[propertyName];
            }

            ProcessedAreaArray = [];
            for (var a_index = 0; a_index < this.Areas.length; a_index++)
            {
                ProcessedAreaArray.push(new FIS.Business.Types.Area(this.Areas[a_index]));
            }
            this.Areas = ProcessedAreaArray;

            this.LoadRecord.IsLoaded = true;
            this.LoadRecord.LoadedAt = new Date();
        }


        return this;
    };
    FIS.Business.Types.Plant.prototype.ToPrototype = function () {
        var _self = this;
        var typed_arr = [];
        for (var i = 0; i < _self.Areas.length; i++) {
            var typed_o = angular.extend(new FIS.Business.Types.Area(), _self.Areas[i]);
            typed_o.ToPrototype();
            typed_arr.push(typed_o);
        }
        _self.Areas = typed_arr;
    };
}

//Business
{
    FIS.Business.Types.Business = function () {

        this.Plants = [];
        this.LoadRecord = new FIS.Business.Types.LoadRecord();

        return this;
    };
    FIS.Business.Types.Business.prototype.Load = function (objWS) {
        //Web-Service Logic Can go-here
        var self = this;

        return objWS.Query("FetchFordTree", {}).success(function (data) {
            self.Plants = [];
            self.LoadRecord.IsLoaded = true;
            self.LoadRecord.LoadedAt = new Date();
            for (var j = 0; j < data.d.length; j++) {
                self.Plants.push(new FIS.Business.Types.Plant(data.d[j]));
                console.log(j);
            }

        });
    };
    FIS.Business.Types.Business.prototype.ToPrototype = function () {
        var _self = this;
        var typed_arr = [];
        for(var i = 0; i < _self.Plants.length; i++)
        {
            var typed_o = angular.extend(new FIS.Business.Types.Plant(), _self.Plants[i]);
            typed_o.ToPrototype();
            typed_arr.push(typed_o);
        }
        _self.Plants = typed_arr;
    };
}

FIS.Business.Types.LoadRecord = function () {
    this.IsLoaded = false;
    this.LoadedAt = null;
};