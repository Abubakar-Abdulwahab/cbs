using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestDetailsVM : PSSRequestDetailsVM
    {
        public HeaderObj HeaderObj { get; set; }

        public IEnumerable<PSSRequestStatusLogVM> RequestStatusLog { get; set; }

        public bool HasCertificate { get; set; }

        public string CertificateLabel { get; set; }
    }
}