using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.CentralBillingSystem.HelperModels
{
    public class ProcessResponseModel
    {
        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public dynamic MethodReturnObject { get; set; }
    }
}
