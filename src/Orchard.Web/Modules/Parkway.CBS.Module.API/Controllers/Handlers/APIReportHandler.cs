using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Lang;
using Newtonsoft.Json;
using Parkway.CBS.Core.Exceptions;
using Orchard.Users.Models;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System.Dynamic;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.DataFilters.CollectionReport;
using Parkway.CBS.Core.Models.Enums;
using System.Globalization;
using System.Linq;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class APIReportHandler : BaseAPIHandler, IAPIReportHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreCollectionService _coreCollectionService;


        public APIReportHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository, IOrchardServices orchardServices, ICoreCollectionService coreCollectionService) : base(settingsRepository)
        {
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _coreCollectionService = coreCollectionService;
        }


        public APIResponse GetCollectionReport(ReportController callback, CollectionReportViewModel model, dynamic headerParams = null)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            System.Net.HttpStatusCode responseCode = System.Net.HttpStatusCode.BadRequest;
            ErrorCode errorCode = new ErrorCode();

            try
            {
                //check if model state is valid
                callback.Validate(model);
                CheckModelState<ReportController>(callback, ref errors);

                ExpertSystemSettings expertSystem = GetExpertSystem(headerParams.CLIENTID);
                //do check for hash
                string value = model.MDAId.ToString() + model.FromRange + model.EndRange + expertSystem.ClientId;

                if (!CheckHash(value, headerParams.SIGNATURE, expertSystem.ClientSecret))
                {
                    Logger.Error(string.Format("Signature ver failed for collection report API request Tenant {0} model: {1} ", expertSystem.ClientId, JsonConvert.SerializeObject(model)));
                    return new APIResponse { ErrorCode = ErrorCode.PPS1.ToString(), StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.couldnotcomputehash().ToString(), FieldName = "Signature" } } } };
                }
                //TODO: do permission check for user
                int take = model.Pager == null ? 0 : (model.Pager.PageSize > 100 ? 100 : model.Pager.PageSize);
                int skip = 0;
                if (take <= 0) { take = 10; }
                int page = model.Pager == null ? 1 : model.Pager.Page;
                if(page > 0) { skip = (page - 1) * take; }

                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                try
                {
                    startDate = DateTime.ParseExact(model.FromRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(model.EndRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new DateTimeCouldNotBeParsedException(string.Format("Invalid date format From Range - {0}, End Range - {1}. Expected form dd/MM/yyyy", model.FromRange, model.EndRange));
                }

                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                
                CollectionSearchParams searchParams = new CollectionSearchParams
                {
                    FromRange = startDate,
                    EndRange = endDate,
                    InvoiceNumber = model.InvoiceNumber,
                    PaymentRef = model.PaymentRef,
                    SRevenueHeadId = model.SelectedRevenueHead,
                    PaymentProviderId = model.PaymentProvider == 0 ? (int)PaymentProvider.None : model.PaymentProvider,
                    ReceiptNumber = model.ReceiptNumber,
                    SelectedMDA = model.MDAId.ToString(),
                    SelectedBankCode = model.SelectedBank,
                    PaymentDirection = model.OrderBy == 0 ? CollectionPaymentDirection.PaymentDate : (CollectionPaymentDirection)model.OrderBy,
                    AdminUserId = expertSystem.ThirdPartyAuthorizedAdmin.Id,
                    Take = take,
                    Skip = skip,
                };

                CollectionReportViewModel result = _coreCollectionService.GetReportForCollection(searchParams, false);

                CollectionReportViewModel returnModel = new CollectionReportViewModel
                {
                    ReportRecords = result.ReportRecords,
                    TotalAmountPaid = Math.Round(result.TotalAmountPaid, 2),
                    TotalNumberOfPayment = result.TotalNumberOfPayment,
                    MDAId = model.MDAId,
                    SelectedRevenueHead = model.SelectedRevenueHead,
                    FromRange = model.FromRange,
                    EndRange = model.EndRange,
                    PaymentRef = model.PaymentRef,
                    InvoiceNumber = model.InvoiceNumber,
                    ReceiptNumber = model.ReceiptNumber,
                    PaymentProvider = model.PaymentProvider,
                    SelectedBank = model.SelectedBank,
                    PaymentDirection = model.PaymentDirection,
                    PayerId =  model.PayerId,
                };

                returnModel.Pager = new ExpandoObject();
                returnModel.Pager.Page = page;
                returnModel.Pager.PageSize = take;

                return new APIResponse { ResponseObject = returnModel, StatusCode = System.Net.HttpStatusCode.OK };
            }
            #region catch clauses
            catch (TenantNotFoundException exception)
            {
                Logger.Error(exception, string.Format("Exception - {0}", exception.Message));
                errorCode = ErrorCode.PPTENANT404;
                errors.Add(new ErrorModel { FieldName = "Tenant", ErrorMessage = ErrorLang.tenant404().ToString() });
            }
            catch (AuthorizedUserNotFoundException exception)
            {
                Logger.Error(exception, ErrorLang.usernotfound().ToString());
                errorCode = ErrorCode.PPUSER404;
                errors.Add(new ErrorModel { FieldName = "User", ErrorMessage = ErrorLang.usernotfound().ToString() });
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, string.Format("Error occured while getting collection report API request - {0} Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPVE;
            }
            catch (DateTimeCouldNotBeParsedException exception)
            {
                Logger.Error(exception, exception.Message);
                errorCode = ErrorCode.PPVE;
                errors.Add(new ErrorModel { FieldName = "Date", ErrorMessage = exception.Message });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error occured while getting collection report API request - {0} Exception - {1}", Util.SimpleDump(model), exception.Message));
                errorCode = ErrorCode.PPIE;
                errors.Add(new ErrorModel { FieldName = "Collection", ErrorMessage = ErrorLang.errorgettingreport("Collection Payment").ToString() });
            }
            #endregion

            return new APIResponse { Error = true, ErrorCode = errorCode.ToString(), StatusCode = responseCode, ResponseObject = errors };
        }
    }
}