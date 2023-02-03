using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIPAYEHandler : BaseAPIHandler, IAPIPAYEHandler
    {
        private readonly IPAYEAPIRequestManager<PAYEAPIRequest> _PAYEAPIRequestManager;
        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private readonly IPAYEAddBatchManager _PAYEAddBatchManager;
        private readonly IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> _payeStagingrepo;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadManager;

        public APIPAYEHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository,
                              IPAYEAPIRequestManager<PAYEAPIRequest> apiRequestManager,
                              IPAYEAddBatchManager payeAddBatchManager,
                              ICoreTaxPayerService coreTaxPayerService, IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> payeStagingrepo,
                               IRevenueHeadManager<RevenueHead> revenueHeadManager) : base(settingsRepository)
        {
            Logger = NullLogger.Instance;
            _PAYEAPIRequestManager = apiRequestManager;
            _coreTaxPayerService = coreTaxPayerService;
            _payeStagingrepo = payeStagingrepo;
            _revenueHeadManager = revenueHeadManager;
            _PAYEAddBatchManager = payeAddBatchManager;
        }

        /// <summary>
        /// Validates the PAYE intialization request model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        private void ValidateInitializeBatchRequestModel(PAYEIntializeBatchRequestModel model, ref List<ErrorModel> errors)
        {
            if (model == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Model is empty", FieldName = "PAYEIntializeBatchRequest" });
                throw new DirtyFormDataException("Model is empty");
            }

            HasStringValue(model.BatchIdentifier, nameof(model.BatchIdentifier), ref errors);
            HasStringValue(model.CallbackURL, nameof(model.CallbackURL), ref errors);
            HasStringValue(model.EmployerPayerId, nameof(model.EmployerPayerId), ref errors);
        }

        private void ValidateAddBatchItemsRequestModel(PAYEAddBatchItemsRequestModel model, ref List<ErrorModel> errors)
        {
            if (model == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Model is empty", FieldName = "PAYEAddBatchItemsRequestModel" });
                throw new DirtyFormDataException("Model is empty");
            }

            HasStringValue(model.BatchIdentifier, nameof(model.BatchIdentifier), ref errors);
            HasStringValue(model.PageNumber.ToString(), nameof(model.PageNumber), ref errors);
        }

        /// <summary>
        /// Processes the initialize batch request and returns an API response
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse ProcessInitializeRequest(PAYEIntializeBatchRequestModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            Logger.Information("Process PAYE API Initialize Batch Request for " + JsonConvert.SerializeObject(model));

            try
            {
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                var valueString = $"{model.BatchIdentifier}{model.EmployerPayerId}";

                // Validate signature
                if (!CheckHash(valueString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + valueString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                // Validate request model
                ValidateInitializeBatchRequestModel(model, ref errors);

                int batchlimit = 50;
                string sbatchlimit = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.PayeApiBatchlimit);

                if (!string.IsNullOrEmpty(sbatchlimit))
                {
                    int.TryParse(sbatchlimit, out batchlimit);
                }

                // Validate batch request identifier does not exist
                if (_PAYEAPIRequestManager.BatchIdentifierExist(model.BatchIdentifier))
                {
                    throw new RecordAlreadyExistsException();
                }
                else
                {
                    var taxEntityId = _coreTaxPayerService.GetTaxEntityId(x => x.PayerId == model.EmployerPayerId);

                    var revenueHead = _revenueHeadManager.GetRevenueHeadDetailsForPaye();

                    DirectAssessmentModel directAssessment = JsonConvert.DeserializeObject<DirectAssessmentModel>(revenueHead.Billing.DirectAssessmentModel);

                    PAYEBatchRecordStaging savedBatchRecord = new PAYEBatchRecordStaging
                    {
                        RevenueHead = new RevenueHead { Id = revenueHead.RevenueHead.Id },
                        Billing = new BillingModel { Id = revenueHead.Billing.Id },
                        TaxEntity = new TaxEntity { Id = taxEntityId },
                        BatchRef = model.BatchIdentifier,
                        AdapterValue = directAssessment.AdapterValue,
                        AssessmentType = (int)PayeAssessmentType.FromAPI
                    };

                    if (!_payeStagingrepo.Save(savedBatchRecord))
                    {
                        throw new CouldNotSaveRecord();
                    }

                    bool savedSuccess = _PAYEAPIRequestManager.Save(new PAYEAPIRequest
                    {
                        CallbackURL = model.CallbackURL,

                        BatchIdentifier = model.BatchIdentifier,
                        ProcessingStage = (int)PAYEAPIProcessingStages.NotProcessed,
                        RequestedByExpertSystem = expertSystem,
                        BatchLimit = batchlimit,
                        TaxEntity = new TaxEntity { Id = taxEntityId },
                        PAYEBatchRecordStaging = savedBatchRecord
                    });

                    if (!savedSuccess)
                    {
                        throw new CouldNotSaveRecord();
                    }

                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = new { Message = "Initialized successfully", model.BatchIdentifier, BatchItemLimit = batchlimit } };
                }
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPPY404;
                errors.Add(new ErrorModel { FieldName = "PAYEIntializeBatchRequestModel", ErrorMessage = ErrorLang.norecord404("Tax entity not found").ToString() });
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (RecordAlreadyExistsException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
                errors.Add(new ErrorModel { ErrorMessage = $"{nameof(model.BatchIdentifier)} already exist", FieldName = $"{nameof(model.BatchIdentifier)}" });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.badrequest().ToString(), FieldName = "PAYEIntializeBatchRequest" });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "PAYEIntializeBatchRequestModel", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }

        /// <summary>
        /// Processes the add batch items request and returns an API response
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        public APIResponse ProcessAddBatchItemsRequest(PAYEAddBatchItemsRequestModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            Logger.Information("Process PAYE API Initialize Batch Request for " + JsonConvert.SerializeObject(model));

            try
            {
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);

                // Validate batch identifier
                var batchDetailsVM = _PAYEAPIRequestManager.GetBatchDetails(model.BatchIdentifier, expertSystem.Id);

                // Validate signature
                var valueString = $"{model.BatchIdentifier}{batchDetailsVM.PayerId}";

                if (!CheckHash(valueString, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Could not compute signature hash " + valueString);
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                if (model.PayeItems.Count <= batchDetailsVM.BatchLimit)
                {
                    var batchPageTrackerExist = _PAYEAddBatchManager.BatchItemPageExist(batchDetailsVM.BatchIdentifier, model.PageNumber);

                    if (!batchPageTrackerExist)
                    {
                        PAYEAPIBatchItemsPagesTracker payeAPIBatchItemsPagesTracker = new PAYEAPIBatchItemsPagesTracker
                        {
                            PageNumber = model.PageNumber,
                            PAYEAPIRequest = new PAYEAPIRequest { Id = batchDetailsVM.PAYEAPIRequestId },
                        };

                        var saveBatchTrackerResult = _PAYEAddBatchManager.SaveBatchItemsPagesTracker(payeAPIBatchItemsPagesTracker);

                        foreach (var payeItem in model.PayeItems)
                        {
                            try
                            {
                                PAYEBatchItemsStaging saveBatchItem = _PAYEAddBatchManager.PopulatePAYEBatchItems(batchDetailsVM, payeItem);

                                _PAYEAddBatchManager.TrySavePAYEBatchItems(payeAPIBatchItemsPagesTracker, batchDetailsVM, payeItem, saveBatchItem,
                                                  out bool saveBatchItemResult,
                                                  out bool saveBatchItemRefResult);

                                if (!saveBatchTrackerResult || !saveBatchItemResult || !saveBatchItemRefResult)
                                {
                                    throw new CouldNotSaveRecord();
                                }
                            }
                            catch (CouldNotSaveRecord exception)
                            {
                                _PAYEAddBatchManager.RollBackAllTransaction();

                                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                                errors.Add(new ErrorModel { FieldName = $"ItemNumber: {payeItem.ItemNumber}", ErrorMessage = ErrorLang.couldnotsavepayeitem().ToString() });
                                errorCode = ErrorCode.PPIE;
                                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                                return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
                            }
                        }
                        return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = new { Message = "Successfull", batchDetailsVM.BatchIdentifier, BatchItemLimit = batchDetailsVM.BatchLimit, model.PageNumber } };
                    }
                    else
                    {
                        errors.Add(new ErrorModel { FieldName = nameof(model.PageNumber), ErrorMessage = ErrorLang.batchidentifierandpageexist(model.PageNumber).ToString() });
                    }
                }
                else
                {
                    errors.Add(new ErrorModel { FieldName = "PAYEAddBatchItemsRequestModel", ErrorMessage = ErrorLang.batchlimitexceeded(batchDetailsVM.BatchLimit).ToString() });
                }
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.tenant404("Tenant").ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404("Tenant").ToString() });
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPPY404;
                errors.Add(new ErrorModel { FieldName = nameof(model.BatchIdentifier), ErrorMessage = ErrorLang.norecord404().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                httpStatusCode = System.Net.HttpStatusCode.BadRequest;
                errorCode = ErrorCode.PPVE;
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.badrequest().ToString(), FieldName = "PAYEAddBatchItemsRequestModel" });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, ErrorLang.genericexception().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "PAYEAddBatchItemsRequestModel", ErrorMessage = ErrorLang.genericexception().ToString() });
                errorCode = ErrorCode.PPIE;
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }
    }
}