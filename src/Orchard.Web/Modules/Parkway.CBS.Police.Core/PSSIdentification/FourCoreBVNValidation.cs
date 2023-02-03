using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using System;

namespace Parkway.CBS.Police.Core.PSSIdentification
{
    public class FourCoreBVNValidation : IIdentificationNumberValidationImpl
    {
        public string ImplementingClassName => typeof(FourCoreBVNValidation).Name;

        private readonly ICoreFourCoreBVNValidationService _coreBVNValidationService;
        private readonly IBVNValidationResponseManager<BVNValidationResponse> _bvnValidationResponse;
        public ILogger Logger { get; set; }

        public FourCoreBVNValidation(ICoreFourCoreBVNValidationService coreBVNValidationService, IBVNValidationResponseManager<BVNValidationResponse> bvnValidationResponse)
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
                if (number.Length != 11) 
                { 
                    Logger.Error("BVN length not valid. Must be 11 characters.");
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "BVN length not valid. Must be 11 characters." };
                }

                FourCoreBVNValidationResponse validationResponse = _coreBVNValidationService.ValidateBVN(number);

                if (validationResponse == null)
                {
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "Unable to validate BVN" };
                }
                else if (!validationResponse.Status || string.IsNullOrEmpty(validationResponse.Data.BVN.LastName))
                {
                    return new ValidateIdentificationNumberResponseModel { IsActive = true, HasError = true, ErrorMessage = "BVN not found" };
                }
                else
                {
                    return new ValidateIdentificationNumberResponseModel
                    {
                        IsActive = true,
                        TaxPayerName = validationResponse.Data.BVN.FirstName + " " + validationResponse.Data.BVN.MiddleName.Trim('-', ' ') + " " + validationResponse.Data.BVN.LastName,
                        PhoneNumber = validationResponse.Data.BVN.PhoneNumber,
                        EmailAddress = validationResponse.Data.BVN.Email,
                        FirstName = validationResponse.Data.BVN.FirstName,
                        MiddleName = validationResponse.Data.BVN.MiddleName,
                        LastName = validationResponse.Data.BVN.LastName
                    };
                }
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


        public void Revalidate(string bvn)
        {
            try
            {
                _bvnValidationResponse.UpdateTaxEntityInfoWithValidationResponseForBVN(bvn);
                _bvnValidationResponse.UpdateCBSUserInfoWithValidationResponseForBVN(bvn);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _bvnValidationResponse.RollBackAllTransactions();
                throw;
            }
        }
    }
}