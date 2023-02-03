using System;

namespace Parkway.CBS.Entities.VMs
{
    public class FileServiceHelper
    {
        public string TenantName { get; set; }

        public int StateId { get; set; }

        public Int64 UnknownTaxPayerCodeId { get; set; }

        public int UnknownTaxProfileCategoryId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string FilePath { get; set; }

        public string SummaryPath { get; set; }

        public string ProcessingPath { get; set; }

        public string ProcessedPath { get; set; }

        public string ProcessedSummaryCSVFilePath { get; set; }

        public string SummaryFilePathWithFileName { get; set; }
    }
}
