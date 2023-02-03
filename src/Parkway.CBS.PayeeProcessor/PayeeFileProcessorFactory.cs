using log4net;
using Parkway.CBS.PayeeProcessor.PayeeProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.PayeeProcessor.Contract
{
    public class PayeeFileProcessorFactory
    {
        Dictionary<string, Type> payeeFileProcessors;
        static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PayeeFileProcessorFactory()
        {
            LoadAllPayeeFileProcessors();
        }


        /// <summary>
        /// Creates an instance of the Payee File Processor 
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns>IBasePayeeFileProcessor</returns>
        public IBasePayeeFileProcessor CreateInstance(string instanceType)
        {

            try
            {
                Type t = GetPayeeFileProcessorTypeToCreate(instanceType);
                if (t == null)
                {
                    return new DefaultPayeeFileProcessor();
                }
                Logger.Info($"Creating payee File processor instance for type {t}");
                return Activator.CreateInstance(t) as IBasePayeeFileProcessor;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured creating Payee File Processor Instance See details", ex);
                throw;
            }
           
            
        }
         
         
        Type GetPayeeFileProcessorTypeToCreate(string instanceType)
        {
            Logger.Info($"Getting Payee File Processor for type {instanceType}");

            foreach(var payeeFileProcessor in payeeFileProcessors)
            {
                if (payeeFileProcessor.Key.Contains(instanceType))
                {
                    return payeeFileProcessors[payeeFileProcessor.Key];
                }
            }
            return null;
        }

        /// <summary>
        /// Using Reflection to scan the assembly and get all possible implementation PayeeProcessor
        /// </summary>
        void LoadAllPayeeFileProcessors()
        {
            Logger.Info($"Loading all payee file Processor Implementations ");

            payeeFileProcessors = new Dictionary<string, Type>();

            Type[] payeeFileProcessorTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach(Type fileprocessortype in payeeFileProcessorTypes)
            {
                if(fileprocessortype.GetInterface(typeof(IBasePayeeFileProcessor).ToString()) != null)
                {
                    Logger.Info($"Loaded {fileprocessortype.Name.ToString()} Implementation ");
                    payeeFileProcessors.Add(fileprocessortype.Name.ToString(), fileprocessortype);
                }
            }

        }
    }
}
