using Orchard;
using Orchard.Autoroute.Services;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Validations.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Validations;
using Orchard.Users.Models;
using Parkway.CBS.Core.Lang;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreRevenueHeadService : CoreMDARevenueHeadService, ICoreRevenueHeadService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMimeTypeProvider _mimeTypeProvider;
        public IInvoicingService _invoicingService;
        private readonly IMDAManager<MDA> _mdaRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;

        private readonly IValidator _validator;
        private readonly ISlugService _slugService;

        public CoreRevenueHeadService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IInvoicingService invoicingService, IMDAManager<MDA> mdaRepository, IValidator validator, ISlugService slugService, IRevenueHeadManager<RevenueHead> revenueHeadRepository, IAPIRequestManager<APIRequest> apiRequestRepository) : base(orchardServices, slugService, validator, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _mediaLibraryService = mediaManagerService;
            _mimeTypeProvider = mimeTypeProvider;
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _mdaRepository = mdaRepository;
            _validator = validator;
            _slugService = slugService;
            _revenueHeadRepository = revenueHeadRepository;
            _apiRequestRepository = apiRequestRepository;
        }


        #region Create revenue heads


        /// <summary>
        /// Create revenue head
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public CreateRevenueHeadResponseModel TryCreateRevenueHead(UserPartRecord user, ref List<ErrorModel> errors, CreateRevenueHeadsModel model, ExpertSystemSettings expertSystem = null, string requestReference = null) 
        {
            //check request identifier
            if (!string.IsNullOrEmpty(requestReference))
            {
                Logger.Error(string.Format("Checking request reference if any client ID: {0} Ref: {1}", expertSystem.ClientId, requestReference ?? ""));
                Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestReference, expertSystem, CallTypeEnum.RevenueHead);
                if (refResult != 0)
                {
                    Logger.Error(string.Format("Request reference found for client ID: {0} Ref: {1} MDA: {2}", expertSystem.ClientId, requestReference, refResult));
                    var persistedRevenueHead = _revenueHeadRepository.Get((int)refResult);
                    if (persistedRevenueHead != null)
                    {
                        Logger.Error(string.Format("Returing ref {0}", requestReference));
                        //check if the request reference for the revenue head has the same parent mda Id
                        if(persistedRevenueHead.Mda.Id != model.ParentMDAId) { throw new MDARecordNotFoundException("The request ref revenue head has an MDA mismatch"); }
                        return new CreateRevenueHeadResponseModel
                        {
                            RevenueHeads = new List<RevenueHeadResponseModel> {
                                {
                                    new RevenueHeadResponseModel { CashflowId = persistedRevenueHead.CashFlowProductId, CashflowProductCode = persistedRevenueHead.CashFlowProductCode, CBSId = persistedRevenueHead.Id, Code = persistedRevenueHead.Code, Name = persistedRevenueHead.Name, ParentMDAId = persistedRevenueHead.Mda.Id, ParentRevenueHeadId = persistedRevenueHead.Revenuehead == null? 0 : persistedRevenueHead.Revenuehead.Id, Slug = persistedRevenueHead.Slug }
                                }
                            }
                        };
                    }
                    throw new CannotFindRevenueHeadException("Cannot find revenue head from API request ref " + refResult.ToString());
                }
            }
            MDA mda = null;
            RevenueHead revenueHead = null;
            Logger.Error("Validating create revenue head request");
            if (!model.IsSubRevenueHead)
            {
                if(model.ParentMDAId != 0) {  mda = GetMDA(model.ParentMDAId); }
                else { mda = GetMDA(model.ParentMDASlug); }
               
                ValidateRevenueHeads(model.RevenueHeads, mda, null, ref errors, user, new string[] { "MDA_Id:false:" + mda.Id });
            }
            else
            {
                revenueHead = GetRevenueHead(model.ParentRevenueHeadId);
                mda = revenueHead.Mda;
                if(mda == null) { throw new MDARecordNotFoundException(); }
                ValidateRevenueHeads(model.RevenueHeads, mda, revenueHead, ref errors, user, new string[] { "RevenueHead_ID:false:" + model.ParentRevenueHeadId });
            }
            Logger.Error("Saving revenue head(s)");
            SaveRecords(mda, revenueHead, model.RevenueHeads, user, expertSystem, requestReference);

            var responseRevenueHeads = model.RevenueHeads.Select(rh => new RevenueHeadResponseModel { CBSId = rh.Id, Code = rh.Code, Name = rh.Name, ParentMDAId = mda.Id, Slug = rh.Slug, ParentRevenueHeadId = revenueHead == null ? 0 : revenueHead.Id, CashflowId = rh.CashFlowProductId, CashflowProductCode = rh.CashFlowProductCode });
            Logger.Error(string.Format("Returning response"));
            return new CreateRevenueHeadResponseModel { RevenueHeads = responseRevenueHeads.ToList() };
        }


        #endregion


        #region Update revenue heads


        /// <summary>
        /// Try update a revenue head
        /// </summary>
        /// <param name="revenueHead">RevenueHead, persisted revenuehead</param>
        /// <param name="updatedRevenueHead">RevenueHead, the updated revenue head model</param>
        /// <param name="user">UserPartRecord</param>
        /// <param name="errors">List{ErrorModel}</param>
        public CreateRevenueHeadResponseModel TryUpdateRevenueHead(RevenueHead revenueHead, RevenueHead updatedRevenueHead, UserPartRecord user, ref List<ErrorModel> errors)
        {
            try
            {
                var mda = revenueHead.Mda;
                var billing = revenueHead.BillingModel;
                if (!Enum.IsDefined(typeof(SettlementType), updatedRevenueHead.SettlementType))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Invalid Settlement Type", FieldName = "RevenueHead.SettlementType" });
                    throw new DirtyFormDataException();
                }

                if (String.IsNullOrEmpty(updatedRevenueHead.SettlementCode))
                {
                    updatedRevenueHead.SettlementCode = null;
                    //check that settlement type is set to none
                    if (updatedRevenueHead.SettlementType != (int)SettlementType.None)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Add a corresponding Settlement Code for the Settlement Type", FieldName = "RevenueHead.SettlementCode" });
                        throw new DirtyFormDataException();
                    }
                }
                else
                {
                    if (updatedRevenueHead.SettlementType == (int)SettlementType.None)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Add a corresponding Settlement Type for the Settlement Code", FieldName = "RevenueHead.SettlementType" });
                        throw new DirtyFormDataException();
                    }
                }

                if (string.IsNullOrEmpty(updatedRevenueHead.ServiceId)) { updatedRevenueHead.ServiceId = null; }

                ValidateRevenueHeadsUpdates(new List<RevenueHead> { { updatedRevenueHead } }, ref errors, new string[] { "Id:true:" + revenueHead.Id, "MDA_Id:false:" + mda.Id });

                bool hasNameChanged = revenueHead.Name != updatedRevenueHead.Name;
                bool hasCodeChanged = revenueHead.Code != updatedRevenueHead.Code;

                UpdateModel(revenueHead, updatedRevenueHead, user);
                //update invoicing service details, product details
                //does the product have a billing info, if so that means the product should have been created on invc serv
                if (billing != null)
                {
                    if (string.IsNullOrEmpty(mda.SMEKey))
                    {
                        Logger.Information("No SMKEY found. Cannot edit revenue head, MDA does not exist on cashflow");
                        throw new KeyNotFoundException("No SMKEY found. MDA does not have a cashflow account");
                    }

                    //if nothing changed don't bother calling cashflow
                    if (hasCodeChanged || hasNameChanged)
                    {
                        UpdateOnCashflow(revenueHead, mda.SMEKey, billing.Amount);//Uzo says Cashflow is one word 
                    }
                }
                UpdateRecord(revenueHead);

                var responseRevenueHead = new RevenueHeadResponseModel { CBSId = revenueHead.Id, Code = revenueHead.Code, Name = revenueHead.Name, ParentMDAId = mda.Id, Slug = revenueHead.Slug, ParentRevenueHeadId = revenueHead.Revenuehead == null ? 0 : revenueHead.Revenuehead.Id, CashflowId = revenueHead.CashFlowProductId, CashflowProductCode = revenueHead.CashFlowProductCode };

                return new CreateRevenueHeadResponseModel { RevenueHeads = new List<RevenueHeadResponseModel> { { responseRevenueHead } } };
            }
            catch (Exception)
            {
                _revenueHeadRepository.RollBackAllTransactions();
                _revenueHeadRepository.ClearSession();
                throw;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="revenueHead">RevenueHead</param>
        /// <param name="updatedRevenueHead">RevenueHead</param>
        /// <param name="user">UserPartRecord</param>
        private void UpdateModel(RevenueHead revenueHead, RevenueHead updatedRevenueHead, UserPartRecord user)
        {
            revenueHead.Name = updatedRevenueHead.Name;
            revenueHead.Code = updatedRevenueHead.Code;
            revenueHead.Slug = updatedRevenueHead.Slug;
            revenueHead.CashFlowProductCode = revenueHead.Code + "/" + revenueHead.Id;
            revenueHead.LastUpdatedBy = user;
            revenueHead.CallBackURL = updatedRevenueHead.CallBackURL;
            revenueHead.SettlementType = updatedRevenueHead.SettlementType;
            revenueHead.SettlementCode = updatedRevenueHead.SettlementCode;
            revenueHead.ServiceId = updatedRevenueHead.ServiceId?.Trim();
        }


        /// <summary>
        /// Get revenue head details by revenue head Id
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHeadDetails</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadDetails GetRevenueHeadDetails(int id)
        {
            var revenueHead = _revenueHeadRepository.GetRevenueHeadDetails(id);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(); }
            return revenueHead;
        }


        /// <summary>
        /// Get all the details you would need for invoice generation given the revenue head Id
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadDetailsForInvoiceGeneration GetRevenueHeadDetailsForInvoiceGeneration(int revenueHeadId)
        {
            RevenueHeadDetailsForInvoiceGeneration response = _revenueHeadRepository.GetRevenueHeadDetailsForInvoiceGeneration(revenueHeadId);
            if (response == null) { throw new CannotFindRevenueHeadException(); }
            return response;
        }

        /// <summary>
        /// Get the Id of the paye revenue head
        /// </summary>
        /// <returns>long</returns>
        public int GetIdRevenueHeadForPaye()
        {
            try
            {
                var record = _revenueHeadRepository.Get(r => ((r.IsPayeAssessment) && (r.IsVisible) && (r.IsActive)));
                if(record == null) { throw new NoRecordFoundException(string.Format("Error getting the revenue head for paye assessment")); }
                return record.Id;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting the revenue head for paye assessment. Error Msg: {0}", exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Get revenue head details by revenue head Id
        /// <para>An extra flag is passed instructing the method to either get the revenue head details along with the form details or not</para>
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <param name="dontGetFormDetails">Instruct the method to get or not get form details</param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        public IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadDetailsForInvoice(int id, bool dontGetFormDetails = false)
        {
            if (dontGetFormDetails)
                return _revenueHeadRepository.GetRevenueHeadVMForInvoiceGeneration(id);
            return _revenueHeadRepository.GetRevenueHeadVMWithFormValidationDetails(id);
        }


        /// <summary>
        /// Get the revenue head details for a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        public GenerateInvoiceRequestModel GetGroupRevenueHeadDetailsForInvoice(int groupId)
        {
            return _revenueHeadRepository.GetGroupRevenueHeadVMForInvoiceGeneration(groupId);
        }


        /// <summary>
        /// Get revenue head id that is assigned to collect unreconciled collection
        /// </summary>
        /// <returns>int</returns>
        public int GetRevenueHeadIdForUnreconciledCollections(string tenantName)
        {
            string sId = string.Empty;
            int id = 0;
            var stateConfig = Utilities.Util.GetTenantConfigBySiteName(tenantName);
            var result = stateConfig.Node.Where(n => n.Key == TenantConfigKeys.UnreconciledRevenueHeadId.ToString()).FirstOrDefault();
            if(result != null) { sId = result.Value; }
            Int32.TryParse(sId, out id);
            return id;
        }


        /// <summary>
        /// Update cashflow product
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>CashFlowProduct</returns>
        private CashFlowProduct UpdateOnCashflow(RevenueHead revenueHead, string companyKey, decimal price)
        {
            try
            {
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", companyKey } });
                var productService = _invoicingService.ProductServices(context);
                #endregion

                CashFlowEditProduct product = new CashFlowEditProduct
                {
                    Product = new CashFlowCreateProduct
                    {
                        Name = revenueHead.NameAndCode(),
                        ProductCode = revenueHead.CashFlowProductCode,
                        Description = string.Format("{0} Assessment", revenueHead.NameAndCode()),
                        Price = price
                    },
                    ProductId = revenueHead.CashFlowProductId
                };
                return productService.EditProduct(product);
            }
            catch (Exception exception)
            {
                Logger.Error("Error Occured : {0}", exception.Message);
                throw new CannotConnectToCashFlowException(exception.Message, exception);
            }
        }

        private void ValidateRevenueHeadsUpdates(ICollection<RevenueHead> revenueHeads, ref List<ErrorModel> errors, string[] inclusiveClauses)
        {
            TrimString(revenueHeads);
            errors = ValidateInnerCollection(revenueHeads);
            //if form data has duplicate values
            if (errors.Count > 0) { throw new DirtyFormDataException("Duplicate items"); }

            List<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            dataValues = GetValidationModel(revenueHeads, inclusiveClauses);//parent Id

            errors = ValidateUniqueness<RevenueHead>(dataValues);
            if (errors.Count > 0) { throw new DirtyFormDataException("Record already exists"); }
            SetSlug(revenueHeads);
        }

        private void ValidateRevenueHeads(ICollection<RevenueHead> revenueHeads, MDA mda, RevenueHead revenueHead, ref List<ErrorModel> errors, UserPartRecord user, string[] inclusiveClauses = null)
        {
            if (revenueHead != null) { HasBilling(revenueHead); }

            TrimString<RevenueHead>(revenueHeads);
            errors = ValidateInnerCollection(revenueHeads);
            //if form data has duplicate values
            if (errors.Count > 0) { throw new DirtyFormDataException("Duplicate items"); }

            List<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            dataValues = GetValidationModel(revenueHeads, inclusiveClauses);//parent TaxEntityId

            errors = ValidateUniqueness<RevenueHead>(dataValues);
            if (errors.Count > 0) { throw new DirtyFormDataException("Record already exists"); }
            SetSlug(revenueHeads);
        }


        /// <summary>
        /// Get mda record
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>MDARecordNotFoundException</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public MDA GetMDA(int mdaId)
        {
            MDA mda = _mdaRepository.Get(mdaId);
            if (mda == null) { throw new MDARecordNotFoundException(string.Format("Could not find MDA record {0}", mdaId)); }
            return mda;
        }


        /// <summary>
        /// Get mda record
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>MDARecordNotFoundException</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        public MDA GetMDA(string mdaSlug)
        {
            MDA mda = _mdaRepository.Get("Slug", mdaSlug);
            if (mda == null) { throw new MDARecordNotFoundException(string.Format("Could not find MDA record {0}", mdaSlug)); }
            return mda;
        }

        /// <summary>
        /// Get revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHead GetRevenueHead(int revenueHeadId)
        {
            RevenueHead revenueHead = _revenueHeadRepository.Get(revenueHeadId);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(string.Format("Could not find revenue head with the ID - {0}", revenueHeadId)); }
            return revenueHead;
        }


        /// <summary>
        /// Get the view model for this revenue head 
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadVM</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadVM GetRevenueHeadVM(int revenueHeadId)
        {
            RevenueHeadVM revenueHead = _revenueHeadRepository.GetRevenueHeadVM(revenueHeadId);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(string.Format("Could not find revenue head with the ID - {0}", revenueHeadId)); }
            return revenueHead;
        }


        /// <summary>
        /// Get the revenue head details by the revenue head short code
        /// </summary>
        /// <param name="revenueHeadCode"></param>
        /// <returns>RevenueHeadVM</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHeadVM GetRevenueHeadVMByCode(string revenueHeadCode)
        {
            RevenueHeadVM revenueHead = _revenueHeadRepository.GetRevenueHeadVMByCode(revenueHeadCode);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(string.Format("Could not find revenue head with the code - {0}", revenueHeadCode)); }
            return revenueHead;
        }


        /// <summary>
        /// Get revenue head by ID and SLug
        /// </summary>
        /// <param name="revenueHeadSlug">revenue head ID</param>
        /// <param name="revenueHeadId">revenue head slug</param>
        /// <returns></returns>
        public RevenueHead GetRevenueHead(string revenueHeadSlug, int revenueHeadId)
        {
            var revenueHead = _revenueHeadRepository.Get(revenueHeadId);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(string.Format("Could not find revenue head with the ID - {0}", revenueHeadId)); }
            if (revenueHead.Slug != revenueHeadSlug) { throw new CannotFindRevenueHeadException(string.Format("Could not find revenue head with the ID and slug - {0} {1}", revenueHeadId, revenueHeadSlug)); }
            return revenueHead;
        }

        private void UpdateRecord(RevenueHead revenueHead)
        {            
            if (!_revenueHeadRepository.Update(revenueHead)) { throw new CannotUpdateRevenueHeadException(); }
        }
        

        /// <summary>
        /// Prepare the collection data for validation
        /// </summary>
        /// <param name="collection">Collection of MDAs</param>
        /// <returns>List{UniqueValidationModel}</returns>
        public List<UniqueValidationModel> GetValidationModel(ICollection<RevenueHead> collection, string[] inclusiveClauses = null)
        {
            ICollection<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            int counter = 0;
            foreach (var model in collection)
            {
                dataValues.Add(new UniqueValidationModel()
                {
                    Identifier = "RevenueHeadsCollection[" + counter + "].Name",
                    Name = "Name",
                    SelectDataValue = "Name:" + model.Name,
                    InclusiveClauses = inclusiveClauses ?? (new string[] { }),
                    ErrorMessage = ErrorLang.revenueheadmdanotunique().ToString()
                });

                dataValues.Add(new UniqueValidationModel()
                {
                    Identifier = "RevenueHeadsCollection[" + counter++ + "].Code",
                    Name = "Code",
                    SelectDataValue = "Code:" + model.Code,
                    InclusiveClauses = inclusiveClauses ?? (new string[] { }),
                    ErrorMessage = ErrorLang.revenueheadcodenotunique().ToString()
                });
            }
            return dataValues.ToList();
        }


        /// <summary>
        /// Prepare the revenue heads for persistence
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHeads"></param>
        private void SaveRecords(MDA mda, RevenueHead parentRevenueHead, ICollection<RevenueHead> revenueHeads, UserPartRecord user, ExpertSystemSettings expertSystem, string requestReference)
        {
            if (user == null) { throw new AuthorizedUserNotFoundException(); }
            Logger.Error("Saving revenue heads");
            foreach (var revenueHead in revenueHeads)
            {
                revenueHead.LastUpdatedBy = user;
                revenueHead.AddedBy = user;
                revenueHead.Mda = mda;
                if (parentRevenueHead != null) { revenueHead.Revenuehead = parentRevenueHead; }
            }
            if (string.IsNullOrEmpty(requestReference))
            {
                Logger.Error("Saving revenue heads by the bundle");
                if (!_revenueHeadRepository.SaveBundle(revenueHeads)) { throw new CannotSaveRevenueHeadException(); }
            }
            else
            {
                Logger.Error("Saving revenue head with request reference");
                if (!_revenueHeadRepository.SaveRevenueHeadWithRequestReference(revenueHeads.ElementAt(0), expertSystem, requestReference)) { throw new CannotSaveRevenueHeadException(); }
            }
        }

        /// <summary>
        /// Check if revenue head has billing info
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <exception cref="RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException"></exception>
        public void HasBilling(RevenueHead revenueHead)
        {
            if (revenueHead.BillingModel != null) { throw new RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException(ErrorLang.revenueheadcannothaveasubrevenuehead().ToString()); }
        }

        /// <summary>
        /// Turn the visiblity of the parent revenue head to true, and then finally turn the MDA visiblity to true
        /// </summary>
        /// <param name="revenueHead"></param>
        public void TurnOnParentVisibilty(RevenueHead revenueHead)
        {
            if (revenueHead.Revenuehead == null) { revenueHead.Mda.IsVisible = true; return; }
            //If the parent revenue head is inactive, return.
            if (!revenueHead.Revenuehead.IsActive) { return; }
            // If the parent revenue head is active, proceed to set visibility to true
            revenueHead.Revenuehead.IsVisible = true;
            //perform this same operation up the hierarchy
            TurnOnParentVisibilty(revenueHead.Revenuehead);
        }


        /// <summary>
        /// Turn the visiblity of the parent revenue head to true, and then finally turn the MDA visiblity to true
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        public void TurnOnParentVisibilty(MDA mda, RevenueHead revenueHead)
        {
            if (revenueHead.Revenuehead == null) { mda.IsVisible = true; return; }
            //If the parent revenue head is inactive, return.
            if (!revenueHead.Revenuehead.IsActive) { return; }
            // If the parent revenue head is active, proceed to set visibility to true
            revenueHead.Revenuehead.IsVisible = true;
            //perform this same operation up the hierarchy
            TurnOnParentVisibilty(revenueHead.Revenuehead);
        }


        /// <summary>
        /// Has subrevenue heads
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        /// <exception cref="CannotFindRevenueHeadException">Null object</exception>
        public bool HasSubRevenueHeads(RevenueHead revenueHead)
        {
            if(revenueHead == null) { throw new CannotFindRevenueHeadException("No revenue head found"); }
            if(revenueHead.RevenueHeads != null && revenueHead.RevenueHeads.Count() > 0) { return true; }
            return false;
        }


        /// <summary>
        /// Check if there is a paye revenue head already set-up
        /// </summary>
        /// <returns>bool</returns>
        public bool PayeExists()
        {
            return _revenueHeadRepository.Count(rev => rev.IsPayeAssessment) > 0;
        }


        /// <summary>
        /// get the Id of the revenue head assigned to be the payee assessment
        /// </summary>
        /// <returns>int?</returns>
        public int? PayeeId()
        {
            return _revenueHeadRepository.Get(r => r.IsPayeAssessment)?.Id;
        }

    }
}