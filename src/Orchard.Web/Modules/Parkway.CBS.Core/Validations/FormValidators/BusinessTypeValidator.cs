using Newtonsoft.Json;
using NHibernate.Linq;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Validations.FormValidators.Contracts;
using Parkway.CBS.Core.Validations.FormValidators.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Validations.FormValidators
{
    public class BusinessTypeValidator : BaseValidator, IComplexFormValidator
    {
        protected ITransactionManager _transactionManager;

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        public void SetTransactionManagerForDBQueries(ITransactionManager transactionManager)
        { _transactionManager = transactionManager; }


        /// <summary>
        /// Do form element validation
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formField"></param>
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

                CheckForEmptyValue(errors, formField.FormValue, formField.FormIndex);
                if (_transactionManager.GetSession().Query<PAYEBusinessType>().Count(st => st.Name == formField.FormValue) != 1)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.businesstype404().ToString(), FieldName = string.Format("[{0}].FormValue", formField.FormIndex) });
                }
            }
            catch (DirtyFormDataException)
            { return; }
        }

    }
}