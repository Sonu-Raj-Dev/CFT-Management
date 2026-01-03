using CFT_Solutions.Core.Entity.ComplaintMaster;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CFT_Solutions.Service.ComplaintMster
{
    public interface IComplaintService
    {  
        List<ComplaintMasterEntity> GetComplaintMasterDashBoardData(string SerachText,Int64 CurruntUserId);
        ComplaintMasterEntity GetComplaintMasterDataById(Int64 Id);
        object InsertUpdateComplaintMaster(ComplaintMasterEntity complaintMasterEntity);
        List<ComplaintMasterEntity> GetCustomer();
        List<ComplaintMasterEntity> GetNatureOfComplaint();
        List<ComplaintMasterEntity> GetEngineerList();
        object UpdateComplaintStatusById(Int64 ComplaintId, Int64 UpdatedBy,string Remark);
    }
}
