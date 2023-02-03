using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// when the job is been run, a row for this table is created
    /// this row would how a bunch of pre-flight titems
    /// these pre flight items are settlement items that are going to run.
    /// 
    /// </summary>
    public class PSSSettlementPreFlightBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ICollection<PSSSettlementPreFlightItems> Items { get; set; }
    }
}