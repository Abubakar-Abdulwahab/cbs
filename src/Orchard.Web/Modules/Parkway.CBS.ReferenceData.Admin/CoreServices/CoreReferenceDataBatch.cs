using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts;
using Parkway.CBS.ReferenceData.Admin.Services.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices
{
    public class CoreReferenceDataBatch : ICoreReferenceDataBatch
    {
        private readonly IReferenceDataBatchManager<ReferenceDataBatch> _referenceDataBatchService;
        private readonly IReferenceDataManager<ReferenceDataRecords> _referenceDataStagingService;
        private readonly IGeneralBatchReferenceManager<GeneralBatchReference> _generalBatchReferenceService;
        private readonly ITransactionManager _transactionManager;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        private readonly ICoreSettingsService _coreSettingsService;
        private readonly ICoreLGA _coreLGA;
        private readonly INagisDataBatchManager<NagisDataBatch> _nagisDataBatchService;


        public CoreReferenceDataBatch(IOrchardServices orchardService, IReferenceDataBatchManager<ReferenceDataBatch> referenceDataBatchService, IReferenceDataManager<ReferenceDataRecords> referenceDataStagingService, ICoreSettingsService coreSettingsService, ICoreLGA coreLGA, IGeneralBatchReferenceManager<GeneralBatchReference> generalBatchReferenceService, INagisDataBatchManager<NagisDataBatch> nagisDataBatchService)
        {
            _referenceDataBatchService = referenceDataBatchService;
            _transactionManager = orchardService.TransactionManager;
            Logger = NullLogger.Instance;
            _referenceDataStagingService = referenceDataStagingService;
            _orchardServices = orchardService;
            _coreSettingsService = coreSettingsService;
            _coreLGA = coreLGA;
            _generalBatchReferenceService = generalBatchReferenceService;
            _nagisDataBatchService = nagisDataBatchService;
        }

        public IEnumerable<ReferenceDataBatch> GetReferenceDataRecords(int skip, int take, ReferenceDataBatchSearchParams searchParams)
        {
            Logger.Information("String batchRef passed");
            var batch = _referenceDataBatchService.GetBatchRecords(skip, take, searchParams);
            if (batch == null) { throw new NoRecordFoundException("No batch record details found "); }
            return batch;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="adminUser"></param>
        /// <param name="model"></param>
        /// <returns>ValidateFileResponseVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public ValidateFileResponseVM SaveFile(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model)
        {
            switch (model.Value)
            {
                case "WithholdingTaxAdapter":
                    return SaveWithHoldingTaxOnRentBatch(file, adminUser, model);
                case "NagisAdapter":
                    return SaveNagisBatch(file, adminUser, model);
                default:
                    throw new Exception($"Unable to process file for {model.Value}");
            }
        }

        /// <summary>
        /// Save reference data batch record
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="LGAId"></param>
        /// <returns>ReferenceDataBatch</returns>
        /// <exception cref="CouldNotSaveRecord"></exception>
        private ReferenceDataBatch SaveSchedule(UserPartRecord adminUser, int LGAId, string adapterClassName, int StateId, int revenueHeadId, Int64 generalBatchReferenceId)
        {
            ReferenceDataBatch staging = new ReferenceDataBatch
            {
                AdminUser = adminUser,
                AdapterClassName = adapterClassName,
                PercentageProgress = 0,
                ProccessStage = (int)ReferenceDataProcessingStages.NotProcessed,
                LGA = new LGA { Id = LGAId },
                RevenueHead = new RevenueHead { Id = revenueHeadId },
                StateModel = new StateModel { Id = StateId },
                GeneralBatchReference = new GeneralBatchReference { Id = generalBatchReferenceId }                
            };

            if (!_referenceDataBatchService.Save(staging))
            {
                throw new CouldNotSaveRecord("Could not save ReferenceDataBatch record for LGA " + LGAId);
            }
            return staging;
        }

        /// <summary>
        /// Save Nagis data batch record
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="LGAId"></param>
        /// <returns>ReferenceDataBatch</returns>
        /// <exception cref="CouldNotSaveRecord"></exception>
        private NagisDataBatch SaveSchedule(UserPartRecord adminUser, string adapterClassName, Int64 generalBatchReferenceId, int StateId)
        {
            NagisDataBatch staging = new NagisDataBatch
            {
                AdminUser = adminUser,
                AdapterClassName = adapterClassName,
                PercentageProgress = 0,
                StateModel = new StateModel { Id = StateId },
                ProccessStage = (int)ReferenceDataProcessingStages.NotProcessed,
                GeneralBatchReference = new GeneralBatchReference { Id = generalBatchReferenceId }
            };

            if (!_nagisDataBatchService.Save(staging))
            {
                throw new CouldNotSaveRecord("Could not save NagisDataBatch record for adapter " + adapterClassName);
            }
            return staging;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="adminUser"></param>
        /// <param name="model"></param>
        /// <returns>ValidateFileResponseVM</returns>
        private ValidateFileResponseVM SaveWithHoldingTaxOnRentBatch(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model)
        {
            try
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".csv")
                {
                    return new ValidateFileResponseVM { ErrorOccurred = true, ErrorMessage = "Only csv file is allowed" };
                }

                int lgaId = 0;
                if (!int.TryParse(model.LGAId, out lgaId)) { throw new NoRecordFoundException("LGA record not found"); }

                //we need to check that the LAGId provide is a valid LGA
                TenantCBSSettings tenantStateSettings = _coreSettingsService.HasTenantStateSettings();
                if (tenantStateSettings == null) { throw new TenantNotFoundException(); }
                LGA lga = _coreLGA.GetLGA(lgaId);

                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName.Replace(" ", "");

                int withHoldingTaxonRentRevenueHeadId = 0;
                int developmentLevyRevenueHeadId = 0;

                bool parsedWithHolding = int.TryParse(ConfigurationManager.AppSettings["WithholdingTaxOnRentRevenueHeadID"], out withHoldingTaxonRentRevenueHeadId);
                if (!parsedWithHolding) { throw new Exception("Unable to convert configured WithholdingTaxOnRent Revenue head config value"); }

                bool parsedDevelopmentLevy = int.TryParse(ConfigurationManager.AppSettings["DevelopmentLevyRevenueHeadID"], out developmentLevyRevenueHeadId);
                if (!parsedDevelopmentLevy) { throw new Exception("Unable to convert configured Development Levy Revenue head config value"); }

                UploadImplInterface uploadImpl = GetUploadImplInterface(tenantStateSettings.StateName, model.Value);

                GeneralBatchReference generalBatch = SaveGeneralBatchReference(uploadImpl.BatchInvoiceResponseClassName);
                ReferenceDataBatch referenceDataBatch = SaveSchedule(adminUser, lgaId, uploadImpl.ClassName, model.StateId, withHoldingTaxonRentRevenueHeadId, generalBatch.Id);

                GeneralBatchReference developmentLevyGeneralBatch = SaveGeneralBatchReference(uploadImpl.BatchInvoiceResponseClassName);
                ReferenceDataBatch developmentLevyReferenceDataBatch = SaveSchedule(adminUser, lgaId, uploadImpl.ClassName, model.StateId, developmentLevyRevenueHeadId, developmentLevyGeneralBatch.Id);

                DirectoryInfo baseProcessing = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/ReferenceData/" + siteName + "/File");
                string fileName = referenceDataBatch.Id + "-" + developmentLevyReferenceDataBatch.Id + "-" + file.FileName;
                string path = Path.Combine(baseProcessing.FullName, fileName);

                Logger.Information(string.Format("About to save file for path {0} and LGA {1}", path, lgaId));
                file.SaveAs(path);
                return new ValidateFileResponseVM { BatchId = referenceDataBatch.Id, BatchRef = referenceDataBatch.BatchRef, RedirectToAction= "CheckBatchRecords" };
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }


        private ValidateFileResponseVM SaveNagisBatch(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model)
        {
            try
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    return new ValidateFileResponseVM { ErrorOccurred = true, ErrorMessage = "Only xls or xlsx file is allowed" };
                }

                //we need to check that the LAGId provide is a valid LGA
                TenantCBSSettings tenantStateSettings = _coreSettingsService.HasTenantStateSettings();
                if (tenantStateSettings == null) { throw new TenantNotFoundException(); }
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);

                UploadImplInterface uploadImpl = GetUploadImplInterface(siteConfig.Value, model.Value);

                GeneralBatchReference generalBatch = SaveGeneralBatchReference(uploadImpl.BatchInvoiceResponseClassName);
                NagisDataBatch nagisDataBatch = SaveSchedule(adminUser, uploadImpl.ClassName, generalBatch.Id, tenantStateSettings.StateId);

                DirectoryInfo baseProcessing = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/NagisData/" + siteConfig.Value + "/File");
                string fileName = nagisDataBatch.Id + "-" + file.FileName;
                string path = Path.Combine(baseProcessing.FullName, fileName);

                Logger.Information(string.Format("Nagis:About to save file for path {0}", path));
                file.SaveAs(path);
                return new ValidateFileResponseVM { BatchId = nagisDataBatch.Id, BatchRef = nagisDataBatch.BatchRef, RedirectToAction = "CheckNAGISBatchRecords" };
            }
            catch (NoRecordFoundException) { throw; }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }


        /// <summary>
        /// Save General Batch Reference record
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="LGAId"></param>
        /// <returns>ReferenceDataBatch</returns>
        /// <exception cref="CouldNotSaveRecord"></exception>
        private GeneralBatchReference SaveGeneralBatchReference(string adapterClassName)
        {
            GeneralBatchReference staging = new GeneralBatchReference
            {
                AdapterClassName = adapterClassName
            };

            if (!_generalBatchReferenceService.Save(staging))
            {
                throw new CouldNotSaveRecord("Could not save GeneralBatchReference for adapter class " + adapterClassName);
            }
            return staging;
        }


        public IEnumerable<ReferenceDataBatchReportStats> GetAggregateReferenceDataRecords(ReferenceDataBatchSearchParams searchParams)
        {
            var batch = _referenceDataBatchService.GetAggregateForBatchRecords(searchParams);
            if (batch == null) { throw new NoRecordFoundException("No batch record details found"); }
            return batch;
        }


        /// <summary>
        /// Get a batch details
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>string</returns>
        public string GetReferenceDataBatchRef(int Id)
        {
            return _referenceDataBatchService.GetBatchRef(Id);
        }

        public IEnumerable<AdaptersVM> GetUploadInterfaces(string stateName)
        {
            var xmlstring = GetXMLString(Util.GetAppXMLFilePath());

            XmlSerializer serializer = new XmlSerializer(typeof(FileUploadTemplates));

            FileUploadTemplates templates = new FileUploadTemplates();

            using (StringReader reader = new StringReader(xmlstring))
            {
                templates = (FileUploadTemplates)serializer.Deserialize(reader);
            }
            return templates.ListOfTemplates.FirstOrDefault(x => x.Name.Equals(stateName, StringComparison.InvariantCultureIgnoreCase)).ListOfUploadImplementations.Select(x => new AdaptersVM() { Name = x.Name, ClassName = x.ClassName, Value = x.Value });
        }

        public UploadImplInterface GetUploadImplInterface(string tenantName, string selectedImpl)
        {
            var xmlstring = GetXMLString(Util.GetAppXMLFilePath());

            XmlSerializer serializer = new XmlSerializer(typeof(FileUploadTemplates));

            FileUploadTemplates templates = new FileUploadTemplates();

            using (StringReader reader = new StringReader(xmlstring))
            {
                templates = (FileUploadTemplates)serializer.Deserialize(reader);
                if (templates == null) { throw new Exception("No template found " + tenantName); }

            }
            var uploadInterfaces = templates.ListOfTemplates.FirstOrDefault(x => x.Name.Equals(tenantName, StringComparison.InvariantCultureIgnoreCase)).ListOfUploadImplementations.Where(imple => imple.Value == selectedImpl).FirstOrDefault();

            if (uploadInterfaces == null) { throw new Exception("No uploadInterface found " + tenantName); }

            return uploadInterfaces; ;
        }


        private string GetXMLString(string xmlFilePath)
        {
            try
            {
                string xmlstring = string.Empty;

                foreach (XElement elements in XElement.Load(xmlFilePath).Elements("FileUploadTemplates"))
                {
                    xmlstring = elements.ToString();
                }
                return xmlstring;
            }
            catch (Exception) { throw new Exception("Could not read xml file"); }
        }

        public ReferenceDataBatch GetReferenceDataBatch(string batchRef)
        {
            return _referenceDataBatchService.GetReferenceDataBatch(batchRef);
        }
    }
}