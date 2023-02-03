using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.FormControlsComposition
{
    public interface IFormControlsComposition : IDependency
    {
        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        void SetTransactionManagerForDBQueries(Orchard.Data.ITransactionManager transactionManager);


        /// <summary>
        /// Set the data
        /// </summary>
        /// <param name="formControlsVM"></param>
        dynamic SetFormData(FormControlViewModel formControlsVM);

    }
}
