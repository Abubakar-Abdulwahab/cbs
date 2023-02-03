using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl
{
    public abstract class BaseBillingImpl
    {
        public ILogger Logger { get; set; }
        protected readonly ICoreCollectionService _coreCollectionService;

        public BaseBillingImpl(ICoreCollectionService coreCollectionService)
        {
            Logger = NullLogger.Instance;
            _coreCollectionService = coreCollectionService;
        }


        protected string GetCatText(TaxEntityCategory category)
        {
            bool ifVowel = new string[5] { "a", "e", "i", "o", "u" }.Contains(category.Name.Substring(0, 1).ToLower());
            return ifVowel ? "An " + category.Name : "A " + category.Name;
        }



        /// <summary>
        /// get tax entity and header obj
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected dynamic GetTaxEntity(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            TaxEntity entity = null;
            HeaderObj headerObj = new HeaderObj { ShowSignin = true };
            if (user != null && user.Entity != null) { entity = user.Entity; headerObj.IsLoggedIn = true; headerObj.DisplayText = user.Name; }

            if (entity == null)
            {
                if (processStage.ProceedWithInvoiceGenerationVM != null)
                {
                    if (processStage.ProceedWithInvoiceGenerationVM.FromTaxProfileSetup)
                    { entity = processStage.ProceedWithInvoiceGenerationVM.Entity; }
                }
            }
            return new { TaxEntity = entity, HeaderObj = headerObj };
        }


        /// <summary>
        /// Get the list of additional form details
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns>List{AdditionalDetails}</returns>
        protected static List<AdditionalDetails> GetAdditionalFormFields(GenerateInvoiceStepsModel processStage)
        {
            //throw new Exception("DO SOMETHING HERE");
            return new List<AdditionalDetails>();
        }


        /// <summary>
        /// Converts the samount string to decimal value
        /// </summary>
        /// <param name="model"></param>
        protected decimal ConvertAmountStringValue(string samount)
        {
            if (string.IsNullOrEmpty(samount)) { return 0.0m; }
            //split by ,
            var segs = samount.Trim().Split(',');
            var segnospace = segs.Select(v => v.Trim());
            var samountConcat = string.Join(string.Empty, segnospace);
            //try parse
            decimal amount = 0.00m;
            decimal.TryParse(samountConcat, out amount);
            return amount;
        }

        /// <summary>
        /// Get the list of LGAs
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns>Dictionary{string, string}</returns>
        protected Dictionary<string, string> GetLGAs(string siteName)
        {
            //get location of LAGs xml file
            Dictionary<string, string> lgaAndValue = new Dictionary<string, string>();
            try
            {
                //for some site name we could have space in e.g Awka Ibom
                var siteArr = siteName.Split(' ');
                siteName = string.Join("", siteArr);
                var remotePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                foreach (XElement stateElement in XElement.Load($"{remotePath}\\LGAs.xml").Elements(siteName))
                {
                    foreach (XElement lgaElement in stateElement.Elements("lga"))
                    {
                        lgaAndValue.Add(lgaElement.Attribute("name").Value, lgaElement.Attribute("value").Value);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error trying to get the lgas for " + siteName);
            }
            return lgaAndValue;
        }
    }
}