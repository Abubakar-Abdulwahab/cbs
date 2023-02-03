using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class RequestListHandler : IRequestListHandler
    {
        private readonly IPoliceRequestFilter _requestFilter;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> _escortDetailsManager;
        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractDetailsManager;
        private readonly Lazy<IInvestigationReportDetailsManager<InvestigationReportDetails>> _investigationReportManager;
        private readonly Lazy<IRequestStatusLogManager<RequestStatusLog>> _requestStatusLogManager;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        private readonly Lazy<IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue>> _formControlRevenueHeadValueManager;
        private readonly Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> _flowDefinitionLevelManager;
        private readonly IEnumerable<Lazy<IPSSServiceTypeDetails>> _serviceTypeDetailsImpl;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _taxEntityProfileLocationManager;

        public ILogger Logger { get; set; }

        public RequestListHandler(IPoliceRequestFilter requestFilter, Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IExtractDetailsManager<ExtractDetails>> extractDetailsManager, Lazy<IPSSEscortDetailsManager<PSSEscortDetails>> escortDetailsManager, Lazy<IInvestigationReportDetailsManager<InvestigationReportDetails>> investigationReportManager, Lazy<IRequestStatusLogManager<RequestStatusLog>> requestStatusLogManager, IEnumerable<Lazy<IPSSServiceTypeDetails>> serviceTypeDetailsImpl, Lazy<IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue>> formControlRevenueHeadValueManage, Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel>> flowDefinitionLevelManager, ICoreStateAndLGA coreStateLGAService, ICoreCharacterCertificateService coreCharacterCertificateService, ITaxEntityProfileLocationManager<TaxEntityProfileLocation> taxEntityProfileLocationManager)
        {
            _requestFilter = requestFilter;
            _requestManager = requestManager;
            _extractDetailsManager = extractDetailsManager;
            _escortDetailsManager = escortDetailsManager;
            _serviceTypeDetailsImpl = serviceTypeDetailsImpl;
            _investigationReportManager = investigationReportManager;
            _requestStatusLogManager = requestStatusLogManager;
            _serviceRequestManager = serviceRequestManager;
            _formControlRevenueHeadValueManager = formControlRevenueHeadValueManage;
            _flowDefinitionLevelManager = flowDefinitionLevelManager;
            _coreStateLGAService = coreStateLGAService;
            Logger = NullLogger.Instance;
            _coreCharacterCertificateService = coreCharacterCertificateService;
            _taxEntityProfileLocationManager = taxEntityProfileLocationManager;
        }

        public RequestListVM GetRequestListVM(RequestsReportSearchParams searchParams)
        {
            try
            {
                dynamic records = _requestFilter.GetRequestReportViewModel(searchParams, false, false);

                var dataSize = ((IEnumerable<ReportStatsVM>)records.TotalRecordCount).First().TotalRecordCount;

                double pageSize = ((double)dataSize / (double)searchParams.Take);
                int pages = 0;

                if (pageSize < 1 && dataSize >= 1) { pages = 1; }
                else { pages = (int)Math.Ceiling(pageSize); }

                return new RequestListVM
                {
                    Requests = ((IEnumerable<PSSRequestVM>)records.ReportRecords),
                    DataSize = pages,
                    TaxEntityId = searchParams.TaxEntityId
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public RequestListVM GetRequestBranchListVM(RequestsReportSearchParams searchParams)
        {
            try
            {
                dynamic records = _requestFilter.GetRequestBranchReportViewModel(searchParams);

                var dataSize = ((IEnumerable<ReportStatsVM>)records.TotalRecordCount).First().TotalRecordCount;

                int pages = Util.Pages(searchParams.Take, dataSize);

                return new RequestListVM
                {
                    Requests = (IEnumerable<PSSRequestVM>)records.ReportRecords,
                    StateLGAs = _coreStateLGAService.GetStateVMs(),
                    ListLGAs = searchParams.State != 0 ? _coreStateLGAService.GetLgas(searchParams.State) : null,
                    DataSize = pages,
                    TaxEntityId = searchParams.TaxEntityId,
                    Branches = _taxEntityProfileLocationManager.GetTaxEntityLocations(searchParams.TaxEntityId)
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public RequestDetailsVM GetRequestViewDetails(string fileRefNumber, long taxEntityId)
        {
            int serviceTypeId = _requestManager.Value.GetServiceType(fileRefNumber);
            foreach (var impl in _serviceTypeDetailsImpl)
            {
                if ((PSSServiceTypeDefinition)serviceTypeId == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetRequestDetails(fileRefNumber, taxEntityId);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + serviceTypeId);
        }

        public RequestDetailsVM GetRequestBranchViewDetails(string fileRefNumber)
        {
            PSSRequestVM requestDetails = _requestManager.Value.GetRequestDetails(fileRefNumber);

            foreach (var impl in _serviceTypeDetailsImpl)
            {
                if ((PSSServiceTypeDefinition)requestDetails.ServiceTypeId == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetRequestDetails(fileRefNumber, requestDetails.TaxEntityId);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + requestDetails.ServiceTypeId);
        }

        public PSSRequestInvoiceVM GetInvoicesForRequest(string fileNumber)
        {
            try
            {
                return new PSSRequestInvoiceVM { Invoices = _requestManager.Value.GetRequestInvoices(fileNumber) };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generate service specific document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateServiceDocumentByteFile(string fileRefNumber, long taxEntityId)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                PSSRequestVM requestDetails = _requestManager.Value.GetRequestDetails(fileRefNumber);

                foreach (var impl in _serviceTypeDetailsImpl)
                {
                    if ((PSSServiceTypeDefinition)requestDetails.ServiceTypeId == impl.Value.GetServiceTypeDefinition)
                    {
                        return impl.Value.CreateCertificate(fileRefNumber, taxEntityId);
                    }
                }
                throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + requestDetails.ServiceTypeId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Generate rejection character certificate
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateRejectionCharacterCertificateByteFile(string fileRefNumber, long taxEntityId)
        {
            try
            {
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                return _coreCharacterCertificateService.CreateRejectionCertificateDocument(fileRefNumber);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}
