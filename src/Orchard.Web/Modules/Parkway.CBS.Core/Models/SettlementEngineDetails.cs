using System;

namespace Parkway.CBS.Core.Models
{
    public class SettlementEngineDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string JSONModel { get; set; }

        public virtual DateTime TimeFired { get; set; }

        public virtual string Params { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual int TransactionCount { get; set; }

        public virtual int SettlementId { get; set; }

        public virtual bool Error { get; set; }

        public virtual string SettlementEngineResponseJSON { get; set; }

        public virtual string LongerParams { get; set; }
    }
}