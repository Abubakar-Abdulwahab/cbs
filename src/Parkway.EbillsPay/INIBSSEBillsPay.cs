using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.EbillsPay
{
    public interface INIBSSEBillsPay
    {

        /// <summary>
        /// Deserialize request string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestStreamString"></param>
        /// <returns>T</returns>
        T DeserializeXMLRequest<T>(string requestStreamString) where T : class;

        /// <summary>
        /// Serialiaze response to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        string SerializeResponseToXML<T>(T obj) where T : class;
    }
}
