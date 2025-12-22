using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Configuration
{
    public class ConnectionStrings
    {
        public string DataSource { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ProviderName { get; set; }
    }

    public class UserDomain
    {
        public string Dev { get; set; }
        public string UAT { get; set; }
        public string Prod { get; set; }
    }

    public static class CommonRegEx
    {
        public static string TextSecurityRegEx { get; set; }
    }
}
