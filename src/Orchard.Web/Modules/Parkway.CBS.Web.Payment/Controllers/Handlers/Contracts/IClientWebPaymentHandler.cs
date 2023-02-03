using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.ThirdParty.Payment.Processor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Web.Payment.Controllers.Handlers.Contracts
{
    public interface IClientWebPaymentHandler : IDependency, ICommonBaseHandler
    {

        /// <summary>
        /// Get model for web pay direct
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>PayDirectWebPaymentFormModel</returns>
        PayDirectWebPaymentFormModel GetPayDirectWebFormModel(string stateName, PayDirectWebPaymentRequestModel tokenModel);


        /// <summary>
        /// process pay direct web payment notif
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="model"></param>
        /// <returns>PayDirectWebPaymentValidationResponse</returns>
        PayDirectWebPaymentValidationResponse ProcessPaymentNotifRequestForPayDirectWeb(string stateName, PayDirectWebPaymentResponseModel model);
    }
}
