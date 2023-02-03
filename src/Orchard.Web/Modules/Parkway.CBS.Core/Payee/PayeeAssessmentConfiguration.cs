using Orchard;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Parkway.CBS.Core.Payee
{
    public class PayeeAssessmentConfiguration : IPayeeAssessmentConfiguration
    {
        private readonly IDirectAssessmentPayee _directAssessmentPayee;
        public PayeeAssessmentConfiguration()
        {
            _directAssessmentPayee = new DirectAssessmentPayee();
        }


        /// <summary>
        /// Get Assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <param name="identifier"></param>
        /// <returns>AssessmentInterface | null</returns>
        public AssessmentInterface GetAssessmentType(string adapterValue, string identifier)
        {
            var adapters = GetAssessmentTypes(identifier);
            return adapters.Where(adp => adp.Value == adapterValue.Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Get AssessmentInterface collection
        /// <para>Send the state name as it is, truncation and other string operation would be done here. No special characters are allowed</para>
        /// </summary>
        /// <returns>List{AssessmentInterface}</returns>
        public List<AssessmentInterface> GetAssessmentTypes(string identifer)
        {
            identifer = identifer.Trim().Split(' ')[0];

            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeAssessmentCollection).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(PayeeAssessmentCollection));
            PayeeAssessmentCollection tenantAssessmentDataCollection = new PayeeAssessmentCollection();
            using (StringReader reader = new StringReader(xmlstring))
            {
                tenantAssessmentDataCollection = (PayeeAssessmentCollection)serializer.Deserialize(reader);
            }
            if (tenantAssessmentDataCollection == null) { return new List<AssessmentInterface>(); }
            AssessmentInterfaceItem tenantAssessmentSection = tenantAssessmentDataCollection.AssessmentInterfaceItem.Where(item => item.Name == identifer + "_AssessmentCollection").FirstOrDefault();
            if (tenantAssessmentSection == null) { return new List<AssessmentInterface>(); }
            return tenantAssessmentSection.AssessmentInterface.Where(impl => impl.IsActive).Select(impl => impl).ToList();
        }


        /// <summary>
        /// Get implementation for this adapter interface
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter)
        {
            return _directAssessmentPayee.GetAdapter(adapter.ClassName);
        }
    }

    public interface IPayeeAssessmentConfiguration : IDependency
    {

        /// <summary>
        /// Get AssessmentInterface collection
        /// <para>Send the state name as it is, truncation and other string operation would be done here. No special characters are allowed</para>
        /// </summary>
        /// <returns>List{AssessmentInterface}</returns>
        List<AssessmentInterface> GetAssessmentTypes(string identifer);


        /// <summary>
        /// Get Assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <param name="identifier"></param>
        /// <returns>AssessmentInterface | null</returns>
        AssessmentInterface GetAssessmentType(string adapterValue, string identifier);


        /// <summary>
        /// Get implementation for this adapter interface
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter);
    }
}