using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class UserCollectionFormView
    {
        public string Category { get; set; }
        public int CategoryIdentifier { get; set; }

        public string RevenueHeadName { get; set; }
        public string MDAName { get; set; }

        public string RevenueHeadCode { get; set; }
        public int RevenueHeadId { get; set; }


        public string PreviewUrl { get; set; }

        public string PdfUrl { get; set; }


        public TaxEntity Customer { get; set; }

        public Invoice SavedInvoice { get; set; }

        private int quantity = 1;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public TaxEntityViewModel TaxEntityViewModel { get; set; }

        public ICollection<CollectionFormVM> CollectionFormVM { get; set; }

        public DirectAssessmentCollection DirectAssessmentCollectionVM { get; set; }

        
        public DisplayPage PageToDisplay { get; set; }


        public bool IsAuthorized { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public Dictionary<string, string> StateLGAs { get; set; }

        public Int64 BatchId { get; set; }

        public string Token { get; set; }

        public string AdapterValue { get; set; }
    }



    public enum DisplayPage
    {
        ShowInputPage,
        ShowPayeeAssessmentPage,
    }
}