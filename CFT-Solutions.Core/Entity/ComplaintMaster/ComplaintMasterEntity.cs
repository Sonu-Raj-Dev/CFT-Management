using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CFT_Solutions.Core.Entity.ComplaintMaster
{
    public class ComplaintMasterEntity:BaseEntity
    {

        public string CustomerName { get; set; }
        public string ComplaintDetails { get; set; }
        public string MobileNumber { get; set; }
        public string StatusName { get; set; }
        public Int64 NatureOfComplaintId { get; set; }
        public string NatureOfComplaint { get; set; }
        public Int64 EngineerId { get; set; }
        public Int64 StatusId { get; set; }
        public string EngineerName { get; set; }
        public Int64 CustomerId { get;set; }
        public string CustomerEmail { get; set; }
            
        public string CreatedByUser { get; set; }
        public string ComplaintCode { get;set; }
        public string Address { get; set; }
        public string ActionType { get; set; }
        public string ModifiedByUser { get; set; }

        public string CreatedDateStr { get; set; }
        public string ModifiedDateStr { get; set; }
        public List<ComplaintMasterEntity> CustomersList { get; set; }
        public List<ComplaintMasterEntity> NatureOfComplaintsList { get; set; }
        public List<ComplaintMasterEntity> EngineerList { get; set; }
        public List<ComplaintMasterEntity> StatusList { get; set; }

    }
}
