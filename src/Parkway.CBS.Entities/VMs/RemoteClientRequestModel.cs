using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class RemoteClientRequestModel
    {
        public string URL { get; set; }

        public Dictionary<string, dynamic> Headers { get; set; }

        public dynamic Model { get; set; }
    }
}
