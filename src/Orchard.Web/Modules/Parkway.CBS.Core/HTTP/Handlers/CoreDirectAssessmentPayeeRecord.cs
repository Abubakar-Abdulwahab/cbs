using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Payee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreDirectAssessmentPayeeRecord : CoreBaseService, ICoreDirectAssessmentPayeeRecord
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> _directAssessmentPayeeRepository;
        public Localizer T { get; set; }


        public CoreDirectAssessmentPayeeRecord(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> directAssessmentPayeeRepository) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _directAssessmentPayeeRepository = directAssessmentPayeeRepository;
        }


        /// <summary>
        /// Save payee records
        /// </summary>
        /// <param name="payees"></param>
        /// <param name="recordId"></param>
        /// <param name="entity"></param>
        /// <exception cref="Exception"></exception>
        public void SaveRecords(List<PayeeAssessmentLineRecordModel> payees, long recordId, TaxEntity entity)
        { _directAssessmentPayeeRepository.SaveRecords(payees, recordId, entity); }

    }
}