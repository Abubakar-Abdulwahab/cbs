using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class RegisterBusinessHandler : CommonBaseHandler, IRegisterBusinessHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly ICoreCollectionService _coreCollectionService;
        private readonly ICoreUserService _coreUserService;
        private readonly IHandlerHelper _handlerHelper;
        private readonly ICoreTaxPayerService _taxPayerService;

        public RegisterBusinessHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreCollectionService coreCollectionService, ICoreUserService coreUserService, ICoreTaxPayerService taxPayerService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _coreCollectionService = coreCollectionService;
            _taxPayerService = taxPayerService;
        }


        /// <summary>
        /// Try register new business as a corporate entity and create CBS user
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        public void TryRegisterBusiness(BaseController callback, RegisterBusinessObj model)
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
                int businessTypeId = 0;
                if(int.TryParse(model.RegisterBusinessModel.BusinessType, out businessTypeId))
                {
                    if (!Enum.GetValues(typeof(BusinessType)).Cast<int>().ToList().Contains(businessTypeId))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Unrecognized business type", FieldName = "RegisterBusinessModel.BusinessType" });
                        throw new DirtyFormDataException();
                    }
                }
                DoRCNumberValidation(model.RegisterBusinessModel.RCNumber, ref errors, $"RegisterBusinessModel.{nameof(model.RegisterBusinessModel.RCNumber)}", true);
                model.RegisterBusinessModel.BusinessTypeId = businessTypeId;
                _coreUserService.TryCreateCBSUser(model.RegisterBusinessModel, category, ref errors, validateEmail: true, validatePhoneNumber: true, fieldPrefix: "RegisterBusinessModel.");
            }
            catch (DirtyFormDataException)
            { AddValidationErrorsToCallback<RegisterBusinessHandler, BaseController>(callback, errors); throw new DirtyFormDataException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rcNumber"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <param name="compulsory"></param>
        private void DoRCNumberValidation(string rcNumber, ref List<ErrorModel> errors, string fieldName, bool compulsory)
        {
            if (!String.IsNullOrEmpty(rcNumber))
            {
                if (_taxPayerService.CheckCountCount(t => t.RCNumber == rcNumber) > 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "There's already a user registered with this RCNumber.", FieldName = fieldName }); throw new DirtyFormDataException();
                }
                else { return; }
            }
            else { if (!compulsory) { return; } else { errors.Add(new ErrorModel { ErrorMessage = "RCNumber not specified", FieldName = fieldName }); throw new DirtyFormDataException(); } }
        }
    }
}