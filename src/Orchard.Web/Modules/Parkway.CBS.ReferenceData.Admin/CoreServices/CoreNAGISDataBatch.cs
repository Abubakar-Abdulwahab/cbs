using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices
{
    public class CoreNAGISDataBatch : ICoreNAGISDataBatch
    {
        private readonly INagisDataBatchManager<NagisDataBatch> _nagisDataBatchService;
        public ILogger Logger { get; set; }

        public CoreNAGISDataBatch(INagisDataBatchManager<NagisDataBatch> nagisDataBatchService)
        {
            _nagisDataBatchService = nagisDataBatchService;
            Logger = NullLogger.Instance;
        }

        public IEnumerable<NagisDataBatch> GetNAGISDataRecords()
        {
            Logger.Information("String batchRef passed");
            var batch = _nagisDataBatchService.GetBatchRecords();
            if (batch == null) { throw new NoRecordFoundException("No batch record details found"); }
            return batch;
        }

        public string GetNAGISDataBatchRef(int Id)
        {
            return _nagisDataBatchService.GetBatchRef(Id);
        }
    }
}