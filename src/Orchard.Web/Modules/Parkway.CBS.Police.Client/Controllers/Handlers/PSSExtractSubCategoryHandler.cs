using System;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PSSExtractSubCategoryHandler : IPSSExtractSubCategoryHandler
    {
        private readonly ICoreExtractCategory _coreExtractCategory;
        public ILogger Logger { get; set; }


        public PSSExtractSubCategoryHandler(ICoreExtractCategory coreExtractCategory)
        {
            _coreExtractCategory = coreExtractCategory;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Get sub categories for this category
        /// </summary>
        /// <param name="parsedCategoryVal"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetSubCategories(int parsedCategoryVal)
        {
            try
            {
                ExtractCategoryVM result = _coreExtractCategory.GetActiveSubCategories(parsedCategoryVal);
                if (result == null) { return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() }; }

                if (!result.FreeForm && (result.SubCategories == null || !result.SubCategories.Any())) { return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().ToString() }; }
                return new APIResponse { ResponseObject = result };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception GetSubCategories for Id {0}, Msg {1}", parsedCategoryVal, exception.Message));
            }
            return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
        }
    }
}