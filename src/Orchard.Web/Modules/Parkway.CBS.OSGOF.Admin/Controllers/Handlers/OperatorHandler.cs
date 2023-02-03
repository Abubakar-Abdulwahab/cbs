using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.OSGOF.Admin.Controllers.Handlers
{
    public class OperatorHandler : IOperatorHandler
    {
        public Localizer T { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _entityCategoryRepository;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreCellSites _coreCellSites;
        private readonly IStateModelManager<StateModel> _stateModelManager;

        public ILogger Logger { get; set; }

        public OperatorHandler(IOrchardServices orchardServices, ICoreUserService coreService, ITaxEntityCategoryManager<TaxEntityCategory> entityCategoryRepository, ITaxEntityManager<TaxEntity> taxEntityManager, IStateModelManager<StateModel> stateRepo, ICoreCellSites coreCellSites, IStateModelManager<StateModel> stateModelManager)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _entityCategoryRepository = entityCategoryRepository;
            _coreUserService = coreService;
            Logger = NullLogger.Instance;
            _taxEntityManager = taxEntityManager;
            _coreCellSites = coreCellSites;
            _stateModelManager = stateModelManager;
        }


        /// <summary>
        /// Get the list of categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaxEntityCategoryViewModel> GetTaxCategories()
        {
            return _entityCategoryRepository.GetCollection(tc => tc.Status == true).Select(tc => new TaxEntityCategoryViewModel { Id = tc.Id, Name = tc.Name });
        }

        /// <summary>
        /// This saves the tax entity details
        /// Also create the payee as a CBS user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>TaxEntityProfileVM</returns>
        public RegisteredOperatorVM CreateCBSUser(SiteOperatorViewModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                int categoryId = 0;
                Int32.TryParse(model.CategoryId, out categoryId);
                TaxEntityCategory category = _coreCellSites.GetTaxEntityCategory(categoryId);                
                //TODO we should do a category validation here
                //to make sure the categoryid that is being given is a valid category
                RegisterCBSUserModel registerModel = new RegisterCBSUserModel
                {
                    Address = model.Address,
                    Password = "password",
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    TIN = model.TIN,
                    UserName = model.Username,
                    ShortName = model.Name.Substring(0, 1).ToUpper(),
                    RCNumber = model.RCNumber
                };

                if(_coreUserService.TryCreateCBSUser(registerModel, new TaxEntityCategory { Id = categoryId }, ref errors, null, null) == null) { throw new Exception("Could not save operator"); }
                return new RegisteredOperatorVM { CategoryName = category.Name };
            }
            catch (DirtyFormDataException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<TaxEntityProfileVM> GetCellSiteOperators()
        {
            var response = _taxEntityManager.GetCollection().Select(tc => new TaxEntityProfileVM { PayerId = tc.PayerId, Name = tc.Recipient });
            return response;
        }


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        public TaxPayerWithDetails GetOperator(string payerId)
        {
            try
            {
                TaxPayerWithDetails operatorDetails = _taxEntityManager.GetTaxPayerWithDetails(payerId);
                //if no record found, why don't you return null or simply throw an error
                //if (taxPayer == null) { return new TaxEntityProfileVM { }; }
                if (operatorDetails == null) { throw new NoRecordFoundException(); }
                return operatorDetails;
                //TaxEntityProfileVM operatorDetails = new TaxEntityProfileVM { Id = taxPayer.Id, Address = taxPayer.Address, Email = taxPayer.Email, Name = taxPayer.Name, TIN = taxPayer.TIN, PhoneNumber = taxPayer.PhoneNumber, PayerId = taxPayer.PayerId, TaxPayerCode = taxPayer.TaxPayerCode };
                //return operatorDetails;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting operator with Id " + payerId);
                throw;
            }
        }

    }
}