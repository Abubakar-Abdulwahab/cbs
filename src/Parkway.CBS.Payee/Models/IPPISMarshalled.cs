using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Payee.Models
{
    [Serializable]
    public class IPPISMarshalled : MarshalByRefObject
    {
        public string ImplementingClassName { get; set; }
    }
}
