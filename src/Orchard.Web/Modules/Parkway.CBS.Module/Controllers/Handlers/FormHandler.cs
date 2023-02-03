using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Validations.Contracts;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Modules.Services;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class FormHandler : BaseHandler, IFormHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;

        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxPayerCategoryManager;
        private readonly ITINFormManager<TINRegistrationForm> _tinFormManager;

        public Localizer T { get; set; }
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IValidator _validator;
        private readonly IRevenueHeadHandler _revenueHeadHandler;
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        //public ILogger Logger { get; set; }

        public FormHandler(IOrchardServices orchardServices, IValidator validator,
            IFormControlsManager<FormControl> formcontrolsRepository, IRevenueHeadHandler revenueHeadHandler, ITINFormManager<TINRegistrationForm> tinFormManager,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IModuleService moduleService, ITaxEntityCategoryManager<TaxEntityCategory> taxPayerCategoryManager, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _contentManager = orchardServices.ContentManager;
            _transactionManager = orchardServices.TransactionManager;
            T = NullLocalizer.Instance;
            _formcontrolsRepository = formcontrolsRepository;
            _validator = validator;
            _revenueHeadHandler = revenueHeadHandler;
            Logger = NullLogger.Instance;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _taxPayerCategoryManager = taxPayerCategoryManager;
            _settingsRepository = settingsRepository;
            _tinFormManager = tinFormManager;
        }

        #region Create ops

        /// <summary>
        /// Get view for form setup
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>FormSetupViewModel</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception> 
        public FormSetupViewModel GetFormSetupView(int revenueHeadId, string revenueHeadSlug)
        {
            IsAuthorized<FormHandler>(Permissions.CreateForm);
            var revenueHead = GetRevenueHead(revenueHeadId, revenueHeadSlug);
            HasBillingInfo(revenueHead);//.HasForm(revenueHead, false);

            //get all the controls
            //get all tax entity categories
            //render controls for each categories
            //var controls = _formcontrolsRepository.GetCollection(f => f.RevenueHead == null);
            var controls = _formcontrolsRepository.GetCollection(f => f.Id != 0);
            var taxPayerCategories = _taxPayerCategoryManager.GetCollection(x => x.Status == true);

            //var groups = controls.GroupBy(c => c.TaxEntityCategory.Id);

            //var dfd = taxPayerCategories.Where()
            return new FormSetupViewModel()
            {
                RevenueHeadName = revenueHead.NameAndCode(),
                ControlsPerEntity = taxPayerCategories
                //.Where(frm => controls.Any(ctrl => (ctrl.TaxEntityCategory == frm) || ctrl.TaxEntityCategory == null))
                .Select(x => new FormControlRevenueHeadMetaDataExtended() {
                    TaxEntityCategoryId = x.Id,
                    TaxEntityCategoryName = x.Name,
                    FormControls = controls.Select(c => new FormControlViewModel()
                    {
                        ControlTypeDropDownNumber = c.ControlTypeDropDownNumber,
                        ControlTypeNumber = c.ControlTypeNumber,
                        DefaultStatus = c.DefaultStatus,
                        ElementType = c.ElementType,
                        FriendlyName = c.FriendlyName,
                        HintText = c.HintText,
                        TechnicalName = c.TechnicalName,
                        ControlIdentifier = c.Id,
                        TaxEntityCategoryId = x.Id,
                        LabelText = c.LabelText,
                        Name = c.Name,
                        PlaceHolderText = c.PlaceHolderText
                    }).ToList(),

                    FormControlIds = new List<int>()
                })
                ,               
                Form = new FormControl()
            };
        }



        /// <summary>
        /// Save form data and the corresponding revenue head
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        public void TrySaveFormDetails(FormController callback, ICollection<FormControlViewModel> controlCollection, int revenueHeadId, string revenueHeadSlug)
        {
            IsAuthorized<FormHandler>(Permissions.CreateForm);
            var revenueHead = GetRevenueHead(revenueHeadId, revenueHeadSlug);
            HasBillingInfo(revenueHead);//.HasForm(revenueHead, false);
            if (controlCollection == null) { return; }           
            CreateFormControlsFormRevenueHead(controlCollection, revenueHead);
        }

        

        #endregion

        #region Edit ops

        /// <summary>
        /// Get view for form setup edit
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        public FormSetupEditModel GetEditFormSetupView(int revenueHeadId, string revenueHeadSlug, out List<FormControlRevenueHeadMetaDataExtended> alreadyExistingControls)
        {
            IsAuthorized<FormHandler>(Permissions.CreateForm);
            var revenueHead = GetRevenueHead(revenueHeadId, revenueHeadSlug);
            var controls = _formcontrolsRepository.GetCollection(f => f.Id != 0);
            var taxPayerCategories = _taxPayerCategoryManager.GetCollection(x => x.Status == true);
            var taxPayerCategoryIds = taxPayerCategories.Select(x => x.Id);

            var controlsAlreadyOwnedByRevenueHeadWithTaxCategoryId = new List<FormControlRevenueHeadMetaDataExtended>();
            try
            {
                controlsAlreadyOwnedByRevenueHeadWithTaxCategoryId = _formRevenueHeadRepository.GetCollection(x => x.RevenueHead.Id == revenueHead.Id && taxPayerCategoryIds.Contains(x.TaxEntityCategory.Id))
                   .Select(x => new FormControlRevenueHeadMetaDataExtended
                   {
                       TaxEntityCategoryName = x.TaxEntityCategory.Name,
                       TaxEntityCategoryId = x.TaxEntityCategory.Id,
                       FormControlIds = JsonConvert.DeserializeObject<List<int>>(x.MetaData)
                   })
                    .ToList();
            }
            catch (Exception)
            {
            }

            controlsAlreadyOwnedByRevenueHeadWithTaxCategoryId.ForEach(x => x.FormControls = _formcontrolsRepository.GetCollection(c => x.FormControlIds.Contains(c.Id)).Select(c => new FormControlViewModel()
            {
                ControlTypeDropDownNumber = c.ControlTypeDropDownNumber,
                ControlTypeNumber = c.ControlTypeNumber,
                DefaultStatus = c.DefaultStatus,
                ElementType = c.ElementType,
                FriendlyName = c.FriendlyName,
                HintText = c.HintText,
                TechnicalName = c.TechnicalName,
                ControlIdentifier = c.Id,
                TaxEntityCategoryId = x.TaxEntityCategoryId,
                LabelText = c.LabelText,
                Name = c.Name,
                PlaceHolderText = c.PlaceHolderText
            }));

            alreadyExistingControls = controlsAlreadyOwnedByRevenueHeadWithTaxCategoryId;
            ////logic here is that for corporate type we send the individual para to the BuildEditControls
            //var revHeadControlsForCorporate = HasBillingInfo(revenueHead).HasForm(revenueHead, true).BuildEditControls(revenueHead, TaxPayerType.Individual);
            //var revHeadControlsForIndividual = BuildEditControls(revenueHead, TaxPayerType.Corporate);
            return new FormSetupEditModel()
            {
                RevenueHeadName = revenueHead.NameAndCode(),
                ControlsPerEntity = taxPayerCategories.Select(x => new FormControlRevenueHeadMetaDataExtended()
                {
                    TaxEntityCategoryId = x.Id,
                    TaxEntityCategoryName = x.Name,
                    FormControls = controls.Select(c => new FormControlViewModel()
                    {
                        ControlTypeDropDownNumber = c.ControlTypeDropDownNumber,
                        ControlTypeNumber = c.ControlTypeNumber,
                        DefaultStatus = c.DefaultStatus,
                        ElementType = c.ElementType,
                        FriendlyName = c.FriendlyName,
                        HintText = c.HintText,
                        TechnicalName = c.TechnicalName,
                        ControlIdentifier = c.Id,
                        TaxEntityCategoryId = x.Id,
                        LabelText = c.LabelText,
                        Name = c.Name,
                        PlaceHolderText = c.PlaceHolderText
                    }).ToList(),
                    FormControlIds = new List<int>()
                }),         
                Slug = revenueHead.Slug,
                Id = revenueHead.Id
            };
        }

        /// <summary>
        /// Save or update form controls
        /// </summary>
        /// <param name="formController"></param>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        public void TryUpdateFormDetails(ICollection<FormControlViewModel> controlCollection, int revenueHeadId, string revenueHeadSlug)
        {
            IsAuthorized<FormHandler>(Permissions.CreateForm);
            var revenueHead = GetRevenueHead(revenueHeadId, revenueHeadSlug);
            HasBillingInfo(revenueHead).UpdateFormControlsFormRevenueHead(controlCollection, revenueHead);
        }
        #endregion

        /// <summary>
        /// Check if the revenue head already has created a form.
        /// Send true if you are trying to perform an edit, or false
        /// if you are performing a create
        /// </summary>
        /// <param name="created"></param>
        /// <returns>FormHandler</returns>
        /// <exception cref="CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException"></exception>
        private FormHandler HasForm(RevenueHead revenueHead, bool created)
        {
            if (revenueHead.FormControls.Count() < 1 && created) { throw new CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException(); }
            if (revenueHead.FormControls.Count() > 0 && !created) { throw new CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException(); }
            return this;
        }

        /// <summary>
        /// Create the formcontrol revenue head records
        /// </summary>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHead"></param>
        private void CreateFormControlsFormRevenueHead(ICollection<FormControlViewModel> controlCollection, RevenueHead revenueHead)
        {
            var controlsGroupedByTaxEntityCategoryId = controlCollection.Where(ctrl => ctrl.DefaultStatus).GroupBy(c => c.TaxEntityCategoryId);
            List<FormControlRevenueHead> formCtrRvh = new List<FormControlRevenueHead>();
            foreach (var group in controlsGroupedByTaxEntityCategoryId)
            {
                var fcrvh = new FormControlRevenueHead()
                {
                    TaxEntityCategory = _taxPayerCategoryManager.Get(group.Key),
                    RevenueHead = revenueHead
                };
                List<long> formControlIds = new List<long>();
                foreach (var item in group)
                {
                    formControlIds.Add(item.ControlIdentifier);
                }
                fcrvh.MetaData = JsonConvert.SerializeObject(formControlIds);

                formCtrRvh.Add(fcrvh);
            }
            
            //var result = controlCollection.Where(ctrl => ctrl.DefaultStatus).Select(ctrl => new FormControlRevenueHead() { /*ForCustomer = 1*//*ctrl.ForCustomerType*/ FormControl = ctrl, RevenueHead = revenueHead, });
            if (!_formRevenueHeadRepository.SaveBundle(formCtrRvh)) { throw new CouldNotSaveFormRevenueHeadControls(); };
        }


        /// <summary>
        /// Commit updated data
        /// </summary>
        /// <param name="controlCollection"></param>
        /// <param name="revenueHead"></param>
        private void UpdateFormControlsFormRevenueHead(ICollection<FormControlViewModel> controlCollection, RevenueHead revenueHead)
        {
            var controlsGroupedByTaxEntityCategoryId = controlCollection.Where(ctrl => ctrl.DefaultStatus).GroupBy(c => c.TaxEntityCategoryId);
            try
            {
                foreach (var group in controlsGroupedByTaxEntityCategoryId)
                {
                    var fcrvh = _formRevenueHeadRepository.Get(x => x.TaxEntityCategory == _taxPayerCategoryManager.Get(group.Key) && x.RevenueHead == revenueHead);
                    
                    List<long> formControlIds = new List<long>();
                    foreach (var item in group)
                    {
                        formControlIds.Add(item.ControlIdentifier);
                    }
                    fcrvh.MetaData = JsonConvert.SerializeObject(formControlIds);

                    _formRevenueHeadRepository.Update(fcrvh);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        
        /// <summary>
        /// Has billing info
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>FormHandler</returns>
        /// <exception cref="NoBillingDetailAddedYetException"></exception>
        private FormHandler HasBillingInfo(RevenueHead revenueHead)
        {
            try
            {
                if (revenueHead.BillingModel.CreatedAtUtc == null) { throw new NoBillingDetailAddedYetException(); }
            }
            catch (NHibernate.ObjectNotFoundException) { throw new NoBillingDetailAddedYetException(); }
            catch (NullReferenceException) { throw new NoBillingDetailAddedYetException(); }
            return this;
        }

        /// <summary>
        /// Get revenue head
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        private RevenueHead GetRevenueHead(int id, string slug)
        {
            return _revenueHeadHandler.GetRevenueHead(slug, id);
        }

        

        internal class ControlComparer : IEqualityComparer<FormControlRevenueHead>
        {
            public bool Equals(FormControlRevenueHead x, FormControlRevenueHead y)
            {
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.Form.Id == y.Form.Id;
            }

            public int GetHashCode(FormControlRevenueHead obj)
            {
                if (Object.ReferenceEquals(obj, null)) return 0;
                return obj.Form.Id.GetHashCode();
            }

            
        }
    }
}