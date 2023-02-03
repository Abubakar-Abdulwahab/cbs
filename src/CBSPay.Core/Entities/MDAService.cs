using CBSPay.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class MDAService : BaseEntity<long>
    {
        public virtual long SBSID { get; set; }
        public virtual long MDAServiceID { get; set; }
        public virtual string MDAServiceName { get; set; }        
        public virtual int TaxYear { get; set; }
        public virtual decimal? ServiceAmount { get; set; }
        public virtual decimal? SettledAmount { get; set; }
        [JsonIgnore]
        public virtual ServiceBillResult ServiceBill { get; set; }

        protected internal virtual void CopyFrom(MDAService item)
        {
            this.SBSID = item.SBSID;
            this.ServiceAmount = item.ServiceAmount;
            this.SettledAmount = item.SettledAmount;
            this.DateModified = DateTime.Now;
            this.MDAServiceName = item.MDAServiceName;
            this.MDAServiceID = item.MDAServiceID;
            this.TaxYear = item.TaxYear;
        }
    }
}
