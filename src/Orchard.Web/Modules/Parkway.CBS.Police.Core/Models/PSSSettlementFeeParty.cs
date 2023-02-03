using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this table includes the parties concerned for a particular settlement.
    /// </summary>
    public class PSSSettlementFeeParty : CBSModel
    {
        public virtual PSSSettlement Settlement { get; set; }

        public virtual PSSFeeParty FeeParty { get; set; }

        /// <summary>
        /// indicates whether additional splits are required
        /// </summary>
        public virtual bool HasAdditionalSplits { get; set; }

        /// <summary>
        /// here we indicate whether this fee party has further splits
        /// this field will indicate what branch of execution should follow
        /// </summary>
        public virtual string AdditionalSplitValue { get; set; }

        /// <summary>
        /// this would hold the eventual deduction type that this split will have
        /// <see cref="Enums.DeductionShareType"/>
        /// </summary>
        public virtual int DeductionTypeId { get; set; }

        /// <summary>
        /// this would hold the eventual deduction value that this split would have
        /// this is determined by first the service deduction, then by the command deduction if any
        /// </summary>
        public virtual decimal DeductionValue { get; set; }
        
        public virtual int Position { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// config with the Maximum/highest percentage
        /// only one per fee party settlement
        /// </summary>
        public virtual bool MaxPercentage { get; set; }

        public virtual bool IsDeleted { get; set; }

    }
}