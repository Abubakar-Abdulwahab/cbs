using System.Collections.Generic;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceItemFormsAndPosition
    {
        public InvoiceItems InvoiceItems { get; set; }

        public IEnumerable<FormControlViewModel> AssociatedForms { get; internal set; }

        internal int Position { get; set; }
    }
}