using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxProfileFormVM : InvoiceGenerationDetailsModel
    {
        public TaxEntityViewModel TaxEntityViewModel { get; set; }

        public ICollection<CollectionFormVM> CollectionFormVM { get; set; }

        public ExternalRedirect ExternalRedirect { get; set; }

        /// <summary>
        /// holds the state and LGAs
        /// </summary>
        
        public List<StateModel> AllStates { get; set; }

        public List<LGA> AllLgas { get; set; }

        public int TenantState { get; set; }
    }


    public class InvoiceProceedVM : InvoiceGenerationDetailsModel
    {
        public TaxEntityViewModel TaxEntityViewModel { get; set; }

        public ICollection<CollectionFormVM> CollectionFormVM { get; set; }

        public bool HasMessage { get; set; }

        public string Message { get; set; }
    }


    public class PAYESelectOptionVM
    {
        public HeaderObj HeaderObj { get; set; }

        public PAYEDirection SelectOption { get; set; }
    }  


    public class ConfirmInvoiceVM : InvoiceGenerationDetailsModel
    {
        public TaxEntityViewModel TaxEntityViewModel { get; set; }

        public ICollection<CollectionFormVM> CollectionFormVM { get; set; }

        public string ExternalRef { get; set; }

        public bool HasMessage { get; set; }

        public string Message { get; set; }

        public bool CanEnterAmount { get; set; }

        public bool IsVisibleSurcharge { get; set; }

        public string Token { get; set; }

        public string SAmount { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public int SelectedState { get; set; }

        public IEnumerable<FormControlViewModel> Forms { get; set; }

        public int SelectedStateLGA { get; set; }

        public int Year { get; set; }
    }


    public class InvoiceConfirmedModel
    {
        public string Token { get; set; }

        public decimal Amount { get; set; }

        public string ExternalRef { get; set; }

        public decimal RevenueHeadSurcharge { get; set; }

        public bool IsNotStaging { get; set; }
    }
}