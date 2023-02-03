using Parkway.CBS.ClientFileServices.Implementations.Contracts;
using Parkway.CBS.ClientFileServices.Implementations.IPPIS;
using Parkway.CBS.ClientFileServices.Logger.Contracts;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientFileServices.Implementations
{
    public class FileProcessor : IFileProcessor
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IReferenceDataBatchDAOManager BatchDAO { get; set; }

        protected void SetBatchDAO()
        { if (BatchDAO == null) { BatchDAO = new ReferenceDataBatchDAOManager(UoW); } }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "ClientFileServices");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="filePath"></param>
        /// <param name="batchId"></param>
        public void ProcessFile(string tenantName, string filePath, long batchId)
        {
            log.Info($"About to start file processing {filePath}");

            try
            {
                //set unit of work
                SetUnitofWork(tenantName);

                //instantiate the ReferenceDataBatch repository
                SetBatchDAO();
                ReferenceDataBatch batchRecord = BatchDAO.GetBatchRecord(batchId);

                var nameSplit = batchRecord.AdapterClassName.Split(',');
                var implementingClass = Activator.CreateInstance(nameSplit[1].Trim(), nameSplit[0].Trim());
                var impl = ((IReferenceDataFileProcessor)implementingClass.Unwrap());
                impl.SaveFile(tenantName, filePath, batchRecord.Id);
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Error Processing for stage {1}. FilePath {0}", filePath, ReferenceDataProcessingStages.NotProcessed));
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                BatchDAO = null;
            }

        }
    }
}
