namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPayerEnumerationLine
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string TIN { get; set; }

        public string LGA { get; set; }

        public string Address { get; set; }

        public bool HasError { get; set; }

        public virtual string ErrorMessages { get; set; }
    }


    public class TaxPayerEnumerationLineComparer : System.Collections.Generic.EqualityComparer<TaxPayerEnumerationLine>
    {
        public override bool Equals(TaxPayerEnumerationLine x, TaxPayerEnumerationLine y)
        {
            if (x == null && y == null) { return true; }
            else if(x == null || y == null) { return false;  }

            return ((x.Name == y.Name) || (x.PhoneNumber == y.PhoneNumber) || (x.Email == y.Email));
        }

        public override int GetHashCode(TaxPayerEnumerationLine obj)
        {
            return base.GetHashCode();
        }
    } 
}