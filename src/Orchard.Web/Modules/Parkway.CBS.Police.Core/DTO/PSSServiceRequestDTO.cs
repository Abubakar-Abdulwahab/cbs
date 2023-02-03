using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSServiceRequestDTO
    {
        public string ServiceName { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal InvoiceAmount { get; set; }

        public int Status { get; set; }

        public IEnumerable<UserFormDetails> FormDetails { get; set; }

        public decimal InvoiceAmountDue { get; set; }

        public int FlowDefinitionLevelId { get; set; }


        public string GetStatus()
        {
            return ((PSSRequestStatus)this.Status).ToDescription();
        }
    }
}