using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Entity.UserMaster;
using System.Collections.Generic;

namespace CFT_Solutions.Service.UserMaster
{
    public interface IUserMasterService
    {
        List<UserMasterEntity> GetUserMasterDashBoardData(string SearchText);

        UserMasterEntity GetUserMasterByUserId(Int64 Id);
        List<RoleEntity> GetRoles();
        List<UserMasterEntity> GetUsersByRoleId(Int64 RoleId);
        object InsertAndUpdateUserMaster(UserMasterEntity entity);
    }
}
