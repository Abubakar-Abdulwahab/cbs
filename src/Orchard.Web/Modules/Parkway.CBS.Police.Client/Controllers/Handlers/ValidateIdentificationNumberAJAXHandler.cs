using Orchard.Logging;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ValidateIdentificationNumberAJAXHandler : IValidateIdentificationNumberAJAXHandler
    {
        private readonly IEnumerable<IIdentificationNumberValidationImpl> _identificationNumbervalidationImpl;
        private readonly IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> _identificationTypeTaxCategoryRepo;
        private readonly IIdentificationTypeManager<IdentificationType> _identificationTypeRepo;
        public ILogger Logger { get; set; }

        public ValidateIdentificationNumberAJAXHandler(IEnumerable<IIdentificationNumberValidationImpl> identificationNumbervalidationImpl, IIdentificationTypeTaxCategoryManager<IdentificationTypeTaxCategory> identificationTypeTaxCategoryRepo, IIdentificationTypeManager<IdentificationType> identificationTypeRepo)
        {
            _identificationNumbervalidationImpl = identificationNumbervalidationImpl;
            _identificationTypeTaxCategoryRepo = identificationTypeTaxCategoryRepo;
            _identificationTypeRepo = identificationTypeRepo;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets identification types available for tax category with specified id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId)
        {
            try
            {
                return _identificationTypeTaxCategoryRepo.GetIdentificationTypesForCategory(categoryId);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets Identification type with the specified Id
        /// </summary>
        /// <param name="idType"></param>
        /// <returns></returns>
        public IdentificationTypeVM validateIdType(int idType)
        {
            try
            {
                return _identificationTypeRepo.GetIdentificationTypeVM(idType);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
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
                foreach(var identificationTypeImpl in _identificationNumbervalidationImpl)
                {
                    if(identificationTypeImpl.ImplementingClassName == implementingClassName)
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

    }
}