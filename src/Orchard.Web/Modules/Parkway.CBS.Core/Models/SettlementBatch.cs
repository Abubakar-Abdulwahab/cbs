using System;

namespace Parkway.CBS.Core.Models
{
    public class SettlementBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual int ProcessStage { get; set; }
    }
}