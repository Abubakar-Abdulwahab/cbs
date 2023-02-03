using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Helpers
{
    /// <summary>
    /// Returns value of specified keys/sections in the web config
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets the config value from the web config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        
        public static string EIRSBaseUrl
        {
            get
            {
                return GetConfig("EIRSBaseUrl");
            }
        }
        

        public static string NetPayBaseUrl
        {
            get
            {
                return GetConfig("NetPayBaseUrl");
            }
        }

        public static string POASearchTaxPayerUrl
        {
            get
            {
                return GetConfig("SearchTaxPayerUrl");
            }
        }

        public static string CBSPayQuicktellerPaymentCode
        {
            get
            {
                return GetConfig("QuicktellerPaymentCode");
            }
        }

        public static string GetQuicktellerClientId
        {
            get
            {
                return GetConfig("QuicktellerClientId");
            }
        }

        public static string GetQuicktellerSecretKey
        {
            get
            {
                return GetConfig("QuicktellerSecretKey");
            }
        }

        public static string GetQuicktellerBaseRESTURL
        {
            get
            {
                return GetConfig("QuicktellerBaseRESTURL");
            }
        }

        public static string GetQuicktellerRESTRequestURL
        {
            get
            {
                return GetConfig("QuicktellerRESTRequestURL");
            }
        }

        public static string GetQuicktellerCBSPayRedirectURL
        {
            get
            {
                return GetConfig("QuicktellerCBSPayRedirectURL");
            }
        }

        public static string SearchByBusinessNameUrl
        {
            get
            {
                return GetConfig("SearchByBusinessName");
            }
        }

        public static string SearchByPhoneUrl
        {
            get
            {
                return GetConfig("SearchByMobileNumber");
            }
        }
        public static string SearchByRINUrl
        {
            get
            {
                return GetConfig("SearchByRIN");
            }
        }


        public static string GetServiceBillItemsUrl
        {
            get
            {
                return GetConfig("ServiceBillItemsUrl");
            }
        }

        public static string GetAssessmentRuleItemsUrl
        {
            get
            {
                return GetConfig("AssessmentRuleItemsUrl");
            }
        }

        public static string GetServiceBillRulesUrl
        {
            get
            {
                return GetConfig("MDAServiceUrl");
            }
        }

        public static string GetInterswitchQuicktellerPrefix
        {
            get
            {
                return GetConfig("InterswitchQuicktellerPrefix");
            }
        }

        public static string GetAssessmentRulesUrl
        {
            get
            {
                return GetConfig("AssessmentRuleUrl");
            }
        }

        public static string EIRSAddSettlementUrl
        {
            get
            {
                return GetConfig("EIRSAddSettlementUrl");
            }
        }

        public static string NetPayPaymentUrl
        {
            get
            {
                return GetConfig("NetPayPaymentUrl");
            }
        }
        public static string NetPayReturnUrl
        {
            get
            {
                return GetConfig("NetPaymentNotificationReturnUrl");
            }
        }

        public static string GetEIRSAPILoginGrantType
        {
            get
            {
                return GetConfig("EIRSAPILoginGrantType");
            }
        }
        public static string GetEIRSAPILoginContentType
        {
            get
            {
                return GetConfig("EIRSAPILoginContentType");
            }
        }
        public static string GetEIRSAPILoginAcceptValue
        {
            get
            {
                return GetConfig("EIRSAPILoginAccept");
            }
        }

        public static string GetAssessmentDetails
        {
            get
            {
                return GetConfig("AssessmentDetailsUrl");
            }
        }


        public static string GetAssessmentDetailUrl
        {
            get
            {
                return GetConfig("AssessmentDetailsUrl");
            }
        }

        public static string GetServiceBillDetailsUrl
        {
            get
            {
                return GetConfig("ServiceBillDetailsUrl");
            }
        }

        public static string GetServiceBillDetails
        {
            get
            {
                return GetConfig("ServiceBillDetailsUrl");
            }
        }

        public static string GetEIRSAPILoginUrl
        {
            get
            {
                return GetConfig("LoginUrl");
            }
        }

        public static string GetEIRSAPILoginUsername
        {
            get
            {
                return GetConfig("EIRSAPILoginUsername");
            }
        }
         

        public static string GetEIRSAPILoginPassword
        {
            get
            {
                return GetConfig("EIRSAPILoginPassword");
            }
        }
        

        public static string GetPayDirectIP1
        {
            get
            {
                return GetConfig("PayDirectIP1");
            }
        }

        public static string GetPayDirectIP2
        {
            get
            {
                return GetConfig("PayDirectIP2");
            }
        }

        public static string GetPayDirectReferenceServiceUrl
        {
            get
            {
                return GetConfig("PayDirectReferenceServiceUrl");
            }
        }


        public static string GetPayDirectPOAServiceUrl
        {
            get
            {
                return GetConfig("PayDirectPOAServiceUrl");
            }
        }

        public static string MerchantId
        {
            get
            {
                return GetConfig("MerchantKey");
            }
        }

        public static string MerchantSecret
        {
            get
            {
                return GetConfig("MerchantSecret");
            }
        }

        public static string StaffAdminEmailAddress
        {
            get { return GetConfig("StaffAdminEmailAddress"); }
        }

        public static string StaffAdminPassword
        {
            get { return GetConfig("StaffAdminPassword"); }
        }


        public static string ClientId
        {
            get
            {
                return GetConfig("ClientId");
            }
        }

        public static string ClientSecret
        {
            get
            {
                return GetConfig("ClientSecret");
            }
        }

        public static string EconomicActivitiesListUrl
        {
            get
            {
                return GetConfig("EconomicActivitiesList");
            }
        }

        public static string TaxPayerTypeListUrl
        {
            get
            {
                return GetConfig("TaxPayerTypeList");
            }
        }


        public static string RevenueSubStreamListUrl
        {
            get
            {
                return GetConfig("RevenueSubStreamList");
            }
        }

        public static string RevenueStreamListUrl
        {
            get
            {
                return GetConfig("RevenueStreamList");
            }
        }
        public static string UnsyncedPaymentUrl
        {
            get
            {
                return GetConfig("UnsyncedPaymentUrl");
            }
        }
        public static string AppBaseUrl
        {
            get
            {
                return GetConfig("AppBaseUrl");
            }
        }
        public static string UpdatePaymentUrl
        {
            get
            {
                return GetConfig("UpdatePaymentUrl");
            }
        }
        public  static string TestMode
        {
            get
            {
                return GetConfig("TestMode");
            }
        }
        public static string TestInterSwitchUrl
        {
            get
            {
                return GetConfig("TestInterSwitchUrl");
            }
        }
        public static string LiveInterSwitchUrl
        {
            get
            {
                return GetConfig("LiveInterSwitchUrl");
            }
        }
    }
}
