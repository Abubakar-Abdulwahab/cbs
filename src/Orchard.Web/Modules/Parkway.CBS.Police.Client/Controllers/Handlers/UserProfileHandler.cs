using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class UserProfileHandler : IUserProfileHandler
    {
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly Lazy<ICoreTaxPayerService> _coreTaxPayerService;
        private readonly IEnumerable<Lazy<IPSSServiceTypeDetails>> _serviceTypeImpl;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> _cbsUserTaxEntityProfileLocationManager;

        public UserProfileHandler(ICoreStateAndLGA coreStateLGAService, Lazy<ICoreTaxPayerService> coreTaxPayerService, IEnumerable<Lazy<IPSSServiceTypeDetails>> serviceTypeImpl, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICBSUserTaxEntityProfileLocationManager<CBSUserTaxEntityProfileLocation> cbsUserTaxEntityProfileLocationManager)
        {
            _coreStateLGAService = coreStateLGAService;
            _coreTaxPayerService = coreTaxPayerService;
            _serviceTypeImpl = serviceTypeImpl;
            _taxCategoriesRepository = taxCategoriesRepository;
            _cbsUserTaxEntityProfileLocationManager = cbsUserTaxEntityProfileLocationManager;
        }


        /// <summary>
        /// Get model for confirm user profile
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>RegisterPSSUserObj</returns>
        public RegisterPSSUserObj GetVMToConfirmUserProfile(UserDetailsModel userDetails)
        {
            return GetModelForLoggedInUser(userDetails);
        }


        /// <summary>
        /// Try save user input
        /// </summary>
        /// <param name="userInput">TaxEntityViewModel</param>
        /// <exception cref="CannotSaveTaxEntityException"></exception>
        public TaxEntityProfileHelper TrySaveUserInfo(TaxEntityViewModel userInput, int categoryId)
        {
            if (!_coreTaxPayerService.Value.CategoryExists(categoryId)) { throw new NoCategoryFoundException("No category found for " + categoryId); }
           return _coreTaxPayerService.Value.ValidateAndSaveTaxEntity(new TaxEntity
            {
                Email = userInput.Email,
                PhoneNumber = userInput.PhoneNumber,
                TaxPayerIdentificationNumber = userInput.TaxPayerIdentificationNumber,
                Recipient = userInput.Recipient,
                StateLGA = new LGA { Id = userInput.SelectedStateLGA },
                RCNumber = userInput.RCNumber,
                Address = userInput.Address,
            }, new TaxEntityCategory { Id = categoryId });

        }


        /// <summary>
        /// check this LGA Id is valid
        /// </summary>
        /// <param name="selectedStateLGA"></param>
        /// <returns>bool</returns>
        public bool ValidateLGA(int lgaId, int stateId)
        {
            return _coreStateLGAService.GetStateIdForLGA(lgaId, stateId) == 1 ? true : false;
        }


        /// <summary>
        /// Get next direction for request
        /// </summary>
        /// <returns></returns>
        public RouteNameAndStage GetNextDirectionForRequest(int serviceTypeId)
        {
            foreach (var impl in _serviceTypeImpl)
            {
               if((PSSServiceTypeDefinition)serviceTypeId == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetDirectionAfterUserProfileConfirmation();
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + serviceTypeId );
        }
        

        private RegisterPSSUserObj GetModelForLoggedInUser(UserDetailsModel userDetails)
        {
            List<TaxEntityCategoryVM> categories = _taxCategoriesRepository.GetTaxEntityCategoryVM();
            TaxEntityProfileLocationVM location = _cbsUserTaxEntityProfileLocationManager.GetCBSUserLocationWithId(userDetails.CBSUserVM.Id);
            return new RegisterPSSUserObj
            {
                RegisterCBSUserModel = new RegisterCBSUserModel
                {
                    Address = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.Address : location.Address,
                    Email = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.Email : userDetails.CBSUserVM.Email,
                    PhoneNumber = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.PhoneNumber : userDetails.CBSUserVM.PhoneNumber,
                    Name = userDetails.TaxPayerProfileVM.Recipient,
                    RCNumber = userDetails.TaxPayerProfileVM.RCNumber,
                    SelectedState = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.SelectedState : location.State,
                    SelectedStateLGA = (userDetails.CBSUserVM.IsAdministrator) ? userDetails.TaxPayerProfileVM.SelectedStateLGA : location.LGA,
                    ContactPersonName = userDetails.TaxPayerProfileVM.ContactPersonName,
                    ContactPersonEmail = userDetails.TaxPayerProfileVM.ContactPersonEmail,
                    ContactPersonPhoneNumber = userDetails.TaxPayerProfileVM.ContactPersonPhoneNumber,
                    IdType = userDetails.TaxPayerProfileVM.IdType,
                    IdNumber = userDetails.TaxPayerProfileVM.IdNumber,
                    Gender = userDetails.TaxPayerProfileVM.Gender,
                },
                CBSUserName = userDetails.CBSUserVM.Name,
                TaxCategoriesVM = categories,
                StateLGAs = (userDetails.CBSUserVM.IsAdministrator) ? new List<StateModel> { { new StateModel { Name = userDetails.TaxPayerProfileVM.SelectedStateName, Id = userDetails.TaxPayerProfileVM.SelectedState } } } : new List<StateModel> { { new StateModel { Name = location.StateName, Id = location.State } } },
                ListLGAs = (userDetails.CBSUserVM.IsAdministrator) ? new List<LGA> { { new LGA { Id = userDetails.TaxPayerProfileVM.SelectedStateLGA, Name = userDetails.TaxPayerProfileVM.SelectedLGAName } } } : new List<LGA> { { new LGA { Id = location.LGA, Name = location.LGAName } } },
                LocationInfo = location
            };
        }

      
    }
}