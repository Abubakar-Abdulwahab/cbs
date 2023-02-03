using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class SettlementRuleVM
    {
        /// <summary>
        /// Settlement Id in the database table
        /// </summary>
        public long Id { get; set; }

        public bool IsEdit { get; set; }

        [Required(ErrorMessage = "Settlement engine rule identifier field is required")]
        [StringLength(100, ErrorMessage = "Name value must be between 4 to 100 characters.", MinimumLength = 4)]
        public string SettlementEngineRuleIdentifier { get; set; }

        [Required(ErrorMessage = "A Name is required")]
        [StringLength(250, ErrorMessage = "Name value must be between 4 to 250 characters.", MinimumLength = 4)]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Please select any or all MDAs")]
        public string SMDAId { get; set; }

        [Required(ErrorMessage = "Please select any or all MDAs")]
        public List<string> SMDAIds { get; set; }

        public int MDAId { get; set; }

        public string SRevenueHeadId { get; set; }

        public int RevenueHeadId { get; set; }

        //[Required(ErrorMessage = "Please select any or all payment providers")]
        public string SPaymentProviderId { get; set; }

        //[Required(ErrorMessage = "Please select any or all payment channels")]
        public string SPaymentChannelId { get; set; }

        public int PaymentChannelId { get; set; }

        public int PaymentProviderId { get; set; }

        public int FrequencyValue
        {
            get { return (int)this.SettlementFrequencyModel.FrequencyType; }
        }

        public SettlementFrequencyModel SettlementFrequencyModel { get; set; }

        public List<PaymentProviderVM> PaymentProviders { get; set; }

        [Required(ErrorMessage = "Please select any or all payment providers")]
        public ICollection<string> SelectedPaymentProviders { get; set; }

        public List<PaymentChannelVM> PaymentChannels { get; set; }

        [Required(ErrorMessage = "Please select any or all payment channels")]
        public ICollection<int> SelectedPaymentChannels { get; set; }

        public List<RevenueHeadVM> RevenueHeads { get; set; }

        public List<MDAVM> MDAs { get; set; }

        /// <summary>
        /// list of selected revenue heads
        /// </summary>
        public ICollection<int> RevenueHeadsSelected { get; set; }

        public bool ApplyToAllRevenueHeads { get; set; }

        public int RevenueHeadIdWithError { get; set; }

        public List<int> SelectedMdas { get; set; }

        public string SelectedRhAndMdas { get; set; }
    }
}