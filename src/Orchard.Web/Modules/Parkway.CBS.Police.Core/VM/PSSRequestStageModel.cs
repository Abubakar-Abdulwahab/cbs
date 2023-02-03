using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSRequestStageModel
    {
        public int CategoryId { get; set; }

        public int ServiceId { get; set; }

        public PSSUserRequestGenerationStage Stage { get; set; }

        public int ServiceType { get; set; }

        public string ServicePrefix { get; set; }

        public string ServiceName { get; set; }

        public string ServiceNote { get; set; }

        public FlashObj FlashObj { get; set; }

        public string Token { get; set; }

        public int SubCategoryId { get; set; }

        public int SubSubCategoryId { get; set; }

        public long CBSUserProfileId { get; set; }

        public string AlternativeContactPersonName { get; set; }

        public string AlternativeContactPersonEmail { get; set; }

        public string AlternativeContactPersonPhoneNumber { get; set; }

        public bool IsAdministrator { get; set; }

        public bool HasDifferentialWorkFlow { get; set; }

        public string OptionType { get; set; }


        public long TaxEntityId { get; set; }

        /// <summary>
        /// hold the parent service Id for when the page is returned to
        /// the options page, so as to get the service options to be loaded
        /// </summary>
        public int ParentServiceOptionId { get; set; }

        public string ParentServiceName { get; set; }

    }
}