using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class PSSRequestTaxEntityViewVM
    {
        public long PSSRequestId { get; set; }

        public int ServiceId { get; set; }

        public string FileRefNumber { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public CBSUserVM CBSUser { get; set; }
    }
}
