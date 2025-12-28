using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Entity.CustomerMaster
{
    public class CustomerMasterEntity:BaseEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }    // added
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public int EmployeeType { get; set; }

    }
}
