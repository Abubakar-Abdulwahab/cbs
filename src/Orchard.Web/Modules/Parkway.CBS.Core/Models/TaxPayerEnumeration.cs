using Orchard.Users.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class TaxPayerEnumeration : CBSBaseModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Tax Entity that uploaded the Enumeration Schedule
        /// </summary>
        public virtual TaxEntity Employer { get; set; }

        /// <summary>
        /// Enumeration Schedule Ref
        /// </summary>
        public virtual string BatchRef { get; set; }

        /// <summary>
        /// EnumerationScheduleUploadType Enum
        /// </summary>
        public virtual int UploadType { get; set; }

        /// <summary>
        /// EnumerationScheduleUploadType Enum Description
        /// </summary>
        public virtual string UploadTypeCode { get; set; }

        /// <summary>
        /// true if uploaded by backend admin user
        /// </summary>
        public virtual bool UploadedByAdmin { get; set; }

        /// <summary>
        /// Backend Admin user that uploaded the schedule
        /// </summary>
        public virtual UserPartRecord Admin { get; set; }

        /// <summary>
        /// true if uploaded by user from frontend
        /// </summary>
        public virtual bool UploadedByUser { get; set; }

        /// <summary>
        /// Frontend user that uploaded the schedule
        /// </summary>
        public virtual CBSUser User { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// TaxPayerEnumerationProcessingStages Enum
        /// </summary>
        public virtual int ProcessingStage { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FilePath { get; set; }

        /// <summary>
        /// Line items for the Enumeration Schedule
        /// </summary>
        public virtual ICollection<TaxPayerEnumerationItems> EnumerationItems { get; set; }
    }
}