using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceOfficer : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual PoliceRanking Rank { get; set; }

        public virtual string IdentificationNumber { get; set; }

        public virtual string IPPISNumber { get; set; }

        public virtual Command Command { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ICollection<PolicerOfficerLog> PoliceOfficerLogs { get; set; }

        /// <summary>
        /// <see cref="Enums.Gender"/>
        /// </summary>
        public virtual int Gender_Id { get; set; }

        public virtual string AccountNumber { get; set; }

        public virtual string BankCode { get; set; }

    }
}