using Parkway.ThirdParty.Payment.Processor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.ThirdParty.Payment.Processor.Processors
{
    public class PaymentProcessorUtil
    {

        public static T GetConfigurations<T>(string xmlFilePath, string clientName) where T : BaseProcessorConfigurations
        {
            string xmlString = GetXMLString(xmlFilePath, clientName, typeof(T).Name);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xmlString))
            { return (T)serializer.Deserialize(reader); }
        }


        /// <summary>
        /// Seriablize to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        /// <exception cref="Exception"></exception>
        public static string SerializeResponseToXML<T>(T obj) where T : class
        {
            try
            {
                if (obj == null) { throw new Exception("Object is null"); }
                //Represents an XML document
                XmlDocument xmlDoc = new XmlDocument();   
                // Initializes a new instance of the XmlDocument class.   
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                // Creates a stream whose backing store is memory. 
                using (MemoryStream xmlStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(xmlStream, obj, ns);
                    xmlStream.Position = 0;
                    //Loads the XML document from the specified string.
                    xmlDoc.Load(xmlStream);
                    var xmlString = xmlDoc.InnerXml;
                    XDocument doc = XDocument.Parse(xmlString);
                    return doc.ToString();
                }
            }
            catch (Exception)
            { throw; }
        }

        private static string GetXMLString(string xmlFilePath, string clientName, string configurationName)
        {
            try
            {
                foreach (XElement elements in XElement.Load($"{xmlFilePath}\\App.xml").Elements(typeof(PaymentConfigurations).Name))
                {
                    foreach (XElement clientElement in elements.Elements(typeof(Client).Name))
                    {
                        if (clientElement.Attribute("Name").Value == clientName)
                        {
                            foreach (XElement processor in clientElement.Elements(configurationName))
                            {
                                return processor.ToString();
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            { throw new Exception(string.Format("Could not validate PaymentConfigurations for path : {0}, clientname : {1}, configName : {2} ", xmlFilePath, clientName, configurationName)); }
        }
    }
}
