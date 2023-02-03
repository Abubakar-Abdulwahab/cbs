using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestDumpVM
    {
        public HeaderObj HeaderObj { get; set; }

        public bool HasMessage { get; set; }

        public string Reason { get; set; }

        public FlashObj FlashObj { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public int SelectedState { get; set; }

        public int SelectedStateLGA { get; set; }

        /// <summary>
        /// holds the selected command for this request
        /// </summary>
        public int SelectedCommand { get; set; }

        public string LGAName { get; set; }

        public string StateName { get; set; }

        public string CommandName { get; set; }

        public string CommandAddress { get; set; }

        public string CommandStateName { get; set; }

        public string CommandLgaName { get; set; }

        public string ExpectedHash { get; internal set; }

        public string SiteName { get; set; }

        public string InvoiceDescription { get; set; }

        public bool DontValidateFormControls { get; set; }

        public string ServiceName { get; set; }

        public string ServiceNote { get; set; }

        public string AlternativeContactName { get; set; }

        public string AlternativeContactPhoneNumber { get; set; }

        public string AlternativeContactEmail { get; set; }

        public bool HasDifferentialWorkFlow { get; set; }

    }
}