using System.Linq;
using System.Collections.Generic;


namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadCombinedHelper
    {

        /// <summary>
        /// this prop hold the user input for invoice generation
        /// </summary>
        public RevenueHeadUserInputModel RequestModel { get; set; }

        /// <summary>
        /// This is the model from the database, this model has the revenue head details needed to generate an invoice
        /// </summary>
        public RevenueHeadForInvoiceGenerationHelper RevenueHeadDBModel { get { return RevenueHeadEssentialsFromDB.FirstOrDefault(); } }

        /// <summary>
        /// this is a helper property, it is use to hold the future query of the database process to get the revenue head details in a loop
        /// <para></para>
        /// </summary>
        public IEnumerable<RevenueHeadForInvoiceGenerationHelper> RevenueHeadEssentialsFromDB { private get; set; }

    }
}