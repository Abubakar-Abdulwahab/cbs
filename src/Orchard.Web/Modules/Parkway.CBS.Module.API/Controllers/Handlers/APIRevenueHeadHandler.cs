using Orchard.Logging;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core;
using Orchard.Users.Models;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIRevenueHeadHandler : BaseAPIHandler, IAPIRevenueHeadHandler
    {
        private readonly ICoreRevenueHeadService _coreRevenueHeadService;

        public APIRevenueHeadHandler(ICoreRevenueHeadService coreRevenueHeadService, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            _coreRevenueHeadService = coreRevenueHeadService;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
        }

        public APIResponse CreateRevenueHead(RevenueHeadController callback, CreateRevenueHeadRequestModel model, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();
            
            try
            {
                //check if model state is valid
                CheckModelState(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.ParentMDAId.ToString() + model.UserEmail + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }

                UserPartRecord user = GetUser(model.UserEmail);
                CreateRevenueHeadsModel requestModel = new CreateRevenueHeadsModel
                {
                    IsSubRevenueHead = model.IsSubRevenueHead,
                    ParentMDAId = model.ParentMDAId,
                    ParentMDASlug = model.ParentMDASlug,
                    ParentRevenueHeadId = model.ParentRevenueHeadId,
                    RevenueHeads = new List<RevenueHead> { model.RevenueHead },
                    UserEmail = model.UserEmail,
                };

                CreateRevenueHeadResponseModel response = _coreRevenueHeadService.TryCreateRevenueHead(user, ref errors, requestModel, expertSystem, model.RequestReference);
                return new APIResponse { ResponseObject = response.RevenueHeads.ElementAt(0), StatusCode = System.Net.HttpStatusCode.OK };
            }
            #region catch clauses
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdacouldnotbefound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenuehead404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenuehead404().ToString() });
                errorCode = ErrorCode.PPRH404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                errorCode = ErrorCode.PPUSER404;
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            catch (RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException exception)
            {
                Logger.Error(exception, string.Format(ErrorLang.revenueheadcannothaveasubrevenuehead().ToString()));
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadcannothaveasubrevenuehead().ToString() });
                errorCode = ErrorCode.PPR2;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.tenant404().ToString(), FieldName = "Tenant" });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch(CannotSaveRevenueHeadException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.revenueheadscouldnotbesaved().ToString(), FieldName = "RevenueHead" });
                errorCode = ErrorCode.PPR3;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "Error" });
                errorCode = ErrorCode.PPIE;
            } 
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }


        public APIResponse EditRevenueHead(RevenueHeadController callback, EditRevenueHeadModel model, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            try
            {
                //check if model state is valid
                CheckModelState(callback, ref errors);
                //get tenant settings
                ExpertSystemSettings tenant = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.Name + model.Code + model.Id + model.UserEmail + headerParams.CLIENTID;
                if (!CheckHash(value, headerParams.SIGNATURE, tenant.ClientSecret))
                {
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.Forbidden, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                UserPartRecord user = GetUser(model.UserEmail);
                var revenueHead = _coreRevenueHeadService.GetRevenueHead(model.Id);
                var updatedRevenueHead = new RevenueHead { Name = model.Name, Code = model.Code };
                return new APIResponse { ResponseObject = _coreRevenueHeadService.TryUpdateRevenueHead(revenueHead, updatedRevenueHead, user, ref errors), StatusCode = System.Net.HttpStatusCode.OK };
            }
            #region catch clauses
            catch (MDARecordNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdacouldnotbefound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdacouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch(KeyNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.mdasmekycouldnotbefound().ToString() + " Exception: " + exception.Message);
                errors.Add(new ErrorModel { FieldName = "MDA", ErrorMessage = ErrorLang.mdasmekycouldnotbefound().ToString() });
                errorCode = ErrorCode.PPM3;
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
            }
            catch (CannotFindRevenueHeadException exception)
            {
                Logger.Error(exception, ErrorLang.revenuehead404().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenuehead404().ToString() });
                errorCode = ErrorCode.PPRH404;
                httpStatusCode = System.Net.HttpStatusCode.NotFound;
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString() + exception.Message);
                errors.Add(new ErrorModel { FieldName = "LastUpdatedBy", ErrorMessage = ErrorLang.usernotfound().ToString() });
                errorCode = ErrorCode.PPUSER404;
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
            }
            catch (RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException exception)
            {
                Logger.Error(exception, string.Format(ErrorLang.revenueheadcannothaveasubrevenuehead().ToString()));
                errors.Add(new ErrorModel { FieldName = "RevenueHead", ErrorMessage = ErrorLang.revenueheadcannothaveasubrevenuehead().ToString() });
                errorCode = ErrorCode.PPR2;
            }
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.tenant404().ToString(), FieldName = "Tenant" });
                httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                errorCode = ErrorCode.PPTENANT404;
            }
            catch (CannotSaveRevenueHeadException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.revenueheadscouldnotbesaved().ToString(), FieldName = "RevenueHead" });
                errorCode = ErrorCode.PPR3;
            }
            catch(CouldNotSaveBillingException exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.cannotconnettoinvoicingservice().ToString(), FieldName = "RevenueHead" });
                errorCode = ErrorCode.PPC1;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "Error" });
                errorCode = ErrorCode.PPIE;
            }
            #endregion
            return new APIResponse { ErrorCode = errorCode.ToString(), Error = true, ResponseObject = errors, StatusCode = httpStatusCode };
        }
    }
}