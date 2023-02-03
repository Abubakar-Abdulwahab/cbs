namespace Parkway.CBS.Core.HelperModels
{
    public class CBSUserVM
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public bool Verified { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsAdministrator { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }
    }
}