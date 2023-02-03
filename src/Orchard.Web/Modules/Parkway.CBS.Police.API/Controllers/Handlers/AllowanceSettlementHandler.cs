using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class AllowanceSettlementHandler : IAllowanceSettlementHandler
    {
        public ILogger Logger { get; set; }
        private readonly IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance> _deploymentAllowanceManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IPoliceofficerDeploymentAllowanceSettlementLogManager<PoliceofficerDeploymentAllowanceSettlementLog> _allowanceSettlementLogManager;

        public AllowanceSettlementHandler(IPoliceofficerDeploymentAllowanceManager<PoliceofficerDeploymentAllowance> deploymentAllowanceManager, IOrchardServices orchardServices, IPoliceofficerDeploymentAllowanceSettlementLogManager<PoliceofficerDeploymentAllowanceSettlementLog> allowanceSettlementLogManager)
        {
            _deploymentAllowanceManager = deploymentAllowanceManager;
            _orchardServices = orchardServices;
            _allowanceSettlementLogManager = allowanceSettlementLogManager;
        }

        /// <summary>
        /// Update the payment status for a particular deployment allowance request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        public APIResponse UpdateDeploymentAllowanceStatus(DeploymentAllowancePaymentNotificationModel model)
        {
            try
            {
                if(model.ResponseObject == null)
                {
                    Logger.Error($"Response object is empty.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = "Response object is empty" };
                }

                if (string.IsNullOrEmpty(model.ResponseObject.ReferenceNumber) || string.IsNullOrEmpty(model.ResponseObject.Hmac))
                {
                    Logger.Error($"ReferenceNumber or Hmac is empty.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = "ReferenceNumber or Hmac is empty" };
                }

                if (model.ResponseObject.Items == null || model.ResponseObject.Items.Count < 1)
                {
                    Logger.Error($"Payment items is empty. Reference Number: {model.ResponseObject.ReferenceNumber}.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = "Payment items is empty" };
                }

                bool convertStatus = Enum.TryParse(model.ResponseObject.Items.First().StatusCode, true, out DeploymentAllowanceStatus paymentStatus);
                if (!convertStatus)
                {
                    Logger.Error($"Unable to convert settlement engine payment status code. Reference Number: {model.ResponseObject.ReferenceNumber}.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = $"Unable to convert settlement engine payment status code." };
                }

                string secret = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.POSSAPSettlementSecret)).SingleOrDefault()?.Value;

                if (string.IsNullOrEmpty(secret))
                {
                    Logger.Error($"Unable to get payment secret key. Reference Number: {model.ResponseObject.ReferenceNumber}.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = "Unable to get payment secret key" };
                }

                if (model.ResponseObject.Hmac != Util.HMACHash256(model.ResponseObject.ReferenceNumber + Newtonsoft.Json.JsonConvert.SerializeObject(model.ResponseObject.Items), secret))
                {
                    Logger.Error($"Hmac mismatch. Reference Number: {model.ResponseObject.ReferenceNumber}.");
                    return new APIResponse { StatusCode = System.Net.HttpStatusCode.BadRequest, Error = true, ResponseObject = "Hmac mismatch" };
                }

                if(!_allowanceSettlementLogManager.Save(new PoliceofficerDeploymentAllowanceSettlementLog { ReferenceNumber = model.ResponseObject.ReferenceNumber, ItemReference = model.ResponseObject.Items.First().Reference, TransactionStatus = (int)paymentStatus }))
                {
                    Logger.Error($"Unable to save deployment allowance settlement log. Reference Number: {model.ResponseObject.ReferenceNumber}.");
                    throw new CouldNotSaveRecord("Unable to save deployment allowance settlement log");
                }

                _deploymentAllowanceManager.UpdateDeploymentAllowanceTransactionStatus(model.ResponseObject.ReferenceNumber, (int)paymentStatus);
                Logger.Information($"Payment updated successfully. Reference Number: {model.ResponseObject.ReferenceNumber}. Status: {model.ResponseObject.Items.First().StatusCode}");
                return new APIResponse { StatusCode = System.Net.HttpStatusCode.OK, ResponseObject = $"Payment updated successfully. Reference Number: {model.ResponseObject.ReferenceNumber}" };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}