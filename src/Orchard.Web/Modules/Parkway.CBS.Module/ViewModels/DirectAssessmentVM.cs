using Parkway.CBS.Core.Payee;
using System.Collections.Generic;

namespace Parkway.CBS.Module.ViewModels
{
    public class DirectAssessmentVM
    {
        public bool AllowFileUpload { get; set; }

        public List<AssessmentInterface> ListOfAssessmentInterface { get; set; }

        public string AdapterValue { get; set; }
    }
}