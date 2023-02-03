using Parkway.EbillsPay.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.EbillsPay
{
    public class NIBSSEBillsPay : INIBSSEBillsPay
    {


        /// <summary>
        /// Deserializes the request stream string value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestStreamString"></param>
        /// <returns>T : BaseRequest</returns>
        /// <exception cref="Exception">Throw an exception if deserialization goes wrong</exception>
        public T DeserializeXMLRequest<T>(string requestStreamString) where T : class
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestStreamString)))
                {
                    var _object = deserializer.Deserialize(stream);

                    return (T)_object;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Serialiaze response to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        public string SerializeResponseToXML<T>(T obj) where T : class
        {
            try
            {
                XmlSerializerNamespaces nameSpace = new XmlSerializerNamespaces();
                nameSpace.Add(string.Empty, string.Empty);
                using (var stringwriter = new Utf8StringWriter())
                using (var xmlWriter = XmlWriter.Create(stringwriter, new XmlWriterSettings { Indent = true }))
                {
                    xmlWriter.WriteStartDocument(true);
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(xmlWriter, obj, nameSpace);
                    //var regex = new Regex(@"\r\n?|\n|\t|", RegexOptions.Compiled);
                    //return regex.Replace(stringwriter.ToString().Trim(), String.Empty);
                    return stringwriter.ToString().Trim();
                }
            }
            catch (Exception)
            { }
            return "Error occurred";
        }

        internal class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
