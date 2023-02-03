using NHibernate.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public class LGADropDown : BaseFormControlsComposition, IFormControlsComposition
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
            return _transactionManager.GetSession().Query<LGA>().OrderBy(s => s.Name).Select(lga => new { Name = lga.Name, StateName = lga.State.Name }).ToList().GroupBy(grp => grp.StateName);
        }

    }
}