using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class ReferenceDataBatchCollectionDetails
    {
        public Int64 Id { get; set; }

        public string BatchRef { get; set; }

        public int PercentageProgress { get; set; }

        public int NumberOfRecords { get; set; }

        public string ProccessStage { get; set; }

        public string LGAName { get; set; }

        public DateTime CreatedDate { get; set; }

        public ReferenceDataProcessingStages Status { get; set; }
    }
}