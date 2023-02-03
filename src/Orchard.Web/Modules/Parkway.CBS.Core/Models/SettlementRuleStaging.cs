using Newtonsoft.Json;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class SettlementRuleStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string SettlementEngineRuleIdentifier { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual string CronExpression { get; set; }

        public virtual DateTime SettlementPeriodStartDate { get; set; }

        public virtual DateTime SettlementPeriodEndDate { get; set; }

        public virtual string JSONScheduleModel { get; set; }

        public virtual bool HasDoneValidation { get; set; }

        public virtual bool IsEdit { get; set; }

        public virtual SettlementRule SettlementRuleToBeEdited { get; set; }

        public virtual ICollection<SettlementRuleDetailsStaging> SettlementRuleDetails { get; set; }

        public SettlementFrequencyModel GetSchedule()
        {
            return JsonConvert.DeserializeObject<SettlementFrequencyModel>(this.JSONScheduleModel);
        }
        
    }
}