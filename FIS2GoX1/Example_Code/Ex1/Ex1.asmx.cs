using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace FIS2GoX1.Example_Code.Ex1
{
    /// <summary>
    /// Summary description for Ex1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Ex1 : System.Web.Services.WebService
    {
        //======================
        //  Static members of a web-service will persist between calls to the web-service
        //  so for resources which don't change, we can use this as a caching method
        //  however, the webservice lives as long as the ASP.NET Application Pool
        //  which *may* recycle worker processes every 20 mins, so it's dangerous to rely on it
        //======================
        public static Random r;
        public Ex1()
        {
            r = new Random();
        }

        //======================
        //  We create 1 Web-Service Function, which accepts 2 arguments...
        //  the asp.net pipeline will try to match those arguments by-name and type
        //  where we have custom objects, it will look for a JavaScript object called "cArg"
        //  and populate it's members with the members of the Javascript object
        //======================
        [WebMethod]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json)] //ASMX web-service are ANCIENT and were originally designed for SOAP (XML) based communication, we'd rather use JSON
        public ComplexResponseObject Example1(int NumberToSquare, ComplexInputArgument cArg)
        {
            ComplexResponseObject response_obj = new ComplexResponseObject();
            response_obj.Greeting = String.Format("Hello {0}", cArg.Username);
            response_obj.doubled_number = NumberToSquare * NumberToSquare; //squared or doubled? :)
            response_obj.yesterday = cArg.dt.AddDays(-1).ToShortDateString();
            response_obj.today = DateTime.Now.ToShortDateString();
            response_obj.tomorrow = cArg.dt.AddDays(1).ToShortDateString();


            Random r = new Random();
            if(r.Next(100) > 90)
            {
                throw new Exception("Example 1 has thrown a random exception!!");
            }


            System.Threading.Thread.Sleep(1000 * 2); //Force the thread to pause to simulate loading time

            return response_obj;
        }
    }

    public class ComplexInputArgument
    {
        public DateTime dt;
        public String Username;
    }
    public class ComplexResponseObject
    {
        public string Greeting;
        public int doubled_number;
        public string yesterday;
        public string today;
        public string tomorrow;
    }





}
