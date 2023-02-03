using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
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
    public class RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler : IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog> _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager;
        ILogger Logger { get; set; }
        public RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementEngineHandler(IOrchardServices orchardServices, IRegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager<RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog> regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager)
        {
            _orchardServices = orchardServices;
            _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager = regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager;
            Logger = NullLogger.Instance;
        }
        /// <summary>
        /// Processes the request
        /// </summary>
        /// <param name="model"></param>
        public APIResponse ProcessPaymentRequestCallBack(SettlementEnginePaymentStatusVM model)
        {
            try
            {
                if (model.ResponseObject == null || model.ResponseObject.Items == null || !model.ResponseObject.Items.Any())
                {
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.nopaymentrecordfound().Text };
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

                DataTable dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog).Name);
                dataTable.Columns.Add(new DataColumn(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.Reference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.PaymentReference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.TransactionStatus), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.UpdatedAtUtc), typeof(DateTime)));

                string reference = string.Format("DAPRS-{0}-REF-{1}", DateTime.Now.Ticks, Util.StrongRandom());
                
                foreach (var paymentRequest in model.ResponseObject.Items)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.Reference)] = reference;
                    row[nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.TransactionStatus)] = (int)paymentRequest.StatusCode;
                    row[nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.PaymentReference)] = paymentRequest.Reference;
                    row[nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }

                if (!_regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager.SaveBundle(dataTable, "Parkway_CBS_Police_Core_" + typeof(RegularizationUnknownOfficersDeploymentAllowancePaymentSettlementLog).Name))
                {
                    Logger.Information($"Unable to save records with reference: {model.ResponseObject.ReferenceNumber}");
                    return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().Text };
                }

                _regularizationUnknownOfficersDeploymentAllowancePaymentSettlementLogManager.UpdateRegularizationUnknownOfficersDeploymentAllowancePaymentRequestItemTransactionStatusFromLog(reference);
                
                Logger.Information($"Successfully updated the payment status with reference: {model.ResponseObject.ReferenceNumber}");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().Text };
            }
            return new APIResponse { Error = false, StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = ErrorLang.ToLocalizeString("Payment items proccessed successfully.").Text };
        }
    }
}