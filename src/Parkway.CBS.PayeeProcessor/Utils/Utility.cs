using log4net;
using Parkway.CBS.PayeeProcessor.DAL.Model;
using Parkway.CBS.PayeeProcessor.PayeeFileProcessorCollections;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.CBS.PayeeProcessor.Utils
{
    public class Utility
    {
        static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Get Payee File Processor Configuration from *.config file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static PayeeFileProcessor GetPayeeFileProcessorConfig(string filepath)
        {

            Logger.Info($"Getting Payee File Processor Configuration for file path {filepath}");

            string filePath = filepath.Trim();
            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeFileProcessorCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(PayeeFileProcessorCollection));

            PayeeFileProcessorCollection payeefileProcessors = new PayeeFileProcessorCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (PayeeFileProcessorCollection)serializer.Deserialize(reader);
            }
            if (payeefileProcessors == null)
            {
                return new PayeeFileProcessor();
            }

            return payeefileProcessors.PayeeFileProcessor.FirstOrDefault(c => c.Path.Directorytowatch == filePath);

        }


        /// <summary>
        /// Get the Payeee File Processor Configuration using the Tenant Identifier
        /// </summary>
        /// <param name="tenantIdentifer"></param>
        /// <returns></returns>
        public static PayeeFileProcessor GetPayeeFileProcessorConfigByTenantIdentifer(string tenantIdentifer)
        {

            Logger.Info($"Getting Payee File Processor Configuration for tenant with Identifier {tenantIdentifer}");

            string tenantIdentifier = tenantIdentifer.Trim();
            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeFileProcessorCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(PayeeFileProcessorCollection));

            PayeeFileProcessorCollection payeefileProcessors = new PayeeFileProcessorCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (PayeeFileProcessorCollection)serializer.Deserialize(reader);
            }
            if (payeefileProcessors == null)
            {
                return new PayeeFileProcessor();
            }

            return payeefileProcessors.PayeeFileProcessor.FirstOrDefault(c => c.Tenant.Name == tenantIdentifer);

        }


        /// <summary>
        /// Get All Payee File Processors available from the *.config file
        /// </summary>
        /// <returns></returns>
        public static List<PayeeFileProcessor> GetPayeeFileProcessors()
        {
            Logger.Info($"Getting Payee File Processors from Config file.");

            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeFileProcessorCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(PayeeFileProcessorCollection));

            PayeeFileProcessorCollection payeefileProcessors = new PayeeFileProcessorCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (PayeeFileProcessorCollection)serializer.Deserialize(reader);
            }
            if (payeefileProcessors == null)
            {
                return new List<PayeeFileProcessor>();
            }
            return payeefileProcessors.PayeeFileProcessor.ToList();

        }


        /// <summary>
        /// Get the List of Directories to watch from the config file
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDirectorysToWatch()
        { 
            Logger.Info($"Getting list of directories to watch");
            var xmlstring = (ConfigurationManager.GetSection(typeof(PayeeFileProcessorCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(PayeeFileProcessorCollection));

            PayeeFileProcessorCollection payeefileProcessors = new PayeeFileProcessorCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (PayeeFileProcessorCollection)serializer.Deserialize(reader);
            }
            return payeefileProcessors.PayeeFileProcessor.Select(c => c.Path.Directorytowatch).ToList();
        }


        /// <summary>
        /// Get Assessment Types by identifier from the config file 
        /// </summary>
        /// <param name="identifer"></param>
        /// <returns></returns>
        public static List<AssessmentInterface> GetAssessmentTypes(string identifer)
        {
            Logger.InfoFormat("Getting Assessment Type");
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
        /// Get Assessment adapter
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <param name="identifier"></param>
        /// <returns>AssessmentInterface | null</returns>
        public static AssessmentInterface GetAssessmentType(string adapterValue, string identifier)
        {
            Logger.InfoFormat("Getting Assessment Type Adapter");
            var adapters = GetAssessmentTypes(identifier);
            return adapters.Where(adp => adp.Value == adapterValue.Trim()).FirstOrDefault();
        }

        
        public static string FormatFileFullPath(string fileName, string filefullPath)
        {
            string charsToTrim = "\\" + fileName;
            char[] array = charsToTrim.ToCharArray();

            return filefullPath.TrimEnd(array);
        }


    }
}
