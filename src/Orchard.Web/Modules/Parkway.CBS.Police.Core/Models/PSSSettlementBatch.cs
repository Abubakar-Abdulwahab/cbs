using System;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlement PSSSettlement { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual DateTime ScheduleDate { get; set; }

        public virtual DateTime SettlementRangeStartDate { get; set; }

        public virtual DateTime SettlementRangeEndDate { get; set; }

        public virtual Decimal SettlementAmount { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSSettlementBatchStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual string StatusMessage { get; set; }

        public virtual ICollection<PSSSettlementBatchItems> PSSSettlementBatchItems { get; set; }

        /// <summary>
        /// Settlement deduction items
        /// </summary>
        public virtual ICollection<PSSSettlementDeduction> Deductions { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual bool HasCommandSplits { get; set; }

        public virtual DateTime? SettlementDate { get; set; }

    }
}