using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReportStatsVM
    {
        public decimal TotalAmount { get; set; }

        public Int64 DistinctRecordCount { get; set; }

        public Int64 TotalRecordCount { get; set; }
    }
}