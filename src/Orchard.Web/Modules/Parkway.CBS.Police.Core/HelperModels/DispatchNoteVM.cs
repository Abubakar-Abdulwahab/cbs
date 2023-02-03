using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class DispatchNoteVM
    {
        public IEnumerable<CommandVM> ServicingCommands { get; set; }

        public string ApplicantName { get; set; }

        public string FileNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public string ServiceOriginLocation { get; set; }

        public string ServiceDeliveryLocation { get; set; }

        public string ServiceDuration { get; set; }

        public string LogoURL { get; set; }

        public string PossapLogoUrl { get; set; }

        public string StripURL { get; set; }

        public string BGPath { get; set; }

        public string ValidateDocumentUrl { get; set; }

        public IEnumerable<PoliceOfficerLogVM> PoliceOfficers { get; set; }

        public string Template { get; set; }
    }
}