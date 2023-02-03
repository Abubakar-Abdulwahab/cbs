using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models.Enums;


namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificateRequestFiles : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxClearanceCertificateRequest TaxClearanceCertificateRequest { get; set; }

        /// <summary>
        /// <see cref="TCCFileUploadType"/>
        /// </summary>
        public virtual int TCCFileUploadTypeId { get; set; }

        public virtual string OriginalFileName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ContentType { get; set; }

    }
}