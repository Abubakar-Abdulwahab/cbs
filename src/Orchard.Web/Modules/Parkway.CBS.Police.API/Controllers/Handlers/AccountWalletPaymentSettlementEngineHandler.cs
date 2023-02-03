using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Data;
using System.Linq;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class AccountWalletPaymentSettlementEngineHandler : IAccountWalletPaymentSettlementEngineHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAccountPaymentRequestSettlementLogManager<AccountPaymentRequestSettlementLog> _paymentRequestSettlementLogManager;
        ILogger Logger { get; set; }

        public AccountWalletPaymentSettlementEngineHandler(IAccountPaymentRequestSettlementLogManager<AccountPaymentRequestSettlementLog> paymentRequestSettlementLogManager, IOrchardServices orchardServices)
        {
            _paymentRequestSettlementLogManager = paymentRequestSettlementLogManager;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }
        /// <summary>
        /// Proccesses the request
        /// </summary>
        /// <param name="model"></param>
        public APIResponse ProcessPaymentRequestCallBack(SettlementEnginePaymentStatusVM model)
        {

            try
            {
                if (model == null || model.ResponseObject.Items == null || !model.ResponseObject.Items.Any() )
                {
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.nopaymentrecordfound() };
                }

                string secret = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)?.Node?.Where(x => x.Key == nameof(PSSTenantConfigKeys.POSSAPSettlementSecret))?.FirstOrDefault()?.Value;

                if (string.IsNullOrEmpty(secret))
                {
                    throw new Exception($"Unable to get retrieve secret: {nameof(PSSTenantConfigKeys.POSSAPSettlementSecret)}");
                }

                if (model.ResponseObject.Hmac != Util.HMACHash256(model.ResponseObject.ReferenceNumber + JsonConvert.SerializeObject(model.ResponseObject.Items), secret))
                {
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.ToLocalizeString("Invalid Hmac").Text };
                }


                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(AccountPaymentRequestSettlementLog).Name);

                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestSettlementLog.Reference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestSettlementLog.PaymentReference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestSettlementLog.TransactionStatus), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestSettlementLog.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(AccountPaymentRequestSettlementLog.UpdatedAtUtc), typeof(DateTime)));

                string reference = string.Format("APRSL-{0}-REF-{1}", DateTime.Now.Ticks, Util.StrongRandom());


                foreach (var paymentRequest in model.ResponseObject.Items)
                {
                    DataRow row = dataTable.NewRow();

                    row[nameof(AccountPaymentRequestSettlementLog.Reference)] = reference;
                    row[nameof(AccountPaymentRequestSettlementLog.TransactionStatus)] = (int)paymentRequest.StatusCode;
                    row[nameof(AccountPaymentRequestSettlementLog.PaymentReference)] = paymentRequest.Reference;
                    row[nameof(AccountPaymentRequestSettlementLog.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(AccountPaymentRequestSettlementLog.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                if (!_paymentRequestSettlementLogManager.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(AccountPaymentRequestSettlementLog).Name))
                {
                    Logger.Information($"Unable to save records with reference: {model.ResponseObject.ReferenceNumber}");

                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception() };
                }

                _paymentRequestSettlementLogManager.UpdatePaymentRequestItemTransactionStatusFromLog(reference);

                Logger.Information($"Successfully updated the payment status with reference: {model.ResponseObject.ReferenceNumber}");

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception() };
            }

            return new APIResponse { Error = false, StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = ErrorLang.ToLocalizeString("Payment items proccessed successfully.").Text };
        }
    }
}