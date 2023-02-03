namespace Parkway.CBS.Payee.PayeeAdapters.ETCC
{
    public class PAYEExemptionTypeAssessmentLine
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Amount { get; set; }

        public PayeeDecimalValue PayeeExemptionAmount { get; internal set; }

    }
}