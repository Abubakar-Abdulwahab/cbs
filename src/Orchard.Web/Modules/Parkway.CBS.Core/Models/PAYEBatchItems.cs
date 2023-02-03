using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBatchItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual decimal GrossAnnual { get; set; }

        public virtual decimal Exemptions { get; set; }

        public virtual decimal IncomeTaxPerMonth { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual PAYEBatchRecord PAYEBatchRecord { get; set; }

        public virtual int Month { get; set; }

        public virtual int Year { get; set; }

        /// <summary>
        /// this date is used for query purposes
        /// </summary>
        public virtual DateTime AssessmentDate { get; set; }
    }
}