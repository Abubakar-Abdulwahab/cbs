using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Models
{
    public abstract class BaseEntity<T>
    {
        public virtual T Id { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual bool IsDeleted { get; set; }
    }

}
