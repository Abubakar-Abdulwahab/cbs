using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSEscortRequestDetailsHandler : IPSSEscortRequestDetailsHandler
    {
        private readonly ICoreEscortService _coreEscortService;
        ILogger Logger { get; set; }

        public PSSEscortRequestDetailsHandler(ICoreEscortService coreEscortService)
        {
            _coreEscortService = coreEscortService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Create dispatch note file
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDispatchNoteByteFile(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                if (_coreEscortService.CheckIfApprovedEscortRequestExists(fileRefNumber))
                {
                    return _coreEscortService.CreateDispatchNote(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved for PSS Escort request. File Ref Number " + fileRefNumber); }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}