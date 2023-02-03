using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSExternalDataStateStagingVM
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
