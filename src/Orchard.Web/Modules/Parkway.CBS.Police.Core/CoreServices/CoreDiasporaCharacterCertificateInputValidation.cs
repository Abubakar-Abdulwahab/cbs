using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;


namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreDiasporaCharacterCertificateInputValidation : ICoreDiasporaCharacterCertificateInputValidation
    {
        private readonly ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> _requestInquiryRepo;
        private readonly ICoreCountryService _coreCountryService;
        private readonly ICoreHelperService _corehelper;
        private readonly ITaxEntityManager<TaxEntity> _taxPayerRepo;
        private readonly IIdentificationTypeManager<IdentificationType> _identificationTypeRepo;
        private readonly IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> _identityTypeTaxCategoryRepo;
        private readonly IEnumerable<IIdentificationNumberValidationImpl> _identificationNumbervalidationImpl;
        public ILogger Logger { get; set; }

        public CoreDiasporaCharacterCertificateInputValidation(ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> requestInquiryRepo, ICoreCountryService coreCountryService, ICoreHelperService corehelper, ITaxEntityManager<TaxEntity> taxPayerRepo, IIdentificationTypeManager<IdentificationType> identificationTypeRepo, IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> identityTypeTaxCategoryRepo, IEnumerable<IIdentificationNumberValidationImpl> identificationNumbervalidationImpl)
        {
            _requestInquiryRepo = requestInquiryRepo;
            _coreCountryService = coreCountryService;
            _corehelper = corehelper;
            _taxPayerRepo = taxPayerRepo;
            _identificationTypeRepo = identificationTypeRepo;
            _identityTypeTaxCategoryRepo = identityTypeTaxCategoryRepo;
            _identificationNumbervalidationImpl = identificationNumbervalidationImpl;
            Logger = NullLogger.Instance;
        }



        /// <summary>
        /// Validate identity details
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        public void ValideateIdentityDetails(PCCDiasporaUserInputVM userInput, List<ErrorModel> errors, long taxEntityId, int taxEntityCategoryId)
        {
            //first we need to check that the identification type tied to the user has biometric capture 
            KeyValuePair<int, string> identificationTypeAndValue = _taxPayerRepo.GetIdentificationTypeAndValue(taxEntityId);
            //now that we have gotten the identification value we need to check that this identification has biometric support
            IdentificationTypeVM identificationType = _identificationTypeRepo.GetIdentificationTypeVM(identificationTypeAndValue.Key);
            if(identificationType == null)
            { errors.Add(new ErrorModel { FieldName = nameof(PCCDiasporaUserInputVM.SelectedIdentityType), ErrorMessage = PoliceErrorLang.selected_option_404().ToString() }); return; }

            if (!identificationType.HasBiometricSupport)
            {
                //if does not have biometric support
                //the user must have chosen what identification type their biometric is on
                //first we check that the user has selected the correct identification by based on their tax category
                identificationType = GetIdentityTypeWithTaxCategory(userInput.SelectedIdentityType, taxEntityCategoryId, errors);
                if (errors.Count > 0) { return; }
                //now that we know that the combination exists
                //we need to check that the values entered are correct
                //we need to validate the identity value provided againsts the identity type
                ValidateForDOB(userInput, identificationType, errors);
            }
        }


        /// <summary>
        /// validate the date of birth
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="identityType"></param>
        /// <param name="errors"></param>
        private void ValidateForDOB(PCCDiasporaUserInputVM userInput, IdentificationTypeVM identityType, List<ErrorModel> errors)
        {
            try
            {
                foreach (var identificationTypeImpl in _identificationNumbervalidationImpl)
                {
                    if (identificationTypeImpl.ImplementingClassName == identityType.ImplementingClassName)
                    {
                        ValidateIdentificationNumberResponseModel result = identificationTypeImpl.Validate(userInput.IdentityValue?.Trim());

                        if (result.HasError)
                        { errors.Add(new ErrorModel { FieldName = nameof(PCCDiasporaUserInputVM.IdentityValue), ErrorMessage = result.ErrorMessage }); return; }

                        //based on the result we need to check the DOB
                        DateTime inputtedDOB = DateTime.ParseExact(userInput.DateOfBirth.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (DateTime.Compare(inputtedDOB.Date, result.DateOfBirth.Date) != 0)
                        { errors.Add(new ErrorModel { FieldName = nameof(PCCDiasporaUserInputVM.DateOfBirth), ErrorMessage = PoliceErrorLang.mismatch_in_dateofbirth().ToString() }); return; }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = nameof(PCCDiasporaUserInputVM.DateOfBirth), ErrorMessage = PoliceErrorLang.genericexception().ToString() });
            }
        }


        /// <summary>
        /// Validate identification number using the specified Identification type implementing class name
        /// </summary>
        /// <param name="idNumber"></param>
        /// <param name="idType"></param>
        /// <param name="implementingClassName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public ValidateIdentificationNumberResponseModel ValidateIdentificationNumber(string idNumber, int idType, string implementingClassName, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                foreach (var identificationTypeImpl in _identificationNumbervalidationImpl)
                {
                    if (identificationTypeImpl.ImplementingClassName == implementingClassName)
                    {
                        return identificationTypeImpl.Validate(idNumber, idType, out errorMessage);
                    }
                }
                return new ValidateIdentificationNumberResponseModel { IsActive = false }; // if we do not have an implementation(api) for validating the specified identification type
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validate that the selected Identification Type TaxCategory
        /// is valid for this tax category the user is assigned to
        /// </summary>
        /// <param name="selectedIdentityTypeId"></param>
        /// <param name="taxEntityCategoryId"></param>
        /// <param name="errors"></param>
        /// <returns>IdentificationTypeVM</returns>
        private IdentificationTypeVM GetIdentityTypeWithTaxCategory(int selectedIdentityTypeTaxCategoryId, int taxEntityCategoryId, List<ErrorModel> errors)
        {
            if (selectedIdentityTypeTaxCategoryId <= 0)
            { errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.selected_option_404().ToString(), FieldName = nameof(PCCDiasporaUserInputVM.SelectedIdentityType) }); return null; }

            IdentificationTypeVM identityType = _identityTypeTaxCategoryRepo.GetIdentityTypeWithTaxCategory(selectedIdentityTypeTaxCategoryId, taxEntityCategoryId);

            if (identityType is null)
            {
                errors.Add(new ErrorModel { ErrorMessage = PoliceErrorLang.selected_option_404().ToString(), FieldName = nameof(PCCDiasporaUserInputVM.SelectedIdentityType) }); return null;
            }

            return identityType;
        }
        

    }
}