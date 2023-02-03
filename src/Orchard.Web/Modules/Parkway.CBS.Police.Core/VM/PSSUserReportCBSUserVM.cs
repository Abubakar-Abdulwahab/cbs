using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSUserReportCBSUserVM
    {
        public string Email { get; set; }

        public bool IsVerified { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string IdentificationNumber { get; set; }

        public bool IsAdministrator { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PayerId { get; set; }
    }
}