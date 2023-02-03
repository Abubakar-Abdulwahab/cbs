using Orchard;
using Orchard.Localization;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System.Collections.Generic;
using Parkway.CBS.Core.DataFilters.MDAReport.Order;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Orchard.Modules.Services;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.DataFilters.CollectionReport;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.CBS.Core.DataFilters.AssessmentReport;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class AJAXHandler : BaseHandler, IAJAXHandler
    {
        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        private readonly IRevenueHeadHandler _revenueHeadHandler;
        private readonly IMDAHandler _mdaHandler;
        private readonly IInvoiceManager<Invoice> _invoiceHandler;

        private readonly IRoleRevenueHeadManager<AccessRoleMDARevenueHead> _accessRoleMDARevenueHeadRepo;
        private readonly IRoleManager<AccessRole> _accessRoleRepo;
        private readonly IRoleUserManager<AccessRoleUser> _accessRoleUserRepo;

        private readonly ITaxEntityManager<TaxEntity> _customerRepository;
        private readonly IEnumerable<IMDAReportOrder> _reportOrder;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _sectorRepository;
        private readonly IAdminSettingManager<ExpertSystemSettings> _tenantRepository;
        public readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        private readonly ITaxPayerReportFilter _taxpayerReportFilter;

        private readonly ICollectionReportFilter _collectionReportFilter;
        private readonly IInvoiceAssessmentsReportFilter _invoiceAssessmentsFilter;


        public AJAXHandler(IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> customerRepository,
            IEnumerable<IMDAReportOrder> reportOrder, IMDAHandler mdaHandler,
            IInvoiceManager<Invoice> invoiceHandler, IRevenueHeadHandler revenueHeadHandler, ITaxEntityCategoryManager<TaxEntityCategory> sectorRepository, IAdminSettingManager<ExpertSystemSettings> tenantRepository, IModuleService moduleService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ICollectionReportFilter collectionreportFilter, ITaxPayerReportFilter taxpayerReportFilter, IInvoiceAssessmentsReportFilter invoiceAssessmentsFilter, IRoleRevenueHeadManager<AccessRoleMDARevenueHead> roleRevenueHeadRepo, IRoleManager<AccessRole> roleRepo, IRoleUserManager<AccessRoleUser> roleUserRepo) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _customerRepository = customerRepository;
            _reportOrder = reportOrder;
            _mdaHandler = mdaHandler;
            _invoiceHandler = invoiceHandler;
            _revenueHeadHandler = revenueHeadHandler;
            _sectorRepository = sectorRepository;
            _tenantRepository = tenantRepository;
            _settingsRepository = settingsRepository;
            _collectionReportFilter = collectionreportFilter;
            _taxpayerReportFilter = taxpayerReportFilter;
            _invoiceAssessmentsFilter = invoiceAssessmentsFilter;
            _accessRoleMDARevenueHeadRepo = roleRevenueHeadRepo;
            _accessRoleRepo = roleRepo;
            _accessRoleUserRepo = roleUserRepo;
        }
    }
}