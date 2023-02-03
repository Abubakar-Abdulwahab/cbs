using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class IPPISBatch : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// <see cref="IPPISProcessingStages"/>
        /// </summary>
        public virtual int ProccessStage { get; set; }

        public virtual int NumberOfRecords { get; set; }

        public virtual string FilePath { get; set; }

        public virtual bool ErrorOccurred { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        public virtual bool HasSummaryFileMoved { get; set; }

        public virtual bool IsSummaryFileReady { get; set; }

        public virtual bool ErrorProcessingSummaryFile { get; set; }

        public virtual string ErrorMessageProcessingSummaryFile { get; set; }

        public IPPISProcessingStages GetProcessStage()
        {
            return (IPPISProcessingStages)this.ProccessStage;
        }
    }
}