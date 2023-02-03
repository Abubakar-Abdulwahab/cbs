using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using System;
using System.Globalization;

namespace Parkway.CBS.Police.Core.PSSIdentification
{
    public class NINValidation : IIdentificationNumberValidationImpl, IIdentityBiometric
    {
        public string ImplementingClassName => typeof(NINValidation).Name;

        private readonly ICoreNINValidationService _coreNINValidationService;
        private readonly ININValidationResponseManager<NINValidationResponse> _ninValidationResponseRepo;
        public ILogger Logger { get; set; }

        public NINValidation(ICoreNINValidationService coreNINValidationService, ININValidationResponseManager<NINValidationResponse> ninValidationResponseRepo)
        {
            _coreNINValidationService = coreNINValidationService;
            _ninValidationResponseRepo = ninValidationResponseRepo;
            Logger = NullLogger.Instance;
        }


        public ValidateIdentificationNumberResponseModel Validate(string number, int idType, out string errorMessage, bool isRevalidation = false)
        {
            errorMessage = string.Empty;
            try
            {
                if (number.Length != 11) { return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "NIN length not valid. Must be 11 characters" }; }
                NINValidationResponse validationResponse = _coreNINValidationService.ValidateNIN(number, out errorMessage);
                if (validationResponse == null)
                {
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = ErrorLang.ToLocalizeString("NIN not found").Text };
                }

                return new ValidateIdentificationNumberResponseModel
                {
                    IsActive = true,
                    TaxPayerName = validationResponse.FirstName + " " + validationResponse.MiddleName + " " + validationResponse.Surname,
                    PhoneNumber = validationResponse.TelephoneNo,
                    EmailAddress = validationResponse.Email,
                    FirstName = validationResponse.FirstName,
                    MiddleName = validationResponse.MiddleName,
                    LastName = validationResponse.Surname,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates the specified identification number
        /// </summary>
        /// <param name="number"></param>
        /// <returns>ValidateIdentificationNumberResponseModel</returns>
        public ValidateIdentificationNumberResponseModel Validate(string number)
        {
            try
            {
                string errorMessage = string.Empty;
                NINValidationResponse validationResponse = null;
                LatestNINValidationResponseVM latestNINValidationResponse = null;
                if (number.Length != 11) { return new ValidateIdentificationNumberResponseModel { HasError = true, ErrorMessage = "NIN length not valid. Must be 11 characters" }; }

                int NINPCCRevalidationIntervalDays = 0;
                string sNINPCCRevalidationIntervalDays = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.NINPCCRevalidationIntervalDays);

                if (!string.IsNullOrEmpty(sNINPCCRevalidationIntervalDays))
                {
                    int.TryParse(sNINPCCRevalidationIntervalDays, out NINPCCRevalidationIntervalDays);
                }

                //get lastest nin response record from db
                latestNINValidationResponse = _ninValidationResponseRepo.GetNINValidationResponse(number);
                if (latestNINValidationResponse == null || (DateTime.Now - latestNINValidationResponse.CreatedAtUtc).Days >= NINPCCRevalidationIntervalDays)
                {
                    //if record does not exist in db or last nin call is not less than than limit(days), call NIN
                    validationResponse = _coreNINValidationService.ValidateNIN(number, out errorMessage);
                    if (validationResponse != null)
                    {
                        latestNINValidationResponse = new LatestNINValidationResponseVM { BirthDate = validationResponse.BirthDate, CreatedAtUtc = validationResponse.CreatedAtUtc };
                    }
                }

                if (latestNINValidationResponse == null)
                {
                    return new ValidateIdentificationNumberResponseModel { HasError = true, ErrorMessage = ErrorLang.ToLocalizeString("NIN not found").ToString() };
                }


                return new ValidateIdentificationNumberResponseModel
                {
                    DateOfBirth = DateTime.ParseExact(latestNINValidationResponse.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public void Revalidate(string nin)
        {
            try
            {
                _ninValidationResponseRepo.UpdateTaxEntityInfoWithValidationResponseForNIN(nin);
                _ninValidationResponseRepo.UpdateCBSUserInfoWithValidationResponseForNIN(nin);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _ninValidationResponseRepo.RollBackAllTransactions();
                throw;
            }
        }

    }
}