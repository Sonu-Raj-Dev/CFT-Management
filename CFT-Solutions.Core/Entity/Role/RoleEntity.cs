using CFT_Solutions.Core.Entity.PermissionRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Entity.Role
{
    public class RoleEntity : BaseEntity
    {


        #region Properties
        public ICollection<PermissionRecordEntity> _permissionRecords;

        public string RoleName { get; set; }

        public Int64 RoleId { get; set; }

        public Int64 RoleType { get; set; }

        public int UserType { get; set; }

        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the permission records
        /// </summary>
        public virtual ICollection<PermissionRecordEntity> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecordEntity>()); }
            set { _permissionRecords = value; }
        }

        #endregion



    }
}
