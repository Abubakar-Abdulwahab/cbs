using System;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class ExtractRequestVM : RequestDumpVM
    {
        public List<CommandVM> ListOfCommands { get; set; }

        public List<ExtractCategoryVM> ExtractCategories { get; set; }

        public List<ExtractSubCategoryVM> ExtractSubCategories { get; set; }

        public List<ExtractCategoryVM> SelectedExtractCategoriesWithSubCategories { get; set; }

        public ICollection<int> SelectedCategories { get; set; }

        public ICollection<int> SelectedSubCategories { get; set; }

        public IDictionary<int, IEnumerable<int>> SelectedCategoriesAndSubCategoriesDeserialized { get; set; }

        public string SelectedCategoriesAndSubCategories { get; set; }

        public IEnumerable<PSSRequestExtractDetailsCategoryVM> SelectedExtractCategories { get; set; }

        public bool ShowFreeForm { get; set; }

        public bool IsIncidentReported { get; set; }

        public string IncidentReportedDate { get; set; }

        public string AffidavitNumber { get; set; }

        public string AffidavitDateOfIssuance { get; set; }

        public DateTime? AffidavitDateOfIssuanceParsed { get; set; }

        public string FileUploadName { get; set; }

        public string FileUploadPath { get; set; }

        public bool HasFileUpload { get; set; }

        public bool ViewedTermsAndConditionsModal { get; set; }

        public PSServiceCaveatVM Caveat { get; set; }

    }
}