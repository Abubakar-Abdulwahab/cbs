using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    /// <summary>
    /// Tax Payer Report for Tax Payer Report
    /// </summary>
    public class TaxEntityReportDetail
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Category { get; set; }

        public string TaxPayerIdentificationNumber { get; set; }

        public string RegNumber { get; set; }

        public string Email { get; set; }

        public string PayerId { get; set; }

        public string StateName { get; set; }

        public string LGA { get; set; }
    }
}