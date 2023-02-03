using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using System.Globalization;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreReportingService : CoreBaseService, ICoreReportingService
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly ITransactionLogManager<TransactionLog> _transactionLogManager;

        public CoreReportingService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, ITransactionLogManager<TransactionLog> transactionLogManager) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _transactionLogManager = transactionLogManager;
        }

    }
}