using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreExtractSubCategory : ICoreExtractSubCategory
    {

        private readonly IExtractSubCategoryManager<ExtractSubCategory> _subRepo;
        public ILogger Logger { get; set; }


        public CoreExtractSubCategory(IExtractSubCategoryManager<ExtractSubCategory> subRepo)
        {
            _subRepo = subRepo;
        }


        public IEnumerable<ExtractSubCategoryVM> GetActiveSubCategories(int categoryId)
        {
            throw new NotImplementedException();
        }


    }
}