namespace Parkway.CBS.Core.HelperModels
{
    public class TokenExpiryResponse
    {
        public long CBSUserId { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public CBSUserVM CBSUser { get; set; }

        public RedirectReturnObject RedirectObj { get; set; }

    }
}