using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreHelperService : CoreBaseService, ICoreHelperService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMimeTypeProvider _mimeTypeProvider;

        public CoreHelperService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _mediaLibraryService = mediaManagerService;
            _mimeTypeProvider = mimeTypeProvider;
            Logger = NullLogger.Instance;
        }       

    }
}