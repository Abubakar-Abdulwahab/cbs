using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class TaxEntityCategory : CBSModel
    {
        public virtual string Name { get; set; }
        public virtual int Identifier { get; set; }
        public virtual bool Status { get; set; }
    }
}
