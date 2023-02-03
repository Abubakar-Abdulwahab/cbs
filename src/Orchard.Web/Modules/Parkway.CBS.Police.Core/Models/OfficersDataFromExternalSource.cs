using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class OfficersDataFromExternalSource : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string IPPISNumber { get; set; }

        public virtual string ServiceNunber { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string GenderCode { get; set; }

        public virtual string Gender { get; set; }

        public virtual string RankName { get; set; }

        public virtual string RankCode { get; set; }

        public virtual string StateName { get; set; }

        public virtual string StateCode { get; set; }

        public virtual string Command { get; set; }

        public virtual string CommandCode { get; set; }

        public virtual string LGAName { get; set; }

        public virtual string LGACode { get; set; }

        public virtual string DateOfBirth { get; set; }

        public virtual string StateOfOrigin { get; set; }


        public virtual string AccountNumber { get; set; }

        public virtual string BankCode { get; set; }

        public virtual string RequestIdentifier { get; set; }

        public virtual int RequestItemSN { get; set; }
    }
}