using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FISLogging
/// </summary>
namespace FISX
{
    public class FISLogging
    {
        public static void Log(string message)
        {
            try
            {
                HttpContext.Current.Response.AppendToLog(message);
                Trace(message);
            }
            catch (Exception ex1)
            {

            }
        }

        public static void Trace(string message, string category = "debug", Exception ex = null)
        {
            try
            {
                HttpContext.Current.Trace.Write(category, message, null);
            }
            catch (Exception ex2)
            {

            }
        }

        public static void LogException(Exception ex, FIS2GoCommonParameter p)
        {
            FISLogging.Log("[E] Exception Encountered: " + ex.Message);
            FISLogging.Log("[E-P] " + p.ToParameterLogString());
            FISLogging.Log("[E-ST] " + ex.StackTrace);
            HttpContext.Current.Response.StatusCode = 500;
        }
    }
}