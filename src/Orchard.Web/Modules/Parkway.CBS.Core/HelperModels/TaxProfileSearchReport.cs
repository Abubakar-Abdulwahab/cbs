namespace Parkway.CBS.Core.HelperModels
{
    public class TaxProfileSearchReport
    {
        public object ReportRecords { get; set; }
        public TaxProfilesSearchParams SearchFilter { get; set; }
        public int TotalNumberOfTaxPayers { get; set; }
    }
}