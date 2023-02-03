using Newtonsoft.Json;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreThirdPartyPaymentNotificationService : CoreBaseService, ICoreThirdPartyPaymentNotificationService
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IThirdPartyPaymentNotificationManager<ThirdPartyPaymentNotification> _thirdPartyPaymentNotificationManager;

        public CoreThirdPartyPaymentNotificationService(IOrchardServices orchardServices,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, IMediaLibraryService mediaLibraryService, IThirdPartyPaymentNotificationManager<ThirdPartyPaymentNotification> thirdPartyPaymentNotificationManager) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _mediaLibraryService = mediaLibraryService;
            _thirdPartyPaymentNotificationManager = thirdPartyPaymentNotificationManager;
        }


        /// <summary>
        /// Persist payment notification for task to notify a third party system
        /// </summary>
        /// <param name="model"></param>
        public void SaveNotification(ThirdPartyPaymentNotification model)
        {
            string smodel = JsonConvert.SerializeObject(model);
            if (!_thirdPartyPaymentNotificationManager.Save(model))
            {
                Logger.Error("COULD NOT SAVE PAYMENT NOTIFICATION TO THIRD PARTY SYSTEM Model: " + smodel);
                throw new CannotSaveRecordException("COULD NOT SAVE PAYMENT NOTIFICATION TO THIRD PARTY SYSTEM Model: " + smodel);
            }
        }
    }
}