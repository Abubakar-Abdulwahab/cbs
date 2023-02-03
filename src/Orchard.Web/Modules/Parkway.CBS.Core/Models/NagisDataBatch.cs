using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class NagisDataBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual UserPartRecord AdminUser { get; set; }

        /// <summary>
        /// <see cref="NagisDataProcessingStages"/>
        /// </summary>
        public virtual int ProccessStage { get; set; }

        public virtual int NumberOfRecords { get; set; }

        public virtual string BatchRef { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string FileName { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual int PercentageProgress { get; set; }

        public virtual string AdapterClassName { get; set; }

        public virtual Int64 NumberOfRecordSentToCashFlow { get; set; }

        public virtual GeneralBatchReference GeneralBatchReference { get; set; }

        public virtual StateModel StateModel { get; set; }


        public NagisDataProcessingStages GetProcessStage()
        {
            return (NagisDataProcessingStages)this.ProccessStage;
        }
    }
}