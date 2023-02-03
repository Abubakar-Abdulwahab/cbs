using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using System;

namespace Parkway.CBS.Police.Core.PSSIdentification
{
    public class TINValidation : IIdentificationNumberValidationImpl
    {
        public string ImplementingClassName => typeof(TINValidation).Name;
        private readonly ICoreTINValidationService _coreTINValidationService;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly ITINValidationResponseManager<TINValidationResponse> _tinValidationResponseRepo;
        public ILogger Logger { get; set; }

        public TINValidation(ICoreTINValidationService coreTINValidationService, ITaxEntityManager<TaxEntity> taxEntityManager, ITINValidationResponseManager<TINValidationResponse> tinValidationResponseRepo)
        {
            _coreTINValidationService = coreTINValidationService;
            _taxEntityManager = taxEntityManager;
            _tinValidationResponseRepo = tinValidationResponseRepo;
            Logger = NullLogger.Instance;
        }


        public ValidateIdentificationNumberResponseModel Validate(string number, int idType, out string errorMessage, bool isRevalidation = false)
        {
            errorMessage = string.Empty;
            try
            {
                if (_taxEntityManager.Count(x => x.IdentificationNumber == number && x.IdentificationType == idType ) > 0 && !isRevalidation) 
                {
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "User with the specified Tax Identification Number already exists." };
                }
                string validationResponse = _coreTINValidationService.ValidateTIN(number, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = errorMessage };
                }
                dynamic responeContentObj = JsonConvert.DeserializeObject(validationResponse);
                return new ValidateIdentificationNumberResponseModel
                {
                    IsActive = true,
                    TaxPayerName = responeContentObj.TaxPayerName,
                    PhoneNumber = responeContentObj.Phone,
                    RCNumber = responeContentObj.RCNumber,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public ValidateIdentificationNumberResponseModel Validate(string number)
        {
            throw new NotImplementedException();
        }


        public void Revalidate(string tin)
        {
            try
            {
                _tinValidationResponseRepo.UpdateTaxEntityInfoWithValidationResponseForTIN(tin);
                _tinValidationResponseRepo.UpdateCBSUserInfoWithValidationResponseForTIN(tin);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _tinValidationResponseRepo.RollBackAllTransactions();
                throw;
            }
        }
    }
}