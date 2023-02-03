using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.EbillsPay.Models
{
    public class NotificationResponse
    {
        //[XmlElement(ElementName = "SessionID")]
        /// <summary>
        /// Uniquely identifies a transaction
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// Unique identifier of the biller
        /// </summary>
        public string BillerID { get; set; }

        /// <summary>
        /// Indicates the status of the transaction.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// List of custom parameters
        /// </summary>
        [XmlElement(ElementName = "Param")]
        public List<Param> Param { get; set; }
    }
}
