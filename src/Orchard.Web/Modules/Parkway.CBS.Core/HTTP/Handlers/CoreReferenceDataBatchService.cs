using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreReferenceDataBatchService : CoreBaseService, ICoreReferenceDataBatchService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IReferenceDataBatchManager<ReferenceDataBatch> _referenceDataBatchRecordRepository;
        public Localizer T { get; set; }


        public CoreReferenceDataBatchService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IReferenceDataBatchManager<ReferenceDataBatch> referenceDataBatchRecordRepository) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _referenceDataBatchRecordRepository = referenceDataBatchRecordRepository;
        }

        public ReferenceDataBatch GetBatch(Int64 batchId)
        {
            return _referenceDataBatchRecordRepository.GetBatch(batchId);
        }

        public ReferenceDataBatch GetBatchDetails(Int64 generalBatchId)
        {
            return _referenceDataBatchRecordRepository.GetBatchDetails(generalBatchId);
        }
    }
}