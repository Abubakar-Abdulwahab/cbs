using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class GenericRequestHandler : IGenericRequestHandler
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IPSServiceRevenueHeadManager<PSServiceRevenueHead>> _revenueServiceMan;
        private readonly Lazy<ICoreFormService> _coreFormService;
        private readonly Lazy<ICorePSService> _coreService;
        private readonly IPSServiceCaveatManager<PSServiceCaveat> _caveatRepo;
        private readonly IOrchardServices _orchardServices;

        public GenericRequestHandler(Lazy<IPSServiceRevenueHeadManager<PSServiceRevenueHead>> revenueServiceMan, Lazy<ICoreFormService> coreFormService, Lazy<ICorePSService> coreService, IPSServiceCaveatManager<PSServiceCaveat> caveatRepo, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _revenueServiceMan = revenueServiceMan;
            _coreFormService = coreFormService;
            _coreService = coreService;
            _caveatRepo = caveatRepo;
            _orchardServices = orchardServices;
        }



        /// <summary>
        /// Get VM for police extract
        /// </summary>
        /// <returns>GenericPoliceRequest</returns>
        public GenericPoliceRequest GetVMForGenericPoliceRequest(int serviceId, int categoryId)
        {
            //get service Id to determine what the request sequence is
            int initLevelId = _coreService.Value.GetInitFlow(serviceId);

            IEnumerable<PSServiceRevenueHeadVM> revenueHeadAndFormCollection = GetRevenueHeadWithFormControls(serviceId, initLevelId);

            IEnumerable<FormControlViewModel> forms = SpreadFormControls(revenueHeadAndFormCollection, categoryId);
            //do forms providers
            if (forms != null && forms.Any())
            {
                _coreFormService.Value.BuildRevenueHeadFormFields(forms).ToList();
            }

            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceCaveatVM caveat = ObjectCacheProvider.GetCachedObject<PSServiceCaveatVM>(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}");

            if (caveat == null)
            {
                caveat = _caveatRepo.GetServiceCaveat(serviceId);

                if (caveat != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}", caveat);
                }
            }

            //filter for taxcategory
            return new GenericPoliceRequest
            {
                HeaderObj = new HeaderObj { },
                ServiceName = revenueHeadAndFormCollection.ElementAt(0).ServiceName,
                Forms = forms,
                Caveat = caveat
            };
        }



        /// <summary>
        /// Do Validation
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="categoryId"></param>
        /// <param name="controlCollection"></param>
        public void GetVMForGenericPoliceRequest(GenericPoliceRequestController callback, IEnumerable<FormControlViewModel> dbformsModel, ICollection<FormControlViewModel> controlCollection)
        {
            List<ErrorModel> errors = _coreFormService.Value.AddAndValidateFormValueFromUserToCorrespondingDBFormControl(controlCollection, dbformsModel);
            //lets see if there are any errors
            if (errors != null && errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    callback.ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                }
                throw new DirtyFormDataException { };
            }
        }



        /// <summary>
        /// Get next action direction for extract
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>dynamic</returns>
        public dynamic GetNextDirectionForConfirmation()
        {
            return new { RouteName = "P.Request.Confirm", Stage = PSSUserRequestGenerationStage.PSSRequestConfirmation };
        }



        /// <summary>
        /// Get the form collection from the database
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="categoryId"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        private IEnumerable<PSServiceRevenueHeadVM> GetRevenueHeadWithFormControls(int serviceId, int levelDefinitionId)
        {
            return _revenueServiceMan.Value.GetRevenueHeadAndFormDetails(serviceId, levelDefinitionId);
        }


        /// <summary>
        /// Spread the form control for these revenueheads
        /// </summary>
        /// <param name="revenueHeadAndFormCollection"></param>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        private IEnumerable<FormControlViewModel> SpreadFormControls(IEnumerable<PSServiceRevenueHeadVM> revenueHeadAndFormCollection, int categoryId)
        {
            return revenueHeadAndFormCollection?.SelectMany(sm => sm.Forms).Where(c => c.TaxEntityCategoryId == categoryId);
        }


    }
}