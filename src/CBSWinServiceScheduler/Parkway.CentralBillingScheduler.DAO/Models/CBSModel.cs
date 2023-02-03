using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public abstract class CBSModel : CBSBaseModel
    {
        public virtual int Id { get; set; }
    }
}
