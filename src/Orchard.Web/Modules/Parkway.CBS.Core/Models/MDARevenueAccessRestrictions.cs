using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.Models
{
    public class MDARevenueAccessRestrictions : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// OperationType Enum
        /// </summary>
        public virtual int OperationType { get; set; }

        public virtual long OperationTypeIdentifierId { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}