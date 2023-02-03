using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficerDeploymentLogVM
    {
        public Int64 Id { get; set; }

        public string Address { get; set; }

        public string CustomerName { get; set; }

        public string ServiceTypeName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PoliceOfficerName { get; set; }

        public string CommandName { get; set; }

        public string OfficerRankName { get; set; }

        public Int64 SelectedOfficerRank { get; set; }

        public int SelectedState { get; set; }

        public int SelectedLGA { get; set; }

        public Int64 InvoiceId { get; set; }

        public Int64 RequestId { get; set; }

        public string BatchId { get; set; }

        public bool IsActive { get; set; }

        public int Status { get; set; }

        public PoliceOfficerDeploymentLog PoliceOfficerDeploymentLog { get; set; }
    }
}