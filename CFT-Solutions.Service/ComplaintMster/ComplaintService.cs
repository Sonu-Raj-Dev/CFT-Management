using CFT_Solutions.Core.Entity.Common;
using CFT_Solutions.Core.Entity.ComplaintMaster;
using CFT_Solutions.Core.Entity.CustomerMaster;
using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Helper;
using CFT_Solutions.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.ComplaintMster
{
    public class ComplaintService:IComplaintService
    {
        #region Fields
        private readonly IRepository<ComplaintMasterEntity> _complaintMasterRepository;
        private readonly IRepository<RoleEntity> _roleRepository;
        #endregion

        #region Constructor
        public ComplaintService(IRepository<ComplaintMasterEntity> complaintRepository, IRepository<RoleEntity> roleRepository)
        {
            _complaintMasterRepository = complaintRepository;
            _roleRepository = roleRepository;
        }
        #endregion

        #region Methods
        public List<ComplaintMasterEntity> GetCustomer()
        {
            List<ComplaintMasterEntity> data = new List<ComplaintMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetCustomer");
                command.CommandType = CommandType.StoredProcedure;    
                data = _complaintMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public List<ComplaintMasterEntity> GetNatureOfComplaint()
        {
            List<ComplaintMasterEntity> data = new List<ComplaintMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetNatureOfComplaint");
                command.CommandType = CommandType.StoredProcedure;
                data = _complaintMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public List<ComplaintMasterEntity> GetEngineerList()
        {
            List<ComplaintMasterEntity> data = new List<ComplaintMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetEngineerList");
                command.CommandType = CommandType.StoredProcedure;
                data = _complaintMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }

        public List<ComplaintMasterEntity> GetComplaintMasterDashBoardData(string SearchText,Int64 CurruntUserId)
        {
            List<ComplaintMasterEntity> data = new List<ComplaintMasterEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stpGetComplaintMaster");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@SearchText", SqlDbType.VarChar).Value = SearchText ?? string.Empty;
                command.Parameters.Add("@CurruntUserId", SqlDbType.BigInt).Value = CurruntUserId;

                data = _complaintMasterRepository.GetRecords(command).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }
        public ComplaintMasterEntity GetComplaintMasterDataById(Int64 Id)
        {
            ComplaintMasterEntity data = new ComplaintMasterEntity();
            try
            {
                SqlCommand command = new SqlCommand("stp_GetComplaintMasterById");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Id", SqlDbType.BigInt).Value = Id;
                data = _complaintMasterRepository.GetRecord(command);
            }
            catch (Exception)
            {
                throw;
            }
            return data;
        }

        public object InsertUpdateComplaintMaster(ComplaintMasterEntity entity)
        {
            try
            {
                SqlCommand command = new SqlCommand("stp_InsertUpdateComplaintMaster");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@Id", SqlDbType.BigInt).Value = entity.Id;
                command.Parameters.Add("@CustomerId", SqlDbType.BigInt).Value = entity.CustomerId;
                command.Parameters.Add("@NatureOfComplaintId", SqlDbType.BigInt).Value = entity.NatureOfComplaintId;
                command.Parameters.Add("@ComplaintDetails", SqlDbType.VarChar).Value = entity.ComplaintDetails;
                command.Parameters.Add("@EngineerId", SqlDbType.BigInt).Value = entity.EngineerId;
                command.Parameters.Add("@StatusId", SqlDbType.BigInt).Value = entity.StatusId;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
                command.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = entity.CreatedBy;

                var data = _complaintMasterRepository.ExecuteProc(command);
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
        public object UpdateComplaintStatusById(Int64 ComplaintId, Int64 UpdatedBy)
        {
            try
            {
                SqlCommand command = new SqlCommand("stp_UpdateComplaintStatusById");
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("@ComplaintId", SqlDbType.BigInt).Value = ComplaintId;
                command.Parameters.Add("@UpdatedBy", SqlDbType.BigInt).Value = UpdatedBy;

                var data = _complaintMasterRepository.ExecuteProc(command);
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
