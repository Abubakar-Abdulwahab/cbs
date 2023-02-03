using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class ValidateDocumentHandler : IValidateDocumentHandler
    {
        
        private readonly IPSSRequestManager<PSSRequest> _pssRequestRepo;
        private readonly IOrchardServices _orchardServices;
        private readonly IEnumerable<Lazy<IPSSServiceTypeDetails>> _serviceTypeDetailsImpl;

        public ValidateDocumentHandler(IPSSRequestManager<PSSRequest> pssRequestRepo, IEnumerable<Lazy<IPSSServiceTypeDetails>> serviceTypeDetailsImpl, IOrchardServices orchardServices)
        {
            _pssRequestRepo = pssRequestRepo;
            _serviceTypeDetailsImpl = serviceTypeDetailsImpl;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Validate request approval number format
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>approval number string if successful, null otherwise</returns>
        public string ValidateDocumentApprovalNumber(string approvalNumber)
        {
            try
            {
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.ApprovalNumberRegexPattern.ToString())?.FirstOrDefault();
                if (node == null || string.IsNullOrEmpty(node.Value)) { throw new Exception("Unable to fetch ApprovalNumberRegexPattern from StateConfig in ValidateDocumentHandler"); }
                string approvalNumberPattern = node.Value;
                string numberPattern = @"\B(\d+)\b";
                var matches = Regex.Matches(approvalNumber, approvalNumberPattern);
                if (matches.Count == 1)
                {
                    foreach (Match match in matches)
                    {
                        var pssApprovalNumber = Regex.Match(match.Value, numberPattern);
                        if (pssApprovalNumber != null)
                        {
                            long extractNum = 0;
                            if (long.TryParse(pssApprovalNumber.Value, out extractNum))
                            {
                                approvalNumber = match.Value.Trim();
                                return approvalNumber;
                            }
                            else { return null; }
                        }
                        else { return null; }
                    }
                }
                else { return null; }
            }
            catch(Exception)
            {
                throw;
            }
            return null;
        }


        /// <summary>
        /// Get request document info with specified approval number
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public ValidatedDocumentVM GetDocumentInfoWithApprovalNumber(string approvalNumber, long taxEntityId)
        {
            try
            {
                ValidatedDocumentVM documentInfo = _pssRequestRepo.GetRequestInfoWithApprovalNumber(approvalNumber, taxEntityId);
                if(documentInfo == null) { return documentInfo; }
                foreach(var impl in _serviceTypeDetailsImpl)
                {
                    if(impl.Value.GetServiceTypeDefinition == (PSSServiceTypeDefinition)documentInfo.ServiceType)
                    {
                        documentInfo.DocumentInfo = impl.Value.GetRequestInfo(documentInfo.RequestId);
                        return documentInfo;
                    }
                }
                throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id " + documentInfo.ServiceType);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get request document info with specified approval number
        /// </summary>
        /// <param name="approvalNumber"></param>
        /// <returns>ValidatedDocumentVM</returns>
        public ValidatedDocumentVM GetDocumentInfoWithApprovalNumber(string approvalNumber)
        {
            try
            {
                ValidatedDocumentVM documentInfo = _pssRequestRepo.GetRequestInfoWithApprovalNumber(approvalNumber);
                if (documentInfo == null) { return documentInfo; }
                foreach (var impl in _serviceTypeDetailsImpl)
                {
                    if (impl.Value.GetServiceTypeDefinition == (PSSServiceTypeDefinition)documentInfo.ServiceType)
                    {
                        documentInfo.DocumentInfo = impl.Value.GetRequestInfo(documentInfo.RequestId);
                        return documentInfo;
                    }
                }
                throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id " + documentInfo.ServiceType);
            }
            catch (Exception) { throw; }
        }

    }
}