using Orchard;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class ChangePassportPhotoAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        private readonly IChangePassportHandler _changePassportPhotoHandler;
        private readonly IRequestListHandler _policeRequestHandler;


        public ChangePassportPhotoAJAXController(IOrchardServices orchardServices, IChangePassportHandler changePassportPhotoHandler, IRequestListHandler policeRequestHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Logger = NullLogger.Instance;
            _changePassportPhotoHandler = changePassportPhotoHandler;
            _policeRequestHandler = policeRequestHandler;
        }


        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the specified category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        public JsonResult GetFileNumberDetailsForPCC(string fileNumber)
        {
            Logger.Information("Getting details for file number " + fileNumber);
            return Json(_changePassportPhotoHandler.GetFileNumberDetails(fileNumber));
        }
    }
}