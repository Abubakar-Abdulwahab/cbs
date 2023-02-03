using Newtonsoft.Json;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Validations.FormValidators.Contracts;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.Validations.FormValidators
{
    /// <summary>
    /// Validator for input box
    /// </summary>
    public class InputBoxValidator : BaseValidator, IFormValidator
    {

        /// <summary>
        /// Validation for input box
        /// </summary>
        /// <param name="formFields"></param>
        /// <returns>List{ErrorModel}</returns>
        public void ValidateFormFields(ref List<ErrorModel> errors, FormControlViewModel formField)
        {
            try
            {
                CheckFormValueEmptyIfCompulsory(formField);

                StringInputProps valProps = null;
                if (string.IsNullOrEmpty(formField.ValidationProps))
                {
                    valProps = new StringInputProps { MaxLength = 500, MinLength = 5 };
                }
                else
                {
                    valProps = JsonConvert.DeserializeObject<StringInputProps>(formField.ValidationProps);
                }
                //bool fdf = true;
                //fdf == true ? null : return;
                CheckForEmptyValue(errors, formField.FormValue, formField.FormIndex, valProps);

                CheckMinLength(errors, formField, valProps);

                CheckMaxLength(errors, formField, valProps);
            }
            catch (DirtyFormDataException)
            { return; }
        }

        
    }
}