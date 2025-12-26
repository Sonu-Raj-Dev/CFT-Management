
using CFT_Solutions.Core.Entity.Common;
using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Entity.UserMaster;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CFT_Solutions.Service.UserMaster
{
    public class UserMasterService : IUserMasterService
    {
        #region Fields
        private readonly IRepository<UserMasterEntity> _userMasterRepository;
        private readonly IRepository<RoleEntity> _roleRepository;
        #endregion

        #region Constructor
        public UserMasterService(IRepository<UserMasterEntity> userRepository, IRepository<RoleEntity> roleRepository)
        {
            _userMasterRepository = userRepository;
            _roleRepository = roleRepository;
        }
        #endregion

        #region Methods
        public List<UserMasterEntity> GetUserMasterDashBoardData(string SearchText)
        {
            List<UserMasterEntity> data = new List<UserMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stpGetUserMaster");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@SearchText", SqlDbType.VarChar).Value = SearchText ?? string.Empty;
                data = _userMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public UserMasterEntity GetUserMasterByUserId(Int64 Id)
        {
            UserMasterEntity data = new UserMasterEntity();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetUserMasterById");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Id", SqlDbType.BigInt).Value = Id;
                data = _userMasterRepository.GetRecord(command);
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public List<RoleEntity> GetRoles()
        {
            List<RoleEntity> data = new List<RoleEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetRoles");
                command.CommandType = CommandType.StoredProcedure;
                data = _roleRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public List<UserMasterEntity> GetUsersByRoleId(Int64 RoleId)
        {
            List<UserMasterEntity> data = new List<UserMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetUserByRoleId");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@RoleId", SqlDbType.BigInt).Value = RoleId;
                data = _userMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public object InsertAndUpdateUserMaster(UserMasterEntity entity)
        {
            try
            {
                SqlCommand command = new SqlCommand("stp_InsertUpdateUserMaster");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@Id", SqlDbType.BigInt).Value = entity.Id;
                command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = entity.FirstName ?? string.Empty;
                command.Parameters.Add("@LastName", SqlDbType.VarChar).Value = entity.LastName ?? string.Empty;
                command.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = entity.EmailId ?? string.Empty;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = entity.MobileNo ?? string.Empty;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = entity.Password ?? string.Empty;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = entity.CreatedBy;
                command.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = entity.EmployeeType;

                var data = _userMasterRepository.ExecuteProc(command);
                return data;
            }
            catch (Exception ex)
            {
                ErrorLogEntity error = new ErrorLogEntity();
                error.ControllerName = "UserMasterService";
                error.ActionName = "InsertAndUpdateUserMaster";
                error.Exception = ex.Message;
                error.StackTrace = ex.StackTrace;
                LogHelper.LogError(error);
                throw;
            }
        }
        #endregion
    }
}
