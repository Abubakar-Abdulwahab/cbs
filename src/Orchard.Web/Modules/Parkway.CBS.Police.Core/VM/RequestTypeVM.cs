using System;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public abstract class RequestTypeVM
    {
        public string Reason { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }


        public int ServiceTypeId { get; set; }

        public Int64 RequestId { get; set; }

        public string StateName { get; set; }

        public string LGAName { get; set; }

        public string CommandName { get; set; }

        public string CommandAddress { get; set; }

        public int FlowDefinitionId { get; set; }

        public int FlowDefinitionLevelId { get; set; }

        public string ApproverComment { get; set; }
    }
}