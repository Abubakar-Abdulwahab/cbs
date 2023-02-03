using CBSPay.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class PaymentHistoryItem : BaseEntity<long>
    {
        public virtual long ItemId { get; set; }
        public virtual string ItemDescription { get; set; }
        public virtual decimal? ItemAmount { get; set; }
        public virtual decimal? AmountPaid { get; set; }
        [JsonIgnore]
        public virtual PaymentHistory PaymentHistory { get; set; }
    }
}
