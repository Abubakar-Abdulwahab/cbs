using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.Validations.FormValidators
{
    public abstract class BaseValidator
    {
        /// <summary>
        /// Check to see if the form field value is empty, if so
        /// check that the form field is not complusory
        /// if the value is empty and the form field is compulsory continue to the 
        /// validation process
        /// if the form field value is empty and the form field is not compulsory return the execution 
        /// to the calling method
        /// </summary>
        /// <param name="formField"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void CheckFormValueEmptyIfCompulsory(FormControlViewModel formField)
        {
            if (string.IsNullOrEmpty(formField.FormValue) && !formField.IsCompulsory) { throw new DirtyFormDataException { }; }
        }

        /// <summary>
        /// This check performs null or empty check on a string value
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formValue"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void CheckForEmptyValue(List<ErrorModel> errors, string formValue, int formIndex)
        {
            if (string.IsNullOrEmpty(formValue))
            {
                errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", formIndex), ErrorMessage = ErrorLang.fieldrequired().ToString() });
                throw new DirtyFormDataException { };
            }
        }


        /// <summary>
        /// This check performs null or empty check on a string value
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formValue"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void CheckForEmptyValue(List<ErrorModel> errors, string formValue, int formIndex, StringInputProps valProps)
        {
            if (string.IsNullOrEmpty(formValue))
            {
                errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", formIndex), ErrorMessage = ErrorLang.inputlengthvalidation(valProps.MinLength, valProps.MaxLength).ToString() });
                throw new DirtyFormDataException { };
            }
        }


        /// <summary>
        /// Check that the string length is
        /// greater that the minlength
        /// </summary>
        /// <param name="length"></param>
        /// <param name="minLength"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void CheckMinLength(List<ErrorModel> errors, FormControlViewModel formField, StringInputProps valProps)
        {
            if (formField.FormValue.Length < valProps.MinLength)
            {
                errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", formField.FormIndex), ErrorMessage = ErrorLang.inputlengthvalidation(valProps.MinLength, valProps.MaxLength).ToString() });
                throw new DirtyFormDataException { };
            }
        }


        /// <summary>
        /// Check that the form field length doesn't exceed the max length
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formField"></param>
        /// <param name="valProps"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        protected virtual void CheckMaxLength(List<ErrorModel> errors, FormControlViewModel formField, StringInputProps valProps)
        {
            if (formField.FormValue.Length > valProps.MaxLength)
            {
                errors.Add(new ErrorModel { FieldName = string.Format("[{0}].FormValue", formField.FormIndex), ErrorMessage = ErrorLang.inputlengthvalidation(valProps.MinLength, valProps.MaxLength).ToString() });
                throw new DirtyFormDataException { };
            }
        }
    }
}