﻿using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSBranchOfficersUploadBatchStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }
        /// <summary>
        /// Profile of the branch which the officers file is uploaded for
        /// </summary>
        public virtual TaxEntityProfileLocation TaxEntityProfileLocation { get; set; }

        public virtual string BatchRef { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSBranchOfficersUploadStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }

        /// <summary>
        /// Admin user uploading the file
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        public virtual string FilePath { get; set; }

        public virtual bool HasGeneratedInvoice { get; set; }
    }
}