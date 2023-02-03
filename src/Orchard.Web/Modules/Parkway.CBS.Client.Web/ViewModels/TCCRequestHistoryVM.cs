using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Client.Web.ViewModels
{
    public class TCCRequestHistoryVM
    {
        public long TaxEntityId { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string Message { get; set; }

        public string ApplicationNumber { get; set; }

        public TCCRequestStatus Status { get; set; }

        public string DateFilter { get; set; }

        public Int64 DataSize { get; set; }

        public List<TCCRequestVM> Requests { get; set; }

        public int TotalRequestRecord { get; set; }

        public string TIN { get; set; }

        public string ApplicantName { get; set; }

        public string Token { get; set; }
    }
}