using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class NAGISDataBatchCollectionDetails
    {
        public Int64 Id { get; set; }

        public string BatchRef { get; set; }

        public Int64 NumberOfRecordSentToCashFlow { get; set; }

        public string ProccessStage { get; set; }

        public DateTime CreatedDate { get; set; }

        public NagisDataProcessingStages Status { get; set; }

    }
}