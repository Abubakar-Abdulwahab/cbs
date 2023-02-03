using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class GenerateInvoiceRequestModel
    {
        public string AdditionalDescription { get; set; }


        public decimal Amount { get; set; }


        public int Quantity { get; set; }

        /// <summary>
        /// holds the expected form control details of this revenue head
        /// </summary>
        public List<FormControlViewModel> FormValues { get; set; }


        private bool _amountCanVary = true;

        /// <summary>
        /// This flag is used to indicate that the amount can vary from the actual amount
        /// the revenue head is expecting
        /// </summary>
        public bool AmountCanVary
        {
            get { return _amountCanVary; }
            set { _amountCanVary = value; }
        }

        /// <summary>
        /// this property is used to set the index of the revenue head if in a list
        /// <para>It serves as a more unique identifier instead of the revenue head Id as the same revenue head Id can appear multiple times in a list</para>
        /// </summary>
        public int Index { get; internal set; }

        public RevenueHeadVM RevenueHeadVM { get; set; }


        public BillingModelVM BillingModelVM { get; set; }


        public MDAVM MDAVM { get; set; }

        /// <summary>
        /// contains the list of revenue heads that this group parent holds
        /// </summary>
        public List<RevenueHeadGroupVM> RevenueHeadGroupVM { get; set; }
        public string CashflowUniqueIdentifier { get; internal set; }
    }
}