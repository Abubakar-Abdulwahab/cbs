using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Validations.FormValidators.Contracts
{
    public interface IComplexFormValidator
    {

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        void SetTransactionManagerForDBQueries(Orchard.Data.ITransactionManager transactionManager);


        /// <summary>
        /// Do form element validation
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="formField"></param>
        void ValidateFormFields(ref List<ErrorModel> errors, FormControlViewModel formField);

    }
}
