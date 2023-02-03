using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class GenerateRequestWithoutOfficersUploadBatchItemsStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual GenerateRequestWithoutOfficersUploadBatchStaging GenerateRequestWithoutOfficersUploadBatchStaging { get; set; }

        public virtual string BranchCode { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual string NumberOfOfficersValue { get; set; }

        public virtual string CommandCode { get; set; }

        public virtual string CommandTypeValue { get; set; }
        
        public virtual int CommandType { get; set; }

        /// <summary>
        /// Resolved command attached to the command code
        /// </summary>
        public virtual Command Command { get; set; }

        /// <summary>
        /// <see cref="Enums.DayType"/>
        /// </summary>
        public virtual int DayType { get; set; }
        
        public virtual string DayTypeValue { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }
    }
}