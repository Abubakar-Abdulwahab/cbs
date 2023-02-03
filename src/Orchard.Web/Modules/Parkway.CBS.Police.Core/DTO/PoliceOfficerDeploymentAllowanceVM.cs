using Orchard.Users.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PoliceOfficerDeploymentAllowanceVM
    {
        public string FileRefNumber { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

        public Int64 Id { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime LastActionDate { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// police officer Id
        /// </summary>
        public Int64 PolicerOfficerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PoliceOfficerName { get; set; }

        public string PoliceOfficerRank { get; set; }

        public string PoliceOfficerIPPIS { get; set; }

        public string PoliceOfficerAPNumber { get; set; }

        public object InitiatedBy { get; set; }

        public int StatusId { get; set; }

        public Int64 RequestId { get; set; }

        public string InvoiceNumber { get; set; }

        public string Comment { get; set; }

        public int ServiceTypeId { get; set; }

        public string Narration { get; set; }

        public DeploymentAllowanceStatus Status
        { get { return (DeploymentAllowanceStatus)StatusId; } }

        public string BankName
        {
            get { return Util.GetBankName(BankCode); }
        }

        public string InitiatedByName
        {
            get { return InitiatedBy == null ? "" : ((UserPartRecord)InitiatedBy).UserName; }
        }
    }
}