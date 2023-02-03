using System;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Models
{
    public class PolicerOfficerLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual PoliceRanking Rank { get; set; }

        public virtual string IdentificationNumber { get; set; }

        public virtual string IPPISNumber { get; set; }

        public virtual Command Command { get; set; }

        public virtual string Gender { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string BankCode { get; set; }
    }
}