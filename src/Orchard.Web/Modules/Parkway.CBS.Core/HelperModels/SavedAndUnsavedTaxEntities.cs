using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SavedAndUnsavedTaxEntities
    {
        public List<FailedAtCashflowModel> FailedToSave { get; set; }

        public List<TaxEntity> SavedSuccessfully { get; set; }
    }
}