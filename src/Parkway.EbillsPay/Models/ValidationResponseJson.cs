using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public class ValidationResponseJson : ValidationResponseJsonBaseModel
    {
        /// <summary>
        /// Params object
        /// </summary>
        public Params Params { get; set; }
    }
}
