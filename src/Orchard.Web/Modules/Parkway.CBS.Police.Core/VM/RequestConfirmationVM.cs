using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestConfirmationVM
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string ServiceRequested { get; set; }

        public string NameOfPoliceCommand { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public FlashObj FlashObj { get; set; }

        public bool HasMessage { get; set; }

        public string Reason { get; set; }

        /// <summary>
        /// if the type has additional value for display, use this field to 
        /// hold the partial
        /// </summary>
        public string PartialName { get; set; }

        public string CustomViewName { get; set; }

        public List<AmountDetails> AmountDetails { get; set; }

        public dynamic RequestSpecificModel { get; set; }

        public List<UserFormDetails> FormValues { get; set; }
    }
}