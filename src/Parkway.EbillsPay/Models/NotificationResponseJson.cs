using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay.Models
{
    public class NotificationResponseJson
    {
        /// <summary>
        /// Success message if any
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether an error occured during the transaction
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// List of errors if any
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
