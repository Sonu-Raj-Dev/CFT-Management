using CFT_Solutions.Core.Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.User
{
    public interface IUserService
    {
        UserEntity ValidateUser(string EmailID);
        UserEntity GetUserByLoginId(string LoginId);
        object ResetPassword(string EmailId, string Password, Int64 CreatedBy);
    }
}
