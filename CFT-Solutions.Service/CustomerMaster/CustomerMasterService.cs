using CFT_Solutions.Core.Entity.Common;
using CFT_Solutions.Core.Entity.CustomerMaster;
using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Entity.UserMaster;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.CustomerMaster
{
    public class CustomerMasterService:ICustomerMasterService
    {
        #region Fields
        private readonly IRepository<CustomerMasterEntity> _userMasterRepository;
        private readonly IRepository<RoleEntity> _roleRepository;
        #endregion

        #region Constructor
        public CustomerMasterService(IRepository<CustomerMasterEntity> userRepository, IRepository<RoleEntity> roleRepository)
        {
            _userMasterRepository = userRepository;
            _roleRepository = roleRepository;
        }
        #endregion

        #region Methods
        public List<CustomerMasterEntity> GetCustomerMasterDashBoardData(string SearchText)
        {
            List<CustomerMasterEntity> data = new List<CustomerMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stpGetCustomerMaster");
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
        public CustomerMasterEntity GetCustomerMasterByUserId(Int64 Id)
        {
            CustomerMasterEntity data = new CustomerMasterEntity();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetCustomerMasterById");
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
      
        public object InsertAndUpdateCustomerMaster(CustomerMasterEntity entity)
        {
            try
            {
                SqlCommand command = new SqlCommand("stp_InsertUpdateCustomerMaster");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@Id", SqlDbType.BigInt).Value = entity.Id;
                command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = entity.FirstName ?? string.Empty;
                command.Parameters.Add("@LastName", SqlDbType.VarChar).Value = entity.LastName ?? string.Empty;
                command.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = entity.EmailId ?? string.Empty;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = entity.MobileNo ?? string.Empty;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = entity.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = entity.CreatedBy;
              
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
