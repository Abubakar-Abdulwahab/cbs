using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.PSSServiceType.ServiceOptions.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System.IO;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PCCDiasporaHandler : IPCCDiasporaHandler
    {

        private readonly ICoreServiceStateCommand _coreServiceStateCommand;
        private readonly ICoreCharacterCertificateReasonForInquiry _coreCharacterCertificateReasonForInquiryService;
        private readonly ICoreCountryService _coreCountryService;
        private readonly ICoreServiceOptions _coreServiceOptions;
        private readonly IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> _requestTypeManager;
        private readonly IPSServiceCaveatManager<PSServiceCaveat> _caveatRepo;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreCharacterCertificateInputValidation _coreCharacterCertificateInputValidation;
        private readonly ICoreCommand _coreCommand;
        private readonly ICoreDiasporaCharacterCertificateInputValidation _coreDiasporaInputValidation;
        private readonly IEnumerable<Lazy<IServiceOptionPresentation>> _optionPresentationImpl;

        private readonly ICoreIdentificationType _coreIdentityType;

        public ILogger Logger { get; set; }


        public PCCDiasporaHandler(ICoreCharacterCertificateReasonForInquiry coreCharacterCertificateReasonForInquiryService, ICoreCountryService coreCountryService, IPSSCharacterCertificateRequestTypeManager<PSSCharacterCertificateRequestType> requestTypeManager, IPSServiceCaveatManager<PSServiceCaveat> caveatRepo, IOrchardServices orchardServices, ICoreServiceStateCommand coreServiceStateCommand, ICoreServiceOptions coreServiceOptions, IEnumerable<Lazy<IServiceOptionPresentation>> optionPresentationImpl, ICoreCharacterCertificateInputValidation coreCharacterCertificateInputValidation, ICoreDiasporaCharacterCertificateInputValidation coreDiasporaInputValidation, ICoreIdentificationType coreIdentityType, ICoreCommand coreCommand)
        {
            _coreCharacterCertificateReasonForInquiryService = coreCharacterCertificateReasonForInquiryService;
            _coreCountryService = coreCountryService;
            Logger = NullLogger.Instance;
            _requestTypeManager = requestTypeManager;
            _caveatRepo = caveatRepo;
            _orchardServices = orchardServices;
            _coreServiceStateCommand = coreServiceStateCommand;
            _coreServiceOptions = coreServiceOptions;
            _optionPresentationImpl = optionPresentationImpl;
            _coreCharacterCertificateInputValidation = coreCharacterCertificateInputValidation;
            _coreDiasporaInputValidation = coreDiasporaInputValidation;
            _coreIdentityType = coreIdentityType;
            _coreCommand = coreCommand;
        }


        /// <summary>
        /// Get character certificate request VM
        /// for diaspora
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>PCCDiasporaUserInputVM</returns>
        public PCCDiasporaUserInputVM GetVMForCharacterCertificate(int serviceId, long taxEntityId, int taxCategoryId)
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            PSServiceCaveatVM caveat = ObjectCacheProvider.GetCachedObject<PSServiceCaveatVM>(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}");

            if (caveat == null)
            {
                caveat = _caveatRepo.GetServiceCaveat(serviceId);

                if (caveat != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.Caveat)}-{serviceId}", caveat);
                }
            }

            PCCDiasporaUserInputVM viewmodel = new PCCDiasporaUserInputVM
            {
                HeaderObj = new HeaderObj { },
                CharacterCertificateReasonsForInquiry = _coreCharacterCertificateReasonForInquiryService.GetReasonsForInquiry(),
                Countries = _coreCountryService.GetCountries(),
                Caveat = caveat,
            };

            //here we need to check that this user's category has biometric support
            //get the tax entity and category
            if (!_coreIdentityType.CheckIfTaxEntityIdentityTypeHasBiometricSupport(taxEntityId))
            {
                //if the user has not been registered with an identity type 
                //that doesn't have biometric support
                viewmodel.IdentityTypeList = _coreIdentityType.GetIdentityWithBiometricSupport(taxCategoryId).ToList();
            }

            return viewmodel;
        }



        public void ValidateCharacterCertificateRequest(PCCDiasporaUserInputVM userInput, ref List<ErrorModel> errors)
        {
            _coreCharacterCertificateInputValidation.ValidateReasonForInquiry(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidatePlaceOfBirth(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidateDateOfBirth(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidateCountryOfResidence(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidateCountryOfPassport(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidatePassportNumber(userInput, errors);
            _coreCharacterCertificateInputValidation.ValidateDateOfPassportIssurance(userInput, errors);
        }


        
        public void ValidateIdentity(PCCDiasporaUserInputVM userInput, ref List<ErrorModel> errors, long taxEntityId, int taxCategoryId)
        {
            _coreDiasporaInputValidation.ValideateIdentityDetails(userInput, errors, taxEntityId, taxCategoryId);
        }


        /// <summary>
        /// Get the CPCCR command that would treat
        /// the requests for diaspora 
        /// </summary>
        /// <returns>CommandVM</returns>
        public CommandVM GetCPCCRCommand()
        {
            return _coreCommand.CPCCRCommand();            
        }



        public void ValidateCharacterCertificateFileInput(PCCDiasporaUserInputVM userInput, ICollection<UploadedFileAndName> uploads, ref List<ErrorModel> errors)
        {
            foreach (var upload in uploads)
            {
                if (upload.Upload == null || upload.Upload.ContentLength == 0)
                {
                    errors.Add(new ErrorModel { FieldName = upload.UploadName, ErrorMessage = $"{Util.InsertSpaceBeforeUpperCase(upload.UploadName)} is not specified" });
                }
            }

            if(errors.Count > 0) { return; }

            //check file size for passport photo
            _coreCharacterCertificateInputValidation.CheckFileSize(uploads, errors, uploads.ElementAt(0).MaxSize);
            _coreCharacterCertificateInputValidation.CheckFileType(uploads, errors);

            if (errors.Count > 0) { return; }

            
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.CharacterCertificateFilePath.ToString()).FirstOrDefault();
            if (node == null || string.IsNullOrEmpty(node.Value))
            {
                Logger.Error(string.Format("Unable to get character certificate file upload path in config file"));
                throw new Exception();
            }
            List<string> paths = new List<string>();
            int counter = 0;
            string fileName = string.Empty;
            string path = string.Empty;

            foreach (var uploadedFile in uploads)
            {
                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                path = Path.Combine(basePath.FullName, Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + ++counter + Path.GetExtension(uploadedFile.Upload.FileName));
                paths.Add(path);
                //save file
                uploadedFile.Upload.SaveAs(path);
            }
            //assign upload names and paths to request vm
            userInput.PassportPhotographUploadName = uploads.ElementAt(0).Upload.FileName;
            userInput.PassportPhotographUploadPath = paths.ElementAt(0);
            userInput.InternationalPassportDataPageUploadName = uploads.ElementAt(1).Upload.FileName;
            userInput.InternationalPassportDataPageUploadPath = paths.ElementAt(1);
            userInput.SignatureUploadName = uploads.ElementAt(2).Upload.FileName;
            userInput.SignatureUploadPath = paths.ElementAt(2);
        }


    }
}