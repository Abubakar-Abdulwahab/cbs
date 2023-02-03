using CBSPay.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Entities
{
    public class AssessmentRule :BaseEntity<long>
    {
        public virtual long AARID { get; set; }
        public virtual long AssetTypeId { get; set; }
        public virtual string AssetTypeName { get; set; }
        public virtual long AssetID { get; set; }
        public virtual string AssetRIN { get; set; }
        public virtual long ProfileID { get; set; }
        public virtual string ProfileDescription { get; set; }
        public virtual long AssessmentRuleID { get; set; }
        public virtual string AssessmentRuleName { get; set; }
        public virtual int TaxYear { get; set; }
        public virtual decimal? AssessmentRuleAmount { get; set; }
        public virtual decimal? SettledAmount { get; set; }
        [JsonIgnore]
        public virtual AssessmentDetailsResult AssessmentDetails { get; set; }

        protected internal virtual void CopyFrom(AssessmentRule item)
        {
            this.AARID = item.AARID;
            this.SettledAmount = item.SettledAmount;
            this.AssessmentRuleAmount = item.AssessmentRuleAmount;
            this.TaxYear = item.TaxYear;
            this.DateModified = DateTime.Now;
            this.ProfileID = item.ProfileID;
            this.ProfileDescription = item.ProfileDescription;
            this.AssetRIN = item.AssetRIN;
            this.AssetID = item.AssetID;
            this.AssessmentRuleID = item.AssessmentRuleID;
            this.AssessmentRuleName = item.AssessmentRuleName;
            this.AssetTypeId = item.AssetTypeId;
            this.AssetTypeName = item.AssetTypeName;
        }

    }

    
}


