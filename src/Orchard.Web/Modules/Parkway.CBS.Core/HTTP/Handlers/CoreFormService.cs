using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using Parkway.CBS.Core.Validations.FormValidators.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.FormControlsComposition;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreFormService : ICoreFormService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IFormControlsManager<FormControl> _formRepository;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxEntityCategoryRepository;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityRepository;
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        public ILogger Logger { get; set; }

        public CoreFormService(IOrchardServices orchardServices, IFormControlsManager<FormControl> formRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxEntityCategoryRepository, ICoreRevenueHeadService revenueHeadCoreService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, ITaxEntityManager<TaxEntity> taxEntityRepository)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _formRepository = formRepository;
            _taxEntityCategoryRepository = taxEntityCategoryRepository;
            _revenueHeadCoreService = revenueHeadCoreService;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _taxEntityRepository = taxEntityRepository;
        }

        /// <summary>
        /// Validate form controls by checking that each entry if complusory has a value
        /// <para>dontValidate flag tells the method to not perform further validation after the entries have been checked</para>
        /// </summary>
        /// <param name="formFields"></param>
        /// <param name="dontValidate"></param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> ValidateFormValues(IEnumerable<FormControlViewModel> formFields, bool dontValidate = false)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            foreach (var item in formFields)
            {
                if (string.IsNullOrEmpty(item.FormValue))
                {
                    if (item.IsCompulsory)
                    {
                        errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", item.Position), ErrorMessage = ErrorLang.fieldrequired().ToString() });
                    }
                    continue;
                }
                if (dontValidate) { continue; }

                LoadValidatorModel(item);
            }

            if (!dontValidate)
            {
                //now all form items have their list of validators            
                //we need to get the spread of validators across all form fields
                IEnumerable<FormControlVMFormValidatorModel> formCntrlValidatorSpread = formFields.SelectMany(vl => vl.ValidatorsModels, (grp, val) => new FormControlVMFormValidatorModel { FormControlViewModel = grp, FormValidatorModel = val });

                var groupOfFormContrlsAndFormValidators = formCntrlValidatorSpread.GroupBy(gr => gr.FormValidatorModel.ValidatorAssemblyAndClass);

                foreach (var item in groupOfFormContrlsAndFormValidators)
                {
                    DoFormControlValidations(ref errors, item.ToList());
                }
            }
            return errors;
        }        


        /// <summary>
        /// Load up the form item with the validators model
        /// <para>This method deserializes the validators JSON string is not empty
        /// transforms it to the list of validators</para>
        /// </summary>
        /// <param name="formItem"></param>
        private void LoadValidatorModel(FormControlViewModel formItem)
        {
            if (!string.IsNullOrEmpty(formItem.Validators))
            {
                formItem.ValidatorsModels = JsonConvert.DeserializeObject<List<FormValidatorModel>>(formItem.Validators);
            }
        }


        /// <summary>
        /// Get the form fields for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, int categoryId)
        {
            return BuildRevenueHeadFormFields(_formRevenueHeadRepository.GetDBForms(revenueHeadId, categoryId));
        }

        /// <summary>
        /// Get the form fields for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="payerId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, string payerId)
        {
            int categoryId = _taxEntityRepository.GetTaxPayerCategoryId(payerId);
            IEnumerable<FormControlViewModel> formControls = _formRevenueHeadRepository.GetDBForms(revenueHeadId, categoryId);
            if(formControls == null || formControls.Count() == 0)
            {
                throw new NoRecordFoundException($"No form controls configured for revenue head Id {revenueHeadId} and category Id {categoryId}");
            }
            return BuildRevenueHeadFormFields(formControls);
        }


        /// <summary>
        /// Get the form fields for this revenue head
        /// <para>this includes the data needed for the form control</para>
        /// </summary>
        /// <param name="forms"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public IEnumerable<FormControlViewModel> BuildRevenueHeadFormFields(IEnumerable<FormControlViewModel> forms)
        {
            foreach (var formVM in forms)
            {
                if (!string.IsNullOrEmpty(formVM.PartialProvider))
                {
                    formVM.FormPartialCompostionModel = JsonConvert.DeserializeObject<FormControlsCompositionModel>(formVM.PartialProvider);
                }
            }
            var groupByClassAssemblyAndClass = forms.Where(fms => fms.FormPartialCompostionModel != null).GroupBy(mp => mp.FormPartialCompostionModel.ClassAssemblyAndName);

            foreach (var group in groupByClassAssemblyAndClass)
            {
                GetFormControlData(group.ToList());
            }

            return forms;
        }


        /// <summary>
        /// This method takes the forms control filled in by the user and put the form value into 
        /// the corresponding form controls gotten from the DB
        /// <para>If for some reason the controls are not found errors are added to the ref errors object
        /// So a check for error count is necessary after this method call
        /// </para>
        /// </summary>
        /// <param name="formControlsFromUser"></param>
        /// <param name="expectedFormControlsFromDB"></param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> AddAndValidateFormValueFromUserToCorrespondingDBFormControl(IEnumerable<FormControlViewModel> formControlsFromUser, IEnumerable<FormControlViewModel> expectedFormControlsFromDB, bool dontValidate = false)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };

            foreach (var item in expectedFormControlsFromDB)
            {
                var value = formControlsFromUser?.Where(i => i.ControlIdentifier == item.ControlIdentifier).FirstOrDefault();
                if (value == null)
                {
                    if (item.IsCompulsory)
                    {
                        errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", item.Position), ErrorMessage = ErrorLang.fieldrequired().ToString() });
                    }
                    continue;
                }

                item.FormValue = value.FormValue;
                item.FormIndex = value.FormIndex;

                if (dontValidate) { continue; }

                LoadValidatorModel(item);
            }

            if (!dontValidate)
            {
                //now all form items have their list of validators            
                //we need to get the spread of validators across all form fields
                IEnumerable<FormControlVMFormValidatorModel> formCntrlValidatorSpread = expectedFormControlsFromDB.SelectMany(vl => vl.ValidatorsModels, (grp, val) => new FormControlVMFormValidatorModel { FormControlViewModel = grp, FormValidatorModel = val });

                var groupOfFormContrlsAndFormValidators = formCntrlValidatorSpread.GroupBy(gr => gr.FormValidatorModel.ValidatorAssemblyAndClass);

                foreach (var item in groupOfFormContrlsAndFormValidators)
                {
                    DoFormControlValidations(ref errors, item.ToList());
                }
            }

            return errors;
        }



        private void DoFormControlValidations(ref List<ErrorModel> errors, List<FormControlVMFormValidatorModel> formContrlsAndFormValidators)
        {
            if (formContrlsAndFormValidators[0].FormValidatorModel.IsComplexValidator)
            {
                IComplexFormValidator validator = GetComplexFormValidatorImplementation(formContrlsAndFormValidators[0].FormValidatorModel);

                foreach (var form in formContrlsAndFormValidators)
                {
                    validator.ValidateFormFields(ref errors, form.FormControlViewModel);
                }
            }
            else
            {
                IFormValidator validator = GetFormValidatorImplementation(formContrlsAndFormValidators[0].FormValidatorModel);
                foreach (var form in formContrlsAndFormValidators)
                {
                    validator.ValidateFormFields(ref errors, form.FormControlViewModel);
                }
            }
        }


        public void GetFormControlData(List<FormControlViewModel> formControlsVM)
        {
            IFormControlsComposition formCompImpl = ((IFormControlsComposition)Activator.CreateInstance(formControlsVM[0].FormPartialCompostionModel.ClassAssembly, formControlsVM[0].FormPartialCompostionModel.ClassName).Unwrap());
            formCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);
            dynamic partialModel = formCompImpl.SetFormData(formControlsVM[0]);

            foreach (var formComp in formControlsVM)
            {
                formComp.PartialModel = partialModel;
            }
        }


        /// <summary>
        /// Get implementation
        /// </summary>
        /// <param name="validator"></param>
        /// <returns>IFormValidator</returns>
        private IFormValidator GetFormValidatorImplementation(FormValidatorModel validator)
        {
            return (IFormValidator)Activator.CreateInstance(validator.ValidatorAssembly, validator.ValidatorClass).Unwrap();
        }


        /// <summary>
        /// Get complex form validator implementation
        /// </summary>
        /// <param name="validator"></param>
        /// <returns>IComplexFormValidator</returns>
        private IComplexFormValidator GetComplexFormValidatorImplementation(FormValidatorModel validator)
        {
            IComplexFormValidator complexFormVaidatorImpl = (IComplexFormValidator)Activator.CreateInstance(validator.ValidatorAssembly, validator.ValidatorClass).Unwrap();
            complexFormVaidatorImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);
            return complexFormVaidatorImpl;
        }


        /// <summary>
        /// Validate the user inputs
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userControlInputs"></param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> ValidateFormValues(int revenueHeadId, int categoryId, ICollection<FormControlViewModel> userControlInputs)
        {
            var dbForms = GetRevenueHeadFormFields(revenueHeadId, categoryId);
            if (dbForms == null || !dbForms.Any()) { return new List<ErrorModel> { }; }
            return AddAndValidateFormValueFromUserToCorrespondingDBFormControl(userControlInputs, dbForms);
        }

    }

}