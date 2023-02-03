using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.NAGISImplementation.Models
{
    public class NAGISDataLineRecordModel
    {
        public NAGISDataStringValue CustomerName { get; internal set; }

        public NAGISDataStringValue PhoneNumber { get; internal set; }

        public NAGISDataStringValue Address { get; internal set; }

        public NAGISDataStringValue CustomerId { get; internal set; }

        public NAGISDecimalValue Amount { get; internal set; }

        public NAGISDataStringValue Tin { get; internal set; }

        public NAGISDataStringValue InvoiceNumber { get; internal set; }

        public NAGISDataStringValue CreationDate { get; internal set; }

        public NAGISDataIntValue RevenueHeadId { get; internal set; }

        public NAGISDataStringValue ExternalRefId { get; internal set; }

        public NAGISDataStringValue InvoiceDescription { get; internal set; }

        public NAGISDecimalValue AmountDue { get; internal set; }

        public NAGISDataIntValue Quantity { get; internal set; }

        public NAGISDataIntValue Status { get; internal set; }

        public NAGISDataIntValue GroupID { get; internal set; }

        public NAGISDataIntValue TaxEntityCategoryID { get; internal set; }

    }
}
