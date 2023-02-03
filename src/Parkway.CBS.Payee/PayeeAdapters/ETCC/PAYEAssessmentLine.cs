namespace Parkway.CBS.Payee.PayeeAdapters.ETCC
{
    public class PAYEAssessmentLine
    {
        public string PayerId { get; set; }

        public string GrossAnnualEarning { get; set; }

        public string Exemptions { get; set; }

        public System.Collections.Generic.List<PAYEExemptionTypeAssessmentLine> PAYEExemptionTypes { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }
    }
}
