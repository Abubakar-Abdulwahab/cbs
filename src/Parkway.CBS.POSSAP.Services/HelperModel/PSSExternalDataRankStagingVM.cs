namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSExternalDataRankStagingVM
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ExternalDataRankId { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
