using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class BaseHRResponseVM
    {
        public bool Error { get; set; }

        public string ErrorCode { get; set; }

        public object ResponseObject { get; set; }
    }
}
