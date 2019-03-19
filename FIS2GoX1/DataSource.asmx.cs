using FIS2GoShared;
using FIS2GoShared.Report_Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Services;
using System.Web.Services;

namespace FISX
{
    /// <summary>
    /// Summary description for DataSource
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DataSource : System.Web.Services.WebService
    {

        public FISXMLReader xml_reader;
        public static string xml_path;

        public DataSource()
        {
            //this.xml_reader = new FISXMLReader(Server.MapPath("./resources/"));
            FISXMLReader.DefaultPath = Server.MapPath("./resources/xml/");
        }

        [WebMethod()]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<Plant> FetchFordTree()
        {
            try
            {
                List<Plant> ret = PlantRepository.GetPlants(true);
                FISLogging.Log("[200-OK] Params: ");

                return ret;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: ");
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 300)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public String GetODBServer(string SiteID)
        {
            try
            {
                String retval = AreaRepository.GetODBServer(SiteID);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 60)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public Area FetchAreaDetails(string SiteID, string AREA_SAKEY)
        {
            try
            {
                Area retval = AreaRepository.GetArea(SiteID, AREA_SAKEY);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }


        [WebMethod(CacheDuration = 60)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FISNode FetchNodeDetails(FIS2GoCommonParameter p)
        {
            try
            {
                FISNode retval = FISNodeRepository.GetNode(p.SuperPath); // .GetArea(SiteID, AREA_SAKEY);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] " + p.ToParameterLogString());
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 60)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<String> FetchGroupChildNodeID(string SiteID, string AREA_SAKEY, string GroupID)
        {
            try
            {
                List<String> retval = FISNodeRepository.GetNodeIDChildren(AreaRepository.GetODBServer(SiteID), AREA_SAKEY, GroupID);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", GroupID = " + GroupID);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", GroupID = " + GroupID);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<FISNodeMetric> FetchJPH(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<FISNodeMetric> response = new FIS2GoResponseObject<FISNodeMetric>();
            response.Params = p;
            try
            {
                FISNodeMetric metric = new FISNodeMetric(p.SiteID, p.AREA_SAKEY, p.NodeID);
                if (p.Period == "CSPS")
                {
                    p.Period = "CS";
                    metric.FetchJPH(p);
                    p.Period = "PS";
                    metric.FetchJPH(p);
                }
                else
                {
                    metric.FetchJPH(p);
                }

                FISLogging.Log("[200-OK] " + p.ToParameterLogString());

                response.Value = metric;
                response.Was_Successful = true;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<List<EventRecord>> FetchCycleDetails(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<List<EventRecord>> response = new FIS2GoResponseObject<List<EventRecord>>();
            response.Params = p;
            try
            {
                List<EventRecord> retval = ReportDatasetRepository.GetCycleTimeDetails(p);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = retval;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        /*
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DataBinGroup FetchCycleBins_Flex(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period = "CSPS")
        {
            try
            {
                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                DataBinGroup retval = ReportDatasetRepository.GetCycleTimeHistogram(metric, !ProdTimeOnly, Period);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<List<MachineStateAccum>> FetchStateBins(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<List<MachineStateAccum>> response = new FIS2GoResponseObject<List<MachineStateAccum>>();
            response.Params = p;
            try
            {
                List<MachineStateAccum> retval = ReportDatasetRepository.GetMachineStateAccumulators(p);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = retval;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<FISNodeMetric> FetchProdCounts(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<FISNodeMetric> response = new FIS2GoResponseObject<FISNodeMetric>();
            response.Params = p;
            try
            {
                FISNodeMetric metric = new FISNodeMetric(p.SiteID, p.AREA_SAKEY, p.NodeID);
                if (p.Period == "CSPS")
                {
                    p.Period = "CS";
                    metric.FetchGPCounts(p);
                    p.Period = "PS";
                    metric.FetchGPCounts(p);
                }
                else
                {
                    metric.FetchGPCounts(p);
                }

                FISLogging.Log("[200-OK] " + p.ToParameterLogString());

                response.Value = metric;
                response.Was_Successful = true;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<FISNodeMetric> FetchAverageCycleTime(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<FISNodeMetric> response = new FIS2GoResponseObject<FISNodeMetric>();
            response.Params = p;
            try
            {
                FISNodeMetric metric = new FISNodeMetric(p.SiteID, p.AREA_SAKEY, p.NodeID);
                if (p.Period == "CSPS")
                {
                    p.Period = "CS";
                    metric.FetchAverageCycleTime(p);
                    p.Period = "PS";
                    metric.FetchAverageCycleTime(p);
                }
                else
                {
                    metric.FetchAverageCycleTime(p);
                }

                FISLogging.Log("[200-OK] " + p.ToParameterLogString());

                response.Value = metric;
                response.Was_Successful = true;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        /*
        [WebMethod(CacheDuration=5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultByDuration_FilterString(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period, string strFilterString = "")
        {
            try {
                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                List<IncidentRecord> retval = ReportDatasetRepository.GetIncidentsByDuration(metric, !ProdTimeOnly, Period, strFilterString);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterString);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterString);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }

        }
        */

        /*
        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultByDuration(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period, bool hasMachineFaults, bool hasManualInterventionFaults, bool HasWarnings)
        {
            try
            {
                List<String> flags = new List<string>();
                if (hasMachineFaults)
                {
                    flags.Add("Machine Faults");
                }
                if (hasManualInterventionFaults)
                {
                    flags.Add("Manual Intervention Faults");
                }
                if (HasWarnings)
                {
                    flags.Add("Warnings");
                }
                String strFilterStrings = String.Join("|", flags);

                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                List<IncidentRecord> retval = ReportDatasetRepository.GetIncidentsByDuration(metric, !ProdTimeOnly, Period, strFilterStrings);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterStrings);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }

        }
        */

        /*
        [WebMethod(CacheDuration=5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultByOcc_FilterString(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period, string strFilterString = "")
        {
            try {
                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                List<IncidentRecord> retval = ReportDatasetRepository.GetIncidentsByOccurances(metric, !ProdTimeOnly, Period, strFilterString);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterString);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterString);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        /*
        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultByOcc(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period, bool hasMachineFaults, bool hasManualInterventionFaults, bool HasWarnings)
        {
            try
            {
                List<String> flags = new List<string>();
                if (hasMachineFaults)
                {
                    flags.Add("Machine Faults");
                }
                if (hasManualInterventionFaults)
                {
                    flags.Add("Manual Intervention Faults");
                }
                if (HasWarnings)
                {
                    flags.Add("Warnings");
                }
                String strFilterStrings = String.Join("|", flags);
                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                List<IncidentRecord> retval = ReportDatasetRepository.GetIncidentsByOccurances(metric, !ProdTimeOnly, Period, strFilterStrings);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period + ", Filter = " + strFilterStrings);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period );
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        /*
        [WebMethod(CacheDuration=5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<CycleTimeBin> FetchCycleTimeBins(string NodeID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period)
        {
            try {
                FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                List<CycleTimeBin> retval = ReportDatasetRepository.GetCycleTimeByBinning(metric, !ProdTimeOnly, Period);
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                return retval;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", NodeID = " + NodeID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<List<List<EventStatistics>>> FetchCycleTimeStatistics(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<List<List<EventStatistics>>> response = new FIS2GoResponseObject<List<List<EventStatistics>>>();
            response.Params = p;
            try
            {
                List<FIS2GoManagedException> lst_ex;
                List<List<EventStatistics>> retval = ReportDatasetRepository.GetCycleTimeStatistics_Flex(p, out lst_ex);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = retval;
                lst_ex.ForEach(x => response.lst_ex.Add(x.GetSerializable()));
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        /*
        [WebMethod(CacheDuration=5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<MachineStateAccum> FetchStateBins_L(string GroupID, string SiteID, string AREA_SAKEY, bool ProdTimeOnly, string Period)
        {
            try {
                List<Task<List<MachineStateAccum>>> TaskList = new List<Task<List<MachineStateAccum>>>();
                foreach(String NodeID in FISNodeRepository.GetNodeIDChildren(AreaRepository.GetODBServer(SiteID), AREA_SAKEY, GroupID))
                {
                    Task<List<MachineStateAccum>> LastTask = new Task<List<MachineStateAccum>>(() => FetchStateBins(NodeID, SiteID, AREA_SAKEY, ProdTimeOnly, Period));
                    LastTask.Start();
                    TaskList.Add(LastTask);
                }
                Task.WaitAll(TaskList.ToArray());

                List<MachineStateAccum> ret_arr = new List<MachineStateAccum>();
                foreach (Task<List<MachineStateAccum>> t in TaskList)
                {
                    ret_arr.AddRange(t.Result);
                }
                FISLogging.Log("[200-OK] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", GroupoID = " + GroupID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                return ret_arr;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: SiteID = " + SiteID + ", AREASAKEY = " + AREA_SAKEY + ", GroupID = " + GroupID + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public string GetBookmark(String base_url, String page, ClientSelections selections)
        {
            try
            {
                try
                {
                    if (WebConfigurationManager.AppSettings["XFordMapping_Enabled"].ToString() == "1" || WebConfigurationManager.AppSettings["XFordMapping_Enabled"].ToString().ToUpper() == "TRUE")
                    {
                        base_url = base_url.Replace(WebConfigurationManager.AppSettings["XFordMapping_Key"].ToString(), WebConfigurationManager.AppSettings["XFordMapping_Value"].ToString());
                    }
                }
                catch (Exception e)
                {

                }
                FISBookmark b = new FISBookmark(selections);
                //return base_url + "?b=" + b.GetBookmark();
                return base_url + "wsl/WSLHandler.aspx?page=" + page + "&data=" + b.GetBookmark() + "&ts=" + DateTime.Now.Second;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: selections = " + selections.ToString());
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public ClientSelections LoadBookmark(String data)
        {
            try
            {
                FISBookmark b = new FISBookmark(null);
                b.LoadBookmark(data);
                b.selections.lst_strSelectedNodeSuperPaths = b.selections.lst_strSelectedNodeSuperPaths.Distinct().ToList();
                return b.selections;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: data = " + data);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public ClientSelections BookmarkOnDemand(String type, List<String> payload)
        {
            try
            {
                ClientSelections c = new ClientSelections();
                foreach (String arg in payload)
                {
                    c.lst_strSelectedNodeSuperPaths.AddRange(FISBookmark.ParseOnDemandBookmark(type, arg));
                }
                c.lst_strSelectedNodeSuperPaths = c.lst_strSelectedNodeSuperPaths.Distinct().ToList();
                return c;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params: Type = " + type + ", Payload: " + String.Join("//", payload));
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<List<List<OvercycleReport>>> FetchOvercyclePercentRanks(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<List<List<OvercycleReport>>> response = new FIS2GoResponseObject<List<List<OvercycleReport>>>();
            response.Params = p;
            try
            {
                List<FIS2GoManagedException> lst_ex;
                List<List<OvercycleReport>> ret_lst = ReportDatasetRepository.GetTopOvercycle_Flex(p, out lst_ex);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Value = ret_lst;
                response.Was_Successful = true;
                lst_ex.ForEach(x => response.lst_ex.Add(x.GetSerializable()));
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<FISNodeMetric> FetchOvercyclePercent(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<FISNodeMetric> response = new FIS2GoResponseObject<FISNodeMetric>();
            response.Params = p;
            try
            {
                FISNodeMetric metric = new FISNodeMetric(p.SiteID, p.AREA_SAKEY, p.NodeID);
                if (p.Period == "CSPS")
                {
                    p.Period = "CS";
                    metric.FetchMedianCycleTime(p);
                    p.Period = "PS";
                    metric.FetchMedianCycleTime(p);
                }
                else
                {
                    metric.FetchMedianCycleTime(p);
                }

                FISLogging.Log("[200-OK] " + p.ToParameterLogString());

                response.Value = metric;
                response.Was_Successful = true;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<FISNodeMetric> FetchAvailability(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<FISNodeMetric> response = new FIS2GoResponseObject<FISNodeMetric>();
            response.Params = p;
            try
            {
                FISNodeMetric metric = new FISNodeMetric(p.SiteID, p.AREA_SAKEY, p.NodeID);
                if (p.Period == "CSPS")
                {
                    p.Period = "CS";
                    metric.FetchAvailability(p);
                    p.Period = "PS";
                    metric.FetchAvailability(p);
                }
                else
                {
                    metric.FetchAvailability(p);
                }

                FISLogging.Log("[200-OK] " + p.ToParameterLogString());

                response.Value = metric;
                response.Was_Successful = true;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<List<RateOfClimbRecord>> FetchRateOfClimbData(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<List<RateOfClimbRecord>> response = new FIS2GoResponseObject<List<RateOfClimbRecord>>();
            response.Params = p;
            try
            {
                List<RateOfClimbRecord> retval = ReportDatasetRepository.GetRateOfClimb(p);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = retval;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<DataBinGroup> FetchCycleTimeBins_Dev(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<DataBinGroup> response = new FIS2GoResponseObject<DataBinGroup>();
            response.Params = p;
            try
            {
                DataBinGroup retval = ReportDatasetRepository.GetCycleTimeByBinning(p);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = retval;
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }

        /*
        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultsByDuration_Flex(List<string> SuperPaths, bool ProdTimeOnly, string Period, string strFilterString)
        {
            try 
            {
                List<IncidentRecord> ret_lst = new List<IncidentRecord>();
                List<FISNodeMetric> nodes = new List<FISNodeMetric>();
                foreach (String SuperPath in SuperPaths)
                {
                    string[] parts = SuperPath.Split('|');
                    String SiteID = parts[0];
                    String AREA_SAKEY = parts[1];
                    String NodeID = parts[2];
                    FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                    nodes.Add(metric);
                }
                ret_lst = ReportDatasetRepository.GetIncidentsByDuration_Flex(nodes, !ProdTimeOnly, Period, strFilterString);
                FISLogging.Log("[200-OK] Params: SuperPaths = " + String.Join(",", SuperPaths) + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                return ret_lst;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params:  SuperPaths = " + String.Join(",", SuperPaths) + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }*/

        /*
        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public List<IncidentRecord> FetchFaultsByOccurance_Flex(List<string> SuperPaths, bool ProdTimeOnly, string Period, string strFilterString)
        {
            try
            {
                List<IncidentRecord> ret_lst = new List<IncidentRecord>();
                List<FISNodeMetric> nodes = new List<FISNodeMetric>();
                foreach (String SuperPath in SuperPaths)
                {
                    string[] parts = SuperPath.Split('|');
                    String SiteID = parts[0];
                    String AREA_SAKEY = parts[1];
                    String NodeID = parts[2];
                    FISNodeMetric metric = new FISNodeMetric(SiteID, AREA_SAKEY, NodeID);
                    nodes.Add(metric);
                }
                ret_lst = ReportDatasetRepository.GetIncidentsByOccurance_Flex(nodes, !ProdTimeOnly, Period, strFilterString);
                FISLogging.Log("[200-OK] Params: SuperPaths = " + String.Join(",", SuperPaths) + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                return ret_lst;
            }
            catch (Exception ex)
            {
                FISLogging.Log("[E] Exception Encountered: " + ex.Message);
                FISLogging.Log("[E-P] Params:  SuperPaths = " + String.Join(",", SuperPaths) + ", ProdTimeOnly = " + ProdTimeOnly + ", Period = " + Period);
                FISLogging.Log("[E-ST] " + ex.StackTrace);
                HttpContext.Current.Response.StatusCode = 500;
                throw ex;
            }
        }
        */

        [WebMethod(CacheDuration = 5)]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public FIS2GoResponseObject<IncidentRankContainer> FetchTopFaults(FIS2GoCommonParameter p)
        {
            FIS2GoResponseObject<IncidentRankContainer> response = new FIS2GoResponseObject<IncidentRankContainer>();
            response.Params = p;
            try
            {
                List<FIS2GoManagedException> lst_ex;
                IncidentRankContainer ret_cont = ReportDatasetRepository.GetIncidentsUnified(p, out lst_ex);
                FISLogging.Log("[200-OK] " + p.ToParameterLogString());
                response.Was_Successful = true;
                response.Value = ret_cont;
                lst_ex.ForEach(x => response.lst_ex.Add(x.GetSerializable()));
            }
            catch (FIS2GoManagedException mex)
            {
                FISLogging.LogException(mex, p);
                response.Was_Successful = false;
                mex.context = p;
                response.AddException(mex);
            }
            catch (Exception ex)
            {
                FISLogging.LogException(ex, p);
                response.Was_Successful = false;
                response.AddException(new FIS2GoManagedException(ex.Message, false, p));
            }
            return response;
        }


        [WebMethod]
        public int ping(String username, string curr_page = "Unknown")
        {
            FISLogging.Log("[P] " + username + "| [V] " + curr_page);
            return 1;
        }

    }

}
