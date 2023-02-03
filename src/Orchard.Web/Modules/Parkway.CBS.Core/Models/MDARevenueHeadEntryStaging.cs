using System;

namespace Parkway.CBS.Core.Models
{
    public class MDARevenueHeadEntryStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ReferenceNumber { get; set; }

        /// <summary>
        /// Hashcode of the class providing this implementation
        /// </summary>
        public virtual int OperationType { get; set; }

        public virtual int OperationTypeIdentifierId { get; set; }
    }
}