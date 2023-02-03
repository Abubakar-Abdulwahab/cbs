using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemVM
    {
        public  decimal GrossAnnual { get; set; }

        public  decimal Exemptions { get; set; }

        public  decimal IncomeTaxPerMonth { get; set; }

        public  int Month { get; set; }

        public  int Year { get; set; }

        /// <summary>
        /// this date is used for query purposes
        /// </summary>
        public  DateTime AssessmentDate { get; set; }
    }
}