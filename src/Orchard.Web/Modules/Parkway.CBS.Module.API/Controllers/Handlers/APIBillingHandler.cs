using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Users.Models;
using Newtonsoft.Json;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIBillingHandler : BaseAPIHandler, IAPIBillingHandler
    {
        private readonly ICoreBillingService _coreBillingService;
        private readonly ICoreRevenueHeadService _coreRevenueHeadService;
        public APIBillingHandler(ICoreBillingService coreBillingService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ICoreRevenueHeadService coreRevenueHeadService) : base(settingsRepository)
        {
            _coreBillingService = coreBillingService;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _coreRevenueHeadService = coreRevenueHeadService;
        }


        public APIResponse CreateBilling(BillingController callback, BillingHelperModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            try
            {
                Logger.Error("Creating billing for model " + JsonConvert.SerializeObject(model));

                Logger.Error("Validating model");
                CheckModelState(callback, ref errors);
                ////get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.AssessmentModel.Amount.ToString("F") + model.UserEmail + model.RevenueHeadID.ToString() + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Signature hash does not match " + value);
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                Logger.Error("Getting user");
                UserPartRecord user = GetUser(model.UserEmail);
                Logger.Error("Getting revenuehead");
                RevenueHead revenueHead = _coreRevenueHeadService.GetRevenueHead(model.RevenueHeadID);
                var result = _coreBillingService.TryPostBillingForCollection(revenueHead.Mda, revenueHead, user, ref errors, model, false, model.RequestReference, expertSystem);
                if(errors.Count > 0) { throw new DirtyFormDataException(); }
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = result };
            }
            #region catch clauses
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPRH404;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadnotfound().ToString() });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                responseCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
            }
            catch(NoBillingInformationFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPBILLING404;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.billinginfo404().ToString() });
            }
            catch (AlreadyHasBillingException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPB2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.alreadyhasbillinginfo().ToString() });
            }
            catch (CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPR2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadhassubrevenueheads().ToString() });
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, StatusCode = responseCode, ResponseObject = errors };
        }



        public APIResponse EditBilling(BillingController callback, BillingHelperModel model, dynamic headerParams)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            try
            {
                Logger.Error("Creating billing for model " + JsonConvert.SerializeObject(model));

                Logger.Error("Validating model");
                CheckModelState(callback, ref errors);
                ////get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.AssessmentModel.Amount.ToString("F") + model.UserEmail + model.RevenueHeadID.ToString() + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error("Signature hash does not match " + value);
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                Logger.Error("Getting user");
                UserPartRecord user = GetUser(model.UserEmail);
                Logger.Error("Getting revenuehead");
                RevenueHead revenueHead = _coreRevenueHeadService.GetRevenueHead(model.RevenueHeadID);
                var result = _coreBillingService.TryPostBillingForCollection(revenueHead.Mda, revenueHead, user, ref errors, model, true, model.RequestReference, expertSystem);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = result };
            }
            #region catch clauses
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPRH404;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadnotfound().ToString() });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                responseCode = System.Net.HttpStatusCode.NotFound;
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
            }
            catch (NoBillingInformationFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPBILLING404;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.billinginfo404().ToString() });
            }
            catch (AlreadyHasBillingException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPB2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.alreadyhasbillinginfo().ToString() });
            }
            catch (CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPR2;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadhassubrevenueheads().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.genericexception().ToString() });
            }
            #endregion

            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, StatusCode = responseCode, ResponseObject = errors };
        }
    }
}