using CFT_Solutions.Core.Entity.CustomerMaster;
using CFT_Solutions.Core.Entity.Role;
using CFT_Solutions.Core.Entity.UserMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Service.CustomerMaster
{
    public interface ICustomerMasterService
    {
        List<CustomerMasterEntity> GetCustomerMasterDashBoardData(string SearchText);

        CustomerMasterEntity GetCustomerMasterByUserId(Int64 Id);
       object InsertAndUpdateCustomerMaster(CustomerMasterEntity entity);
       
    }
}
