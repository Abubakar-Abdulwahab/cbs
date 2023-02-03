using Orchard.Logging;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.PaymentProviderHandlers
{
    public abstract class BaseHandler
    {
        public ILogger Logger { get; set; }

        public BaseHandler()
        {
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Assert that the value provided when hashed with the secret is equal to the
        /// hash value variable
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assertedHash"></param>
        /// <param name="maggi"></param>
        /// <returns></returns>
        protected virtual bool CheckHash(string value, string maggi, string assertedHash)
        {
            return (Util.HMACHash256(value, maggi) == assertedHash);
        }


        /// <summary>
        /// check if settlement type is appropriate for these params
        /// </summary>
        /// <param name="custReference"></param>
        /// <param name="merchantReference"></param>
        /// <param name="thirdPartyCode"></param>
        /// <param name="flatScheme"></param>
        /// <param name="mdaSettlementType"></param>
        /// <param name="revenueHeadSettlementType"></param>
        //protected virtual APIResponse CheckSettlementType(bool flatScheme, int mdaSettlementType, int revenueHeadSettlementType)
        //{
        //    string msg = string.Empty;
        //    try
        //    {
        //        int settlementType = revenueHeadSettlementType == ((int)SettlementType.None) ? mdaSettlementType : revenueHeadSettlementType;
        //        if (settlementType == ((int)SettlementType.None))
        //        {
        //            msg = string.Format("PAYMENT PROVIDER ::: settlement scheme not specified {0}", (SettlementType)settlementType);
        //            throw new UserNotAuthorizedForThisActionException(msg);
        //        }

        //        if (flatScheme)
        //        {
        //            if (settlementType != (int)SettlementType.Flat)
        //            {
        //                msg = string.Format("PAYMENT PROVIDER ::: scheme is flat but settlement type is {0}", (SettlementType)settlementType);
        //                throw new UserNotAuthorizedForThisActionException(msg);
        //            }
        //        }
        //        else
        //        {
        //            if (settlementType != (int)SettlementType.Percentage)
        //            {
        //                msg = string.Format("PAY DIRECT ::: scheme is percentage but settlement type is {0}", (SettlementType)settlementType);
        //                throw new UserNotAuthorizedForThisActionException();
        //            }
        //        }
        //        return null;
        //    }
        //    catch (UserNotAuthorizedForThisActionException exception)
        //    { Logger.Error("Customer ref : " + custRef + " " + exception.Message); }
        //    catch (Exception exception)
        //    { Logger.Error(exception, "Customer ref : " + custRef + " " + exception.Message); }

        //    return new PayDirectAPIResponseObj
        //    { StatusCode = HttpStatusCode.OK, ResponseObject = new CustomerInformationResponse { Customers = new List<Customer> { { new Customer { CustReference = custRef, Status = 1, StatusMessage = ErrorLang.usernotauthorized().ToString(), ThirdPartyCode = thirdPartyCode, Amount = 0.0m, FirstName = string.Empty } } }, MerchantReference = merchRef }, ReturnType = "CustomerInformationResponse" };
        //}


        /// <summary>
        /// get payment method
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <returns>PaymentMethods</returns>
        protected virtual PaymentMethods GetMethod(string paymentMethod)
        {
            switch (paymentMethod)
            {
                case "Cash":
                    return PaymentMethods.Cash;
                case "Cheque":
                    return PaymentMethods.Cheque;
                case "Card":
                    return PaymentMethods.DebitCard;
                case "Transfer":
                    return PaymentMethods.BankTransfer;
                case "Inter-transfer":
                    return PaymentMethods.InternalTransfer;
                default:
                    return PaymentMethods.OtherPaymentMethods;
            }
        }


        /// <summary>
        /// get payment channel
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns>PaymentChannel</returns>
        protected virtual PaymentChannel GetChannel(string channelName)
        {
            switch (channelName.ToLower())
            {
                case "bank branch":
                    return PaymentChannel.BankBranch;
                case "atm":
                    return PaymentChannel.ATM;
                case "pos":
                    return PaymentChannel.POS;
                case "web":
                    return PaymentChannel.Web;
                case "mob":
                    return PaymentChannel.MOB;
                case "kiosk":
                    return PaymentChannel.Kiosk;
                case "voice":
                    return PaymentChannel.Voice;
                case "agency banking":
                    return PaymentChannel.AgencyBanking;
                default:
                    return PaymentChannel.OtherChannels;
            }
        }

    }
}