using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Entity.UserMaster
{
    public class UserMasterEntity:BaseEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }    // added
        public string LastName { get; set; }
        public string LoginId { get; set; }       // added
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public Int64 RoleId { get; set; }   
        public Int64 EngineerId { get; set; }     // added to bind selected engineer
        public int EmployeeType { get; set; }


    }
}
