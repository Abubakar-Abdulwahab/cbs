using System.Linq;
using NHibernate.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public  class StateDropDown : BaseFormControlsComposition, IFormControlsComposition
    {

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        public override void SetTransactionManagerForDBQueries(Orchard.Data.ITransactionManager transactionManager)
        { _transactionManager = transactionManager; }


        /// <summary>
        /// Set the data
        /// </summary>
        /// <param name="formControlsVM"></param>
        /// <returns>List{StateModel}</returns>
        public dynamic SetFormData(FormControlViewModel formControlsVM)
        {
            //get the states
            return _transactionManager.GetSession().Query<StateModel>().OrderBy(s => s.Name).Select(s => s.Name).ToList();
        }

    }
}