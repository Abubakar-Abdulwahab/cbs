using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload;
using Parkway.CBS.ReferenceData.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Admin.Controllers.Handlers
{
    public class DataEnumerationHandler : IDataEnumerationHandler
    {
        public Localizer T { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreReferenceDataBatch _coreReferenceDataBatch;
        private readonly ICoreNAGISDataBatch _coreNAGISDataBatch;
        private readonly ICoreLGA _coreLGA;
        private readonly ICoreNagisInvoiceSummary _coreNagisInvoiceSummary;
        public ILogger Logger { get; set; }
        private readonly ICoreSettingsService _coreSettingsService;
        private readonly IAuthorizer _authorizer;


        public DataEnumerationHandler(IOrchardServices orchardServices, ICoreReferenceDataBatch coreReferenceDataBatch, ICoreLGA coreLGA, ICoreSettingsService coreSettingsService, ICoreNAGISDataBatch coreNAGISDataBatch, ICoreNagisInvoiceSummary coreNagisInvoiceSummary)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _coreReferenceDataBatch = coreReferenceDataBatch;
            _coreLGA = coreLGA;
            _coreSettingsService = coreSettingsService;
            _coreNAGISDataBatch = coreNAGISDataBatch;
            _coreNagisInvoiceSummary = coreNagisInvoiceSummary;
            _authorizer = _orchardServices.Authorizer;
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Permission permission)
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
        }


        /// <summary>
        /// Upload the enumeration data file to the specified folder
        /// </summary>
        /// <param name="file"></param>
        /// <param name="adminUser"></param>
        /// <returns>ValidateFileResponseVM</returns>
        public ValidateFileResponseVM ProcessEnumerationDataFile(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model)
        {
            try
            {
                return _coreReferenceDataBatch.SaveFile(file, adminUser, model);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw exception;
            }
        }


        public ReferenceDataBatchVM GetCollectionReport(int skip, int take, ReferenceDataBatchSearchParams searchParams)
        {
            try
            {
                ReferenceDataBatchVM returnModel = new ReferenceDataBatchVM();

                returnModel.ReportRecords = _coreReferenceDataBatch.GetReferenceDataRecords(skip, take, searchParams).Select(x =>
                new ReferenceDataBatchCollectionDetails()
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedAtUtc,
                    BatchRef = x.BatchRef,
                    ProccessStage = Util.GetReferenceDataProcessingStatus(x.ProccessStage),
                    NumberOfRecords = x.NumberOfRecords,
                    LGAName = x.LGA.Name,
                    Status = x.GetProcessStage()
                });

                var result = _coreReferenceDataBatch.GetAggregateReferenceDataRecords(searchParams).First();
                returnModel.TotalNumberOfRecords = result.RecordCounts;

                return returnModel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

       
        public ValidateFileModel GetLGAs(int stateId)
        {
            try
            {
                ValidateFileModel returnModel = new ValidateFileModel();

                returnModel.LGAList = _coreLGA.GetLGAs(stateId).Select(x =>
                new ReferenceDataLGAs()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                return returnModel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Get the list of LGAs for this tenant
        /// </summary>
        /// <returns>ValidateFileModel</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        public ValidateFileModel GetLGAsAndAdapters()
        {
            TenantCBSSettings tenantStateSettings = _coreSettingsService.HasTenantStateSettings();
            if (tenantStateSettings == null) { throw new TenantNotFoundException(); }

            ValidateFileModel returnModel = new ValidateFileModel();
            returnModel.LGAList = _coreLGA.GetLGAs(tenantStateSettings.StateId).Select(x =>
                new ReferenceDataLGAs()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            returnModel.Adapters = _coreReferenceDataBatch.GetUploadInterfaces(siteConfig.Value);
            returnModel.StateId = tenantStateSettings.StateId;
            return returnModel;
        }

        public string GetReferenceDataBatchRef(int Id)
        {
            try
            {
                return _coreReferenceDataBatch.GetReferenceDataBatchRef(Id);
            }
            catch (NoRecordFoundException exception)
            {
                throw exception;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReferenceDataBatch GetReferenceDataBatch(string batchRef)
        {
            try
            {
                return _coreReferenceDataBatch.GetReferenceDataBatch(batchRef);
            }
            catch (NoRecordFoundException exception)
            {
                throw exception;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetNAGISDataBatchRef(int Id)
        {
            try
            {
                return _coreNAGISDataBatch.GetNAGISDataBatchRef(Id);
            }
            catch (NoRecordFoundException exception)
            {
                throw exception;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public NAGISDataBatchVM GetCollectionReport()
        {
            try
            {
                NAGISDataBatchVM returnModel = new NAGISDataBatchVM();

                returnModel.ReportRecords = _coreNAGISDataBatch.GetNAGISDataRecords().Select(x =>
                new NAGISDataBatchCollectionDetails()
                {
                    Id = x.Id,
                    CreatedDate = x.CreatedAtUtc,
                    BatchRef = x.BatchRef,
                    ProccessStage = Util.GetNAGISDataProcessingStatus(x.ProccessStage),
                    NumberOfRecordSentToCashFlow = x.NumberOfRecordSentToCashFlow,
                    Status = x.GetProcessStage()
                });

                return returnModel;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public NAGISInvoiceSummaryVM GetNAGISInvoiceSummaryCollection(long nagisDataBatchId)
        {
            return _coreNagisInvoiceSummary.GetInvoiceSummaries(nagisDataBatchId);
        }
    }
}