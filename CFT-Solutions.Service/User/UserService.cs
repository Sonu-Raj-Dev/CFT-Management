using CFT_Solutions.Core.Entity.Common;
using CFT_Solutions.Core.Entity.PermissionRecord;
using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Entity.User;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.User
{
    public class UserService:IUserService
    {
        #region Fields

        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<RoleEntity> _userRoleRepository;
        private readonly IRepository<PermissionRecordEntity> _permissionRecordRepository;

  
        #endregion

        #region Constructor
        public UserService(
            IRepository<UserEntity> userRepository,
            IRepository<RoleEntity> userRoleRepository,
            IRepository<PermissionRecordEntity> permissionRecordRepository
          
        )
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _permissionRecordRepository = permissionRecordRepository;
           
        }

        #endregion

        #region Methods
        public UserEntity ValidateUser(string EmailID)
        {
            UserEntity data = new UserEntity();
            try
            {
                SqlCommand command = new SqlCommand("stp_ValidateUserByEmailId");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = EmailID;
                data = _userRepository.GetRecord(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }
        public UserEntity GetUserByLoginId(string LoginId)
        {
            UserEntity user = new UserEntity();
            try
            {

                SqlCommand command = new SqlCommand("stp_GetUserDetailsByLoginId");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@LoginId", SqlDbType.VarChar).Value = LoginId;
                user = _userRepository.GetRecord(command);

                if (user != null)
                {
                    List<RoleEntity> Role = new List<RoleEntity>();
                    SqlCommand command1 = new SqlCommand("stp_GetUserRoleBYUserID");
                    command1.CommandType = System.Data.CommandType.StoredProcedure;
                    command1.Parameters.Add("@UserId", SqlDbType.VarChar).Value = user.Id;
                    Role = _userRoleRepository.GetRecords(command1).ToList();
                    if (Role.Count > 0)
                    {
                        user.UserRoles = Role;
                        user.DefaultRoleId = Role[0].RoleId;
                        user.DefaultRoleName = Role[0].RoleName;
                        SqlCommand command2 = new SqlCommand("spGetPermissionOnRoleID");
                        command2.CommandType = System.Data.CommandType.StoredProcedure;
                        command2.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = Role[0].RoleId;
                        // user.PermissionRecords = _permissionRecordRepository.GetRecords(command2).ToList();
                        Role[0]._permissionRecords = _permissionRecordRepository.GetRecords(command2).ToList();
                        user.DefaultPermissions = Role[0]._permissionRecords.Select(k => k.SystemName).ToList();                       
                    }                                
                }
                else
                {
                    user = null;
                }

            }
            catch (Exception ex)
            {
                ErrorLogEntity error = new ErrorLogEntity();
                error.ControllerName = "UserService";
                error.ActionName = "GetUserByLoginId";
                error.Exception = ex.Message;
                error.StackTrace = ex.StackTrace;
                LogHelper.LogError(error);
            }

            return user;
        }
        #endregion

    }
}
