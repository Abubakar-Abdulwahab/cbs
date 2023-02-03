using Orchard;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Logging;
using Parkway.CBS.Client.Web.ViewModels;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class BvnValidationHandler : CommonBaseHandler, IBvnValidationHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly ICoreCollectionService _coreCollectionService;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreTaxPayerService _taxPayerService;

        public BvnValidationHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, ICoreCollectionService coreCollectionService, ICoreTaxPayerService taxPayerService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreCollectionService = coreCollectionService;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _taxPayerService = taxPayerService;
        }

        /// <summary>
        /// Register new user and generate state tin (payerid)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public RegisterUserResponse TryRegisterCBSUser(BvnValidationController callback, ValidateBvnVM model)
        {
            //get the category
            Logger.Information("Getting category");
            int catId = 0;
            bool parsed = Int32.TryParse(model.TaxPayerType, out catId);
            if (!parsed) { throw new NoCategoryFoundException(); }
            TaxEntityCategory category = _coreCollectionService.GetTaxEntityCategory(catId);
            //check if the category is not null
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                return _coreUserService.TryCreateCBSUser(model.RegisterCBSUserModel, category, ref errors, fieldPrefix: "RegisterCBSUserModel");
            }
            catch (DirtyFormDataException)
            { AddValidationErrorsToCallback<BvnValidationHandler, BvnValidationController>(callback, errors); throw new DirtyFormDataException(); }
        }

        /// <summary>
        /// Check if a particular BVN has been registered
        /// </summary>
        /// <param name="bvn"></param>
        /// <returns></returns>
        public APIResponse CheckIfBvnExists(string bvn)
        {
            try
            {
                if (_taxPayerService.CheckCountCount(x => x.BVN == bvn) > 0)
                {
                    return new APIResponse { ResponseObject = new { Message = "User with specified BVN already exists", Registered = true } };
                }
                else
                {
                    return new APIResponse { ResponseObject = new { Message = "User with specified BVN does not exist", Registered = false } };
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception,exception.Message);
            }
            return new APIResponse { Error = true };
        }
    }
}