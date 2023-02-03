using CBSPay.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class UserrRecord : BaseEntity<int>, IUser<int>
    { 
        public virtual string UserName { get; set; }

        public virtual string PasswordHash { get; set; }
    }

    public class Role : BaseEntity<int>, IRole<int>
    { 
        public virtual string Name { get; set; }
 
    }
}
