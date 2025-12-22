using CFT_Solutions.Core.Entity.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Entity.User
{
    public class UserEntity:BaseEntity
    {
        public List<RoleEntity> UserRoles { get; set; }

        public List<string> DefaultPermissions { get; set; }
        public string DefaultRoleName { get; set; }
        public Int64 DefaultRoleId { get; set; }
        public string EmailID { get; set; }
        public string LoginId { get; set; }
    }
}
