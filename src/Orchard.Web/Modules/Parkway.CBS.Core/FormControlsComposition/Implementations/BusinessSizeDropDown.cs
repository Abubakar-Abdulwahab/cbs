using System.Linq;
using NHibernate.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public  class BusinessSizeDropDown : BaseFormControlsComposition, IFormControlsComposition
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
        /// <returns>List{PAYEBusinessSize}</returns>
        public dynamic SetFormData(FormControlViewModel formControlsVM)
        {
            //get the PAYE Business Size
            return _transactionManager.GetSession().Query<PAYEBusinessSize>().OrderBy(s => s.Id).Select(s => s.Size).ToList();
        }

    }
}