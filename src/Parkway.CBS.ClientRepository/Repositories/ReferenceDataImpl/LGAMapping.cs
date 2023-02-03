using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Configuration;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class LGAMapping : ILGAMapping
    {
        public List<LGACollection> GetLGACollections()
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(LGAMappingCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(LGAMappingCollection));

            LGAMappingCollection lgaMapping = new LGAMappingCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                lgaMapping = (LGAMappingCollection)serializer.Deserialize(reader);
            }
            if (lgaMapping == null)
            {
                return new List<LGACollection>();
            }
            return lgaMapping.LGACollection.ToList();
        }

        public string GetLGADatabaseId(string tenant, string LGAFileId)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(LGAMappingCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(LGAMappingCollection));

            LGAMappingCollection lgaMapping = new LGAMappingCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                lgaMapping = (LGAMappingCollection)serializer.Deserialize(reader);
            }
            return lgaMapping.LGACollection.FirstOrDefault(x => x.TenantName.Equals(tenant, StringComparison.InvariantCultureIgnoreCase)).lga.Single(x => x.LGAFileId.Equals(LGAFileId.Trim(), StringComparison.InvariantCultureIgnoreCase)).LGADatabaseId;
        }
    }
}
