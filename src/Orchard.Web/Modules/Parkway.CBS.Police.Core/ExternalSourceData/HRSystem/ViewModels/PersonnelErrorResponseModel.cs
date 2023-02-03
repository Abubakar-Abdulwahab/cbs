using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels
{
    public class PersonnelErrorResponseModel
    {
        public string FieldName { get; set; }

        public string ErrorMessage { get; set; }
    }
}