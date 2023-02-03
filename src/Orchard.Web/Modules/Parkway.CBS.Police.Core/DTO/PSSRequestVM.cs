using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSSRequestVM
    {
        public Int64 Id { get; set; }

        public long TaxEntityId { get; set; }

        public string ServiceName { get; set; }

        public string FileRefNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public DateTime RequestDate { get; set; }

        public string CustomerName { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime LastActionDate { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }

        public decimal InvoiceAmount { get; set; }

        public int ServiceTypeId { get; set; }

        public int ServiceId { get; set; }

        public string RequestDateString { get { return RequestDate.ToString("dd/MM/yyyy"); } }

        public string LastActionDateString { get { return LastActionDate.ToString("dd/MM/yyyy"); } }

        public string ApprovedBy { get; set; }

        public string Comment { get; set; }

        public PSSRequestStatus Status
        { get { return (PSSRequestStatus)StatusId; } }


        public int StatusId { get; set; }

        public string CommandName { get; set; }

        public string State { get; set; }

        public string LGA { get; set; }

        public string BranchName { get; set; }

        public int FlowDefinitionLevelId { get; set; }

        public string RequestPhaseName { get; set; }
    }
}