using CFT_Solutions.Core.Entity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        UserEntity CurrentUser { get; set; }

        /// <summary>
        /// AuthenticationType
        /// </summary>
        /// <value></value>
        int AuthenticationType { get; set; }
    }
}
