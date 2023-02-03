using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.ViewModels
{
    public class RequestHistoryDataModel
    {
        public string ApplicantName { get; set; }

        public string ApplicationNumber { get; set; }

        public string TIN { get; set; }

        public int Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public long OperatorId { get; set; }

        public int ChunkSize { get; set; }

    }
}