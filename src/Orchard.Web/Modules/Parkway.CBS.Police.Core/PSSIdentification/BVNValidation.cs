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
    public class BVNValidation : IIdentificationNumberValidationImpl
    {
        public string ImplementingClassName => typeof(BVNValidation).Name;

        private readonly ICoreBVNValidationService _coreBVNValidationService;
        private readonly IBVNValidationResponseManager<BVNValidationResponse> _bvnValidationResponse;
        public ILogger Logger { get; set; }

        public BVNValidation(ICoreBVNValidationService coreBVNValidationService, IBVNValidationResponseManager<BVNValidationResponse> bvnValidationResponse)
        {
            _coreBVNValidationService = coreBVNValidationService;
            _bvnValidationResponse = bvnValidationResponse;
            Logger = NullLogger.Instance;
        }


        public ValidateIdentificationNumberResponseModel Validate(string number, int idType, out string errorMessage, bool isRevalidation = false)
        {
            errorMessage = string.Empty;
            try
            {
                string validationResponse = _coreBVNValidationService.ValidateBVN(number, out errorMessage);
                if (validationResponse == null) 
                { 
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "Unable to validate BVN" };
                }
                else if (validationResponse == string.Empty) 
                { 
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "BVN not found" };
                }
                else
                {
                    dynamic responeContentObj = JsonConvert.DeserializeObject(validationResponse);
                    return new ValidateIdentificationNumberResponseModel
                    {
                        IsActive = true,
                        TaxPayerName = responeContentObj.responseObject.firstName + " " + responeContentObj.responseObject.middleName + " " + responeContentObj.responseObject.lastName,
                        PhoneNumber = responeContentObj.responseObject.phoneNumber,
                        EmailAddress = responeContentObj.responseObject.email,
                        FirstName = responeContentObj.responseObject.firstName,
                        MiddleName = responeContentObj.responseObject.middleName,
                        LastName = responeContentObj.responseObject.lastName
                    };
                }
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public ValidateIdentificationNumberResponseModel Validate(string number)
        {
            throw new NotImplementedException();
        }


        public void Revalidate(string bvn)
        {
            try
            {
                _bvnValidationResponse.UpdateTaxEntityInfoWithValidationResponseForBVN(bvn);
                _bvnValidationResponse.UpdateCBSUserInfoWithValidationResponseForBVN(bvn);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _bvnValidationResponse.RollBackAllTransactions();
                throw;
            }
        }
    }
}