using System.Linq;
using NHibernate.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public  class BusinessTypeDropDown : BaseFormControlsComposition, IFormControlsComposition
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
        /// <returns>List{PAYEBusinessType}</returns>
        public dynamic SetFormData(FormControlViewModel formControlsVM)
        {
            //get the PAYE Business Type
            return _transactionManager.GetSession().Query<PAYEBusinessType>().OrderBy(s => s.Name).Select(s => s.Name).ToList();
        }

    }
}