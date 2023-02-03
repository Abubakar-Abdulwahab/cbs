using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSExtractRequestDetailsHandler : IPSSExtractRequestDetailsHandler
    {
        private readonly ICoreExtractService _coreExtractService;
        ILogger Logger { get; set; }

        public PSSExtractRequestDetailsHandler(ICoreExtractService coreExtractService)
        {
            _coreExtractService = coreExtractService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Generate extract document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateExtractDocumentByteFile(string fileRefNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                if (_coreExtractService.CheckIfApprovedExtractRequestExists(fileRefNumber))
                {
                    return _coreExtractService.CreateExtractDocument(fileRefNumber, false);
                }
                else { throw new NoRecordFoundException("404 or request not yet approved for PSS Extract request. File Ref Number " + fileRefNumber); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}