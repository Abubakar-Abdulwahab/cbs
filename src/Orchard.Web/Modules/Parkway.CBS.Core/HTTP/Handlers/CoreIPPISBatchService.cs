using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreIPPISBatchService : CoreBaseService, ICoreIPPISBatchService
    {
        private readonly IIPPISBatchManager<IPPISBatch> _repository;

        public CoreIPPISBatchService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IIPPISBatchManager<IPPISBatch> repository) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            Logger = NullLogger.Instance;
            _repository = repository;
        }       


        /// <summary>
        /// Save IPPIS batch 
        /// </summary>
        /// <param name="iPPISBatch">IPPISBatch</param>
        public void SaveProcessBatch(IPPISBatch batch)
        {
            if (!_repository.Save(new IPPISBatch { ErrorOccurred = batch.ErrorOccurred, ErrorMessage = batch.ErrorMessage, FilePath = batch.FilePath, Month = batch.Month, Year = batch.Year }))
            {
                Logger.Error(string.Format("Could not save batch for IPPIS file {0} ", batch.FilePath));
                throw new Exception(string.Format("Could not save batch for IPPIS file {0} ", batch.FilePath));
            }
        }
    }
}