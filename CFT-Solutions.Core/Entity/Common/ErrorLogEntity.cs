using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Entity.Common
{
    public class ErrorLogEntity : BaseEntity
    {
        #region Properties

        public Guid ErrorGuid { get; set; }

        public string Source { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string RequestType { get; set; }

        public string TargetSite { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Exception { get; set; }

        public DateTime ErrorDateTime { get; set; }

        public string FunctionName { get; set; }

        public string HResult { get; set; }

        public string InnerException { get; set; }

        public string ExceptionMessage { get; set; }

        #endregion
    }
}
