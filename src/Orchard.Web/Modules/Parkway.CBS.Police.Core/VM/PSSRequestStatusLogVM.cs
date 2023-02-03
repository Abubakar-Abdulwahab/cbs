using Parkway.CBS.Police.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSRequestStatusLogVM
    {
        public Int64 Id { get; set; }

        public PSSRequestVM Request { get; set; }

        public string StatusDescription { get; set; }

        public RequestInvoiceVM Invoice { get; set; }

        public int Status { get; set; }

        //public PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public bool UserActionRequired { get; set; }

        public bool Fulfilled { get; set; }

        public string PositionName { get; set; }

        public int Position { get; set; }

        public int DefinitionId { get; set; }

        public int WorkFlowActionValue { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedAtParsed { get { return this.UpdatedAt.Value.ToString(); } }
    }
}