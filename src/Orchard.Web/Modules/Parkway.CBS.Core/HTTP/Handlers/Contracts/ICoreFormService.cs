using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreFormService : IDependency
    {

        /// <summary>
        /// This method takes the forms control filled in by the user and put the form value into 
        /// the corresponding form controls gotten from the DB
        /// <para>If for some reason the controls are not found errors are added to the ref errors object
        /// So a check for error count is necessary after this method call
        /// </para>
        /// </summary>
        /// <param name="formControlsFromUser"></param>
        /// <param name="expectedFormControlsFromDB"></param>
        /// <returns></returns>
        List<ErrorModel> AddAndValidateFormValueFromUserToCorrespondingDBFormControl(IEnumerable<FormControlViewModel> formControlsFromUser, IEnumerable<FormControlViewModel> expectedFormControlsFromDB, bool dontValidate = false);


        /// <summary>
        /// Validate form controls by checking that each entry if complusory has a value
        /// <para>dontValidate flag tells the method to not perform further validation after the entries have been checked</para>
        /// </summary>
        /// <param name="formFields"></param>
        /// <param name="dontValidate"></param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> ValidateFormValues(IEnumerable<FormControlViewModel> formFields, bool dontValidate = false);


        /// <summary>
        /// Get the form fields for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, int categoryId);

        /// <summary>
        /// Get the form fields for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="payerId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, string payerId);


        /// <summary>
        /// Get the form fields for this revenue head
        /// </summary>
        /// <param name="forms"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        IEnumerable<FormControlViewModel> BuildRevenueHeadFormFields(IEnumerable<FormControlViewModel> forms);


        /// <summary>
        /// Validate the user inputs
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userControlInputs"></param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> ValidateFormValues(int revenueHeadId, int categoryId, ICollection<FormControlViewModel> userControlInputs);

    }
}
