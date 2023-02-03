using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Validations.FormValidators.Contracts;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Core.Validations.FormValidators
{
    /// <summary>
    /// Validation for Year drop down
    /// </summary>
    public class YearDropDownValidator : BaseValidator, IFormValidator
    {
        /// <summary>
        /// Validation for year drop down
        /// </summary>
        /// <param name="formFields"></param>
        /// <returns>List{ErrorModel}</returns>
        public void ValidateFormFields(ref List<ErrorModel> errors, FormControlViewModel formField)
        {
            try
            {
                CheckFormValueEmptyIfCompulsory(formField);

                YearDropDownProps valProps = null;
                if (string.IsNullOrEmpty(formField.ValidationProps))
                {
                    valProps = new YearDropDownProps { MaxYear = DateTime.Now.Year, MinYear = DateTime.Now.Year - 30 };
                }
                else
                {
                    valProps = JsonConvert.DeserializeObject<YearDropDownProps>(formField.ValidationProps);
                }

                CheckForEmptyValue(errors, formField.FormValue, formField.FormIndex);

                CheckYearWithinRange(errors, formField, valProps);
            }
            catch (DirtyFormDataException)
            { return; }
        }

        /// <summary>
        /// Check that the form field value is within the accepted year boundary
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formField"></param>
        /// <param name="valProps"></param>
        protected void CheckYearWithinRange(List<ErrorModel> errors, FormControlViewModel formField, YearDropDownProps valProps)
        {
            int parsedYear;
            if (int.TryParse(formField.FormValue, out parsedYear))
            {
                if (parsedYear > valProps.MaxYear || parsedYear < valProps.MinYear)
                {
                    errors.Add(new ErrorModel { FieldName = string.Format("[{0}].formValue", formField.FormIndex), ErrorMessage = ErrorLang.yearrangevalidation(valProps.MinYear, valProps.MaxYear).ToString() });
                    throw new DirtyFormDataException { };
                }
            }
        }
    }
}