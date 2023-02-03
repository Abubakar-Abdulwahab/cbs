using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Helpers
{
    public class WebRequestValidationService
    {
        

        public static bool IsValidEIRSPaymentRequest(string MerchantId, string MerchantSecret)
        {
            string configMerchantId = Configuration.MerchantId;
            string configMerchantSecret = Configuration.MerchantSecret;
            if (MerchantId == configMerchantId && MerchantSecret == configMerchantSecret)
            {
                return true;
            }
            else
                return false;

        }

        public static bool IsValidNetPayRequest(string MerchantId, string MerchantSecret)
        {
            string configMerchantId = Configuration.MerchantId;
            string configMerchantSecret = Configuration.MerchantSecret;
            if (MerchantId == configMerchantId && MerchantSecret == configMerchantSecret)
            {
                return true;
            }
            else
                return false; 
        }

        public static bool IsValidRequest(string ClientId, string ClientSecret)
        { 
            if (ClientId == Configuration.ClientId && ClientSecret == Configuration.ClientSecret)
            {
                return true;
            }
            else
                return false;
        }

        public static bool ValidateEIRSStaff(string emailAddress, string password)
        {
            if(emailAddress == Configuration.StaffAdminEmailAddress && password == Configuration.StaffAdminPassword)
            {
                return true;
            }
            else 
                return false;
            
        }

    }
}
