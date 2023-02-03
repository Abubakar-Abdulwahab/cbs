using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.FormControlsComposition.Implementations
{
    public class YearDropDown : BaseFormControlsComposition, IFormControlsComposition
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
            var years = new List<int>();
            //get allowed years
            for (int i = 0; i <= 30; ++i)
            {
                years.Add(DateTime.Now.Year - i);
            }

            return years;
        }
    }
}