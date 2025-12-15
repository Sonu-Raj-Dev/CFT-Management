using CFT_Solutions.Core.Entity.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CFT_Solutions.Core.Helper
{
    public static class LogHelper
    {
        private static readonly ILog _errorlog = LogManager.GetLogger(Assembly.GetEntryAssembly(), "ErrorLog");
        private static readonly ILog _successlog = LogManager.GetLogger(Assembly.GetEntryAssembly(), "SuccessLog");

        public static void InitLog(string ConfigPath)
        {
            string HourID = DateTime.Now.ToString("hhtt");
            string dateMask = DateTime.Today.ToString("ddMMyyyy");
            log4net.GlobalContext.Properties["CurDate"] = dateMask;
            log4net.GlobalContext.Properties["ErrorLogName"] = "ErrorLog-" + HourID;
            log4net.GlobalContext.Properties["SuccessLogName"] = "SuccessLog" + HourID;
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(ConfigPath));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            //if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["Log4Net-ConfigFile"]) && !String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Log4Net-ConfigFile"]))
            //{
            //   log4net.Config.XmlConfigurator.ConfigureAndWatch(,new System.IO.FileInfo(ConfigurationManager.AppSettings["Log4Net-ConfigFile"].Trim())); // External log4net.config file path
            //}
        }

        public static void LogError(ErrorLogEntity error)
        {
            _errorlog.Error("<----------------Log Start " + error.ErrorGuid + " ------------------>");
            _errorlog.Error("Error Log Service :");
            _errorlog.Error("Error ActionName:" + error.ActionName);
            _errorlog.Error(" Error ControllerName: " + error.ControllerName);
            _errorlog.Error(" Error Source: " + error.Source);
            _errorlog.Error(" Error TargetSite: " + error.TargetSite);
            _errorlog.Error(" Error Message:" + error.Message);
            _errorlog.Error(" Error Exception:" + error.Exception);
            _errorlog.Error(" Error StackTrace: " + error.StackTrace);
            _errorlog.Error(" Error InnerException: " + error.InnerException);
            _errorlog.Error(" Error InnerException StackTrace: " + error.ExceptionMessage);
            _errorlog.Error(" Error DateTime:" + DateTime.UtcNow);

            //_errorlog.Error(JsonConvert.SerializeObject(error, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            _errorlog.Error("<----------------Log End------------------>");
        }


        public static void LogError(Exception ex)
        {
            _errorlog.Error("System Error :" + ex.Message, ex);
        }

        public static void LogError(string message)
        {
            _errorlog.Error("Custom Error :" + message);
        }

        public static void LogError(Exception ex, string message)
        {
            LogError(message);
            LogError(ex);
        }

        public static void LogSuccess(string message)
        {
            _successlog.Info(message);
        }
    }
}
