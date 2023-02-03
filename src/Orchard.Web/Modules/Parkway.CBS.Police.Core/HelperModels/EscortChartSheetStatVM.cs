using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class EscortChartSheetStatVM
    {
        public int NumberofSuccessfulRecords { get; set; }

        public int NumberofUnSuccessfulRecords { get; set; }

        public List<EscortChartSheetSeedingVM> UnSuccessfulRecords { get; set; }
    }

    public class EscortChartSheetSeedingVM
    {
        public int RankId { get; set; }

        public int PSSEscortServiceCategoryId { get; set; }

        public decimal RateAmount { get; set; }

        public int StateId { get; set; }

        public int LGAId { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class EscortChartSheetResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of rates
        /// </summary>
        public ConcurrentStack<EscortChartSheetSeedingVM> ChartSheetRecords { get; set; }
    }


}