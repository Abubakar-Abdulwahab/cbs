using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{

    public class PSSSettlementDeduction : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string DeductionName { get; set; }

        public virtual string DeductionDescription { get; set; }

        public virtual string DeductionImplementation { get; set; }

        public virtual string BankAccountNumber { get; set; }

        public virtual string BankCode { get; set; }

        /// <summary>
        /// Whether it is flat or percentage
        /// </summary>
        public virtual int DeductionType { get; set; }

        public virtual decimal DeductionValue { get; set; }

        public virtual bool IsDeleted { get; set; }
    }

}