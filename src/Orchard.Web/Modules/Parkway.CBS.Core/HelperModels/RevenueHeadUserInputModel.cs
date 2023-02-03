using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadUserInputModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid revenue head ID")]
        public int RevenueHeadId { get; set; }

        [StringLength(100, ErrorMessage = "The Additional Description value can only be 100 characters long")]
        public string AdditionalDescription { get; set; }


        public decimal Amount { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid number")]
        public int Quantity { get; set; }


        public List<FormControlViewModel> FormValues { get; set; }


        private bool _amountCanVary = true;

        [JsonIgnore]
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
        [JsonIgnore]
        public int Index { get; internal set; }

        [JsonIgnore]
        public bool ApplySurcharge { get; set; }

        [JsonIgnore]
        public decimal Surcharge { get; set; }
    }
}