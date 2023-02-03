using Newtonsoft.Json;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class SettlementRule : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string SettlementEngineRuleIdentifier { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual UserPartRecord ConfirmedBy { get; set; }

        public virtual string CronExpression { get; set; }

        /// <summary>
        /// this is the settlement period start date
        /// <para>Indicates when the settlement will start</para>
        /// </summary>
        public virtual DateTime SettlementPeriodStartDate { get; set; }

        /// <summary>
        /// indicate when the settlement will end
        /// </summary>
        public virtual DateTime SettlementPeriodEndDate { get; set; }

        public virtual DateTime NextScheduleDate { get; set; }

        public virtual int NumberOfRuns { get; set; }


        public virtual string JSONScheduleModel { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// +1, +2, +3
        /// </summary>
        public virtual int SettlementCycle { get; set; }


        public virtual ICollection<SettlementRuleDetails> SettlementRuleDetails { get; set; }


        public SettlementFrequencyModel GetSchedule()
        {
            return JsonConvert.DeserializeObject<SettlementFrequencyModel>(this.JSONScheduleModel);
        }       
    }
}