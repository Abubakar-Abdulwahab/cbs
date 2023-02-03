using Orchard;
using Orchard.Localization;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.AssessmentReport;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Linq;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class FileExportHandler : BaseHandler, IFileExportHandler
    {
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;

        public readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        private readonly IInvoiceAssessmentsReportFilter _invoiceAssessmentsFilter;

        private readonly ICoreCollectionService _coreCollectionService;


        public FileExportHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IInvoiceAssessmentsReportFilter invoiceAssessmentsFilter, IRoleUserManager<AccessRoleUser> roleUserRepo, ICoreCollectionService coreCollectionService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
            _coreCollectionService = coreCollectionService;
            _invoiceAssessmentsFilter = invoiceAssessmentsFilter;
            _accessRoleUserRepo = roleUserRepo;
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            IsAuthorized<FileExportHandler>(permission);
        }


        /// <summary>
        /// Get recors for assessment report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>MDAReportViewModel</returns>
        public MDAReportViewModel GetAssessmentReport(InvoiceAssessmentSearchParams searchParams)
        {
            bool applyAccessRestrictions = _accessRoleUserRepo.UserHasAcessTypeRole(_orchardServices.WorkContext.CurrentUser.Id, AccessType.InvoiceAssessmentReport);

            searchParams.AdminUserId = _orchardServices.WorkContext.CurrentUser.Id;
            int parsedId = 0;
            searchParams.MDAId = 0;
            if (Int32.TryParse(searchParams.SMDA, out parsedId)) { searchParams.MDAId = parsedId; }
            if (Int32.TryParse(searchParams.SRevenueHeadId, out parsedId)) { searchParams.RevenueHeadId = parsedId; }
            if (Int32.TryParse(searchParams.SCategory, out parsedId)) { searchParams.Category = parsedId; }

            var details = _invoiceAssessmentsFilter.GetReport(searchParams, applyAccessRestrictions);
            if (details == null || !details.Any()) { details = new List<DetailReport> { }; }
            return new MDAReportViewModel { ReportRecords = details };
        }



        /// <summary>
        /// Get record details for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>CollectionReportViewModel</returns>
        public CollectionReportViewModel GetCollectionReport(CollectionSearchParams searchParams)
        {
            return _coreCollectionService.GetCollectionReport(searchParams);
        }
    }
}