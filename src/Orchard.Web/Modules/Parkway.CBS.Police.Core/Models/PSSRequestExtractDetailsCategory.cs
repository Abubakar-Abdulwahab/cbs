using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestExtractDetailsCategory : CBSBaseModel
    {
        public virtual Int64 Id { get; set; } 

        public virtual PSSRequest Request { get; set; }

        public virtual ExtractDetails ExtractDetails { get; set; }

        public virtual ExtractCategory ExtractCategory { get; set; }

        public virtual ExtractSubCategory ExtractSubCategory { get; set; }

        public virtual string RequestReason { get; set; }
    }
}