using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Configuration
{
    public class RefDataConfiguration : IRefDataConfiguration
    {

        /// <summary>
        /// Get Ref Data configuration collection
        /// <para>Send the state name as it is, truncation and other string operation would be done here. No special characters are allowed</para>
        /// </summary>
        /// <returns>List{RefData}</returns>
        public List<RefData> GetCollection(string identifer)
        {
            identifer = identifer.Trim().Split(' ')[0];

            var xmlstring = (ConfigurationManager.GetSection(typeof(RefDataCollection).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(RefDataCollection));
            RefDataCollection tenantRefDataCollection = new RefDataCollection();
            using (StringReader reader = new StringReader(xmlstring))
            {
                tenantRefDataCollection = (RefDataCollection)serializer.Deserialize(reader);
            }
            if(tenantRefDataCollection == null) { return new List<RefData>(); }
            RefDataItem tenantRefDataSection = tenantRefDataCollection.RefDataItem.Where(item => item.Name == identifer + "_RefDataCollection").FirstOrDefault();
            if(tenantRefDataSection == null) { return new List<RefData>(); }
            return tenantRefDataSection.RefData;
        }
    }
}