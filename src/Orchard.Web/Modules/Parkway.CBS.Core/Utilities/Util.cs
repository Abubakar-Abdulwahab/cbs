using Newtonsoft.Json;
using Parkway.CBS.Core.Configs;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Parkway.CBS.Core.Utilities
{
    public static class UrlExtensions
    {
        public static string AppendModulePrefixToRouteName(this UrlHelper url, string action, object routeValues = null)
        {
            object area = string.Empty;
            bool hasArea = url.RequestContext.RouteData.Values.TryGetValue("area", out area);
            if (hasArea && !string.IsNullOrEmpty((string)area))
            {
                //if the area is found, we need to check if the said area has a prefix
                string prefix = Util.GetRouteNamePrefix((string)area);
                if (!string.IsNullOrEmpty(prefix)) { action = prefix + action; }
            }
            return url.RouteUrl(action, routeValues);
        }

        public static MvcForm AppendModulePrefixToFormRouteName(this HtmlHelper htmlHelper, string routeName, FormMethod method, object htmlAttributes = null)
        {
            object area = string.Empty;
            bool hasArea = htmlHelper.ViewContext.RouteData.Values.TryGetValue("area", out area);
            if (hasArea && !string.IsNullOrEmpty((string)area))
            {
                //if the area is found, we need to check if the said area has a prefix
                string prefix = Util.GetRouteNamePrefix((string)area);
                if (!string.IsNullOrEmpty(prefix)) { routeName = prefix + routeName; }
            }
            return htmlHelper.BeginRouteForm(routeName, method, htmlAttributes);
        }


    }


    public class Util
    {
        public static string SimpleDump<O>(O obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch (Exception exception)
            {
                return string.Format("Could not serialize object type {0} - Exception {1}", typeof(O).FullName, exception.Message);
            }
        }

        /// <summary>
        /// Encrypt input
        /// </summary>
        /// <param name="soup"></param>
        /// <param name="maggi">maggi. If secret is empty, we would use the secret set in the application setting config EncryptionSecret</param>
        /// <returns>string</returns>
        public static string LetsEncrypt(string soup, string maggi = null)
        {
            if (string.IsNullOrEmpty(maggi)) { maggi = AppSettingsConfigurations.EncryptionSecret; }

            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(soup);
            byte[] resultArray = null;
            using (TripleDESCryptoServiceProvider dezzProv = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                {
                    byte[] keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(maggi));

                    dezzProv.Key = keyArray;
                    dezzProv.Mode = CipherMode.ECB;
                    dezzProv.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cryptor = dezzProv.CreateEncryptor();
                    resultArray = cryptor.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public static string StrongRandom()
        {
            using (RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[45];
                cryptoRandomDataGenerator.GetBytes(buffer);
                return Convert.ToBase64String(buffer);
            }
        }


        public static string StrongRandomNoSpecailCharacters()
        {
            Dictionary<string, string> specialChars = new Dictionary<string, string>(12)
            {
                { "/", "47" }, { $"\\", "s" }, { ":", "58" },
                { "*", "42" }, { "?", "63" }, { $"\"", "92" },
                { "<", "60"}, { ">", "62" }, { "|", "124" },
                { "%", "" }, { "'", "39" }, { "`", "96" } 
            };
            StringBuilder strongString = new StringBuilder(StrongRandom());
            foreach (var item in specialChars)
            {
                strongString.Replace(item.Key, item.Value);
            }
            return strongString.ToString();
        }

        /// <summary>
        /// Get base64 hash string value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maggi"></param>
        /// <returns>string</returns>
        public static string OneWaySHA512Hash(string value, string maggi)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(maggi);
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            using (var hasher = new HMACSHA512(keyByte))
            {
                byte[] hashmessage = hasher.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        /// Get base64 hash string value 
        /// </summary>
        /// <param name="soup">Value</param>
        /// <param name="salt">Salt</param>
        /// <returns>string</returns>
        public static string OnWayHashThis(string soup, string salt)
        {
            byte[] preHash = Encoding.UTF32.GetBytes(soup + salt);
            byte[] hash = null;
            using (SHA256 sha = SHA256.Create())
            { hash = sha.ComputeHash(preHash); }
            return Convert.ToBase64String(hash);
        }


        /// <summary>
        /// Decrypt input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">If secret is empty, we would use the secret set in the application setting config EncryptionSecret</param>
        /// <returns>string</returns>
        public static string LetsDecrypt(string input, string key = null)
        {
            if (string.IsNullOrEmpty(key)) { key = AppSettingsConfigurations.EncryptionSecret; }

            byte[] inputArray = Convert.FromBase64String(input);
            byte[] resultArray = null;
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                {
                    byte[] keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                    tripleDES.Key = keyArray;
                    tripleDES.Mode = CipherMode.ECB;
                    tripleDES.Padding = PaddingMode.PKCS7;
                    ICryptoTransform decryptor = tripleDES.CreateDecryptor();
                    resultArray = decryptor.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


        /// <summary>
        /// SHA1 hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string HMACHash(string value, string secret)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(value);
            using (var hmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }


        /// <summary>
        /// Get the next date for the cron expression
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="startDateAndTime"></param>
        /// <returns>DateTime</returns>
        public static DateTime? GetNextDate(string cronExpression, DateTime startDateAndTime)
        {
            return new CRONBaker.CronBaker().GetCronDate(cronExpression, startDateAndTime);
        }


        /// <summary>
        /// Hex value of HMAC 256 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maggi"></param>
        /// <returns></returns>
        public static string HexHMACHash256(string value, string maggi)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(maggi);
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            using (var hasher = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hasher.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }
        }


        /// <summary>
        /// Compute a hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maggi">client secret</param>
        /// <returns>string</returns>
        public static string HMACHash256(string value, string maggi)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(maggi);
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            using (var hasher = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hasher.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }


        public static string GetRouteNamePrefix(string area)
        { return GetModuleNamePrefix(area); }


        /// <summary>
        /// Normalize a phone Number. Remove the country code for NG
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>string</returns>
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("+"))
            {
                //normalize
                phoneNumber = phoneNumber.Remove(0, 1);
            }
            if (phoneNumber.StartsWith("234"))
            {
                phoneNumber = phoneNumber.Remove(0, 3);
                phoneNumber = "0" + phoneNumber;
            }
            return phoneNumber;
        }


        /// <summary>
        /// Get the prefix that is to be attached to the base route name for this area
        /// </summary>
        /// <param name="area">the module area e.g Parkway.CBS.OSGOF.Web</param>
        /// <returns>string | null</returns>
        private static string GetModuleNamePrefix(string area)
        {
            List<string> pathAndValue = new List<string>();
            try
            {
                var remotePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                foreach (XElement excludeElement in XElement.Load($"{remotePath}\\App.xml").Elements("ModuleAreas"))
                {
                    foreach (XElement pathElement in excludeElement.Elements("ModuleRouteNamePrefix"))
                    {
                        if (pathElement.Attribute("Name").Value == area) { return pathElement.Attribute("UrlNamePrefix").Value; }
                    }
                }
                return null;
            }
            catch (Exception) { throw new Exception("Could not validate App xml"); }
        }

        /// <summary>
        /// string concat the values in the collection
        /// </summary>
        /// <param name="collection"></param>
        public static string GetStringConcat(ICollection<int> collection)
        {
            return string.Join("", collection.OrderBy(v => v));
        }


        /// <summary>
        /// Read file contents from stream, and convert/deserialize content to a list of Ms
        /// <para>send in a file name, and the extension. This method reads a file content of a file to stream and deserializes
        /// the string to a list of M's. Would work only for lists of simple objects.
        /// </para>
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns>List{M}</returns>
        public static List<M> GetListOfObjectsFromJSONFile<M>(string fileName, string extension = "json") where M : class
        {
            List<M> modelCollection = null;
            using (StreamReader r = new StreamReader(string.Format("{0}{1}.{2}", GetAppRemotePath(), fileName, extension)))
            {
                string json = r.ReadToEnd();
                modelCollection = JsonConvert.DeserializeObject<List<M>>(json);
            }
            return modelCollection;
        }



        /// <summary>
        /// Get the state config object
        /// </summary>
        /// <returns>CBSStates</returns>
        public static CBSStates StateConfig()
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(CBSStates).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(CBSStates));
            CBSStates stateConfigs = new CBSStates();
            using (StringReader reader = new StringReader(xmlstring))
            { stateConfigs = (CBSStates)serializer.Deserialize(reader); }
            return stateConfigs;
        }


        public static StateConfig.StateConfig GetTenantConfigByVendorName(string vendorCode)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(CBSStates).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(CBSStates));
            CBSStates stateConfigs = new CBSStates();
            using (StringReader reader = new StringReader(xmlstring))
            { stateConfigs = (CBSStates)serializer.Deserialize(reader); }
            return stateConfigs.StateConfig.Where(x => x.CashflowVendorCode == vendorCode).FirstOrDefault();
        }


        /// <summary>
        /// Do email validation
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <param name="compulsory"></param>
        public static void DoEmailValidation(string email, ref List<ErrorModel> errors, string fieldName, bool compulsory)
        {
            //validate email
            if (string.IsNullOrEmpty(email))
            {
                if (!compulsory) { return; }
                errors.Add(new ErrorModel { ErrorMessage = "A valid email address is required.", FieldName = fieldName });
                return;
            }
            if (!email.Contains("@"))
            {
                errors.Add(new ErrorModel { ErrorMessage = "A valid email address is required.", FieldName = fieldName });
                return;
            }
            if (email.Contains(" "))
            {
                errors.Add(new ErrorModel { ErrorMessage = "A valid email address is required.", FieldName = fieldName });
                return;
            }
        }

        /// <summary>
        /// Get the state config object
        /// <para>This gets the config details of the tenant by the site name</para>
        /// </summary>
        /// <returns>StateConfig</returns>
        public static StateConfig.StateConfig GetTenantConfigBySiteName(string tenantName)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(CBSStates).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(CBSStates));
            CBSStates stateConfigs = new CBSStates();
            using (StringReader reader = new StringReader(xmlstring))
            { stateConfigs = (CBSStates)serializer.Deserialize(reader); }
            return stateConfigs.StateConfig.Where(x => x.Value == tenantName).FirstOrDefault();
        }



        /// <summary>
        /// Get the IPPISSettlementConfigTenant config object
        /// </summary>
        /// <returns>IPPISSettlementConfigTenant</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IPPISSettlementConfigTenant GetIPPISSettlementConfig(string tenantName)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(IPPISSettlement).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(IPPISSettlement));
            IPPISSettlement settlemtConfig = new IPPISSettlement();
            using (StringReader reader = new StringReader(xmlstring))
            { settlemtConfig = (IPPISSettlement)serializer.Deserialize(reader); }
            return settlemtConfig.Tenants.Where(x => x.Name == tenantName).Single();
        }


        public static string GetAppRemotePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        }

        public static string GetAppXMLFilePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "App.xml";
        }


        public static string GetOSGOFXMLFilePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "OSGOF.xml";
        }

        /// <summary>
        /// Pad a sring value with max padding zeros.
        /// <para>If length of string is greater than maxpadding, no padding is done and the value is returned as it is.</para> 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxPadding"></param>
        /// <param name="prefix"></param>
        /// <returns>String</returns>
        public static string ZeroPadUp(string value, int maxPadding, string prefix = null)
        {
            string result = value.PadLeft(maxPadding, '0');
            if (!string.IsNullOrEmpty(prefix)) { return prefix + result; }
            return result;
        }


        /// <summary>
        /// Display format for integer value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        public static string DisplayFormat(int value)
        {
            if (value < 1000)
            {
                return value.ToString();
            }
            else if (value < 10000)
            {
                return value.ToString().Substring(0, 1) + "K";
            }
            else if (value < 100000)
            {
                return value.ToString().Substring(0, 2) + "K";
            }
            else if (value < 1000000)
            {
                return value.ToString().Substring(0, 3) + "K";
            }
            else if (value < 10000000)
            {
                return value.ToString().Substring(0, 1) + "M";
            }
            else if (value < 100000000)
            {
                return value.ToString().Substring(0, 2) + "M";
            }
            return value.ToString();
        }


        public static string GetPaymentProviderDescription(int paymentProvider)
        {
            //TODO handle exceptions here
            var statuses = Enum.GetValues(typeof(PaymentProvider)).Cast<PaymentProvider>()
               .ToList().Select(t => new { Name = t.ToDescription(), Value = (int)t });
            return statuses.SingleOrDefault(c => c.Value == paymentProvider).Name;
        }


        /// <summary>
        /// Check if this phone number is valid
        /// <para>Returns false if the validation fails</para>
        /// </summary>
        /// <param name="sPhoneNumber"></param>
        /// <returns>bool</returns>
        public static bool DoPhoneNumberValidation(string sPhoneNumber)
        {
            if (string.IsNullOrEmpty(sPhoneNumber))
            { return false; }
            //validate phone number
            sPhoneNumber = sPhoneNumber.Trim();
            if (sPhoneNumber.Substring(0, 1) == "+")
            {
                sPhoneNumber = sPhoneNumber.Replace("+", string.Empty);
            }
            long phoneNumber = 0;
            bool isANumber = long.TryParse(sPhoneNumber, out phoneNumber);
            if (!isANumber || (sPhoneNumber.Length != 13 && sPhoneNumber.Length != 11))
            { return false; }
            return true;
        }


        public static string GetPaymentChannelDescription(int channel)
        {
            var statuses = Enum.GetValues(typeof(PaymentChannel)).Cast<PaymentChannel>()
               .ToList().Select(t => new { Name = t.ToDescription(), Value = (int)t });
            return statuses.SingleOrDefault(c => c.Value == channel).Name;
        }


        public static string GetBankName(List<BankVM> banks, string bankCode)
        {
            if (string.IsNullOrEmpty(bankCode)) { return string.Empty; }
            return banks.Where(b => b.Code == bankCode.Trim()).Select(b => b.Name).FirstOrDefault();
        }

        public static string GetBankName(string bankCode)
        {
            List<BankVM> banks = GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
            if (string.IsNullOrEmpty(bankCode)) { return string.Empty; }
            return banks.Where(b => b.Code == bankCode.Trim()).Select(b => b.Name).FirstOrDefault();
        }

        /// <summary>
        /// Get the number of pages for the data size off the chunk size
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        public static int Pages(int chunkSize, Int64 dataSize)
        {
            double pageSize = (((double)dataSize) / ((double)chunkSize));
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            return pages;
        }


        /// <summary>
        /// This gives a uniform format for IPPIS request ref
        /// <para>this request reference is used to uniquely identify a record for IPPIS direct assessments.
        /// It is used in the DirectAssessmentBatchRecord table as the Duplicate..
        /// Format: "{0}|{1}|{2}|{3}", agencyCode, payePeriod.Month, payePeriod.Year, PayeAssessmentType.FileUploadForIPPIS
        /// </para>
        /// </summary>
        /// <param name="agencyCode"></param>
        /// <param name="payePeriod"></param>
        /// <returns>string</returns>
        public static string GetFormattedRefForIPPISRef(string agencyCode, DateTime payePeriod)
        {
            return string.Format("{0}|{1}|{2}|{3}", agencyCode, payePeriod.Month, payePeriod.Year, (int)PayeAssessmentType.FileUploadForIPPIS);
        }


        /// <summary>
        /// Get the invoice description for invoices for IPPIS
        /// <para>{Unreconciled} Direct Paye Assessment for {month}/{year}. Agency Code {agency code}</para>
        /// </summary>
        /// <param name="agencyCode"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="unreconciledInvoice"></param>
        /// <returns>string</returns>
        public static string GetInvoiceDescriptionForIPPIS(string agencyCode, int month, int year, bool unreconciledInvoice = false)
        {
            return ((unreconciledInvoice ? "Unreconciled " : string.Empty) + string.Format("Direct Paye Assessment for {0}/{1}. Agency Code {2}", month, year, agencyCode)).Trim();
        }

        public static string GetReferenceDataProcessingStatus(int ProccessStage)
        {
            //TODO handle exceptions here
            var statuses = Enum.GetValues(typeof(ReferenceDataProcessingStages)).Cast<ReferenceDataProcessingStages>()
               .ToList().Select(t => new { Name = t.ToDescription(), Value = (int)t });
            return statuses.SingleOrDefault(c => c.Value == ProccessStage).Name;
        }


        /// <summary>
        /// Do a split of the amounts on the amount paid based 
        /// off the individual percentages 
        /// </summary>
        /// <param name="amounts"></param>
        /// <param name="splitAmount"></param>
        /// <returns>Dictionary{int, decimal}</returns>
        public static Dictionary<long, decimal> DoAmountSplit(Dictionary<long, decimal> amounts, decimal splitAmount)
        {
            var totalAmount = amounts.Sum(x => x.Value);
            var percentages = new Dictionary<long, decimal>(amounts.Count);
            var amountPerPercentages = new Dictionary<long, decimal>(amounts.Count);
            int counter = amounts.Count;
            decimal amountPercentage = 0.00m;

            foreach (var amount in amounts)
            {
                if (counter == 1)
                {
                    amountPercentage = 100 - (percentages.Sum(p => p.Value));
                }
                else
                {
                    amountPercentage = Math.Round(Math.Round((amount.Value / totalAmount), 5, MidpointRounding.ToEven) * 100, 5, MidpointRounding.ToEven);
                    percentages.Add(amount.Key, amountPercentage);
                }

                if (counter == 1)
                {
                    decimal amtprt = Math.Round((amountPercentage * splitAmount), 2, MidpointRounding.ToEven);
                    decimal amtPerPercentage = splitAmount - amountPerPercentages.Sum(ap => ap.Value);
                    amountPerPercentages.Add(amount.Key, amtPerPercentage);
                }
                else
                {
                    decimal amtprt = Math.Round((amountPercentage * splitAmount), 2, MidpointRounding.ToEven);
                    decimal amtPerPercentage = Math.Round(amtprt / 100, 2, MidpointRounding.ToEven);
                    amountPerPercentages.Add(amount.Key, amtPerPercentage);
                }

                --counter;
            }
            return amountPerPercentages;
        }

        /// <summary>
        /// Converts <paramref name="str"/> to an encoded string using <see cref="System.Security.Cryptography.SHA256Managed"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns> Encode hash of <paramref name="str"/>. </returns>
        public static string SHA256ManagedHash(string str)
        {
            var sb = new StringBuilder();
            using (var hasher = new SHA256Managed())
            {
                byte[] hashmessage = hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
                foreach (byte hash in hashmessage)
                {
                    sb.Append(hash.ToString("x2"));
                }
                // to base64
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }

        public static string GetNAGISDataProcessingStatus(int ProccessStage)
        {
            //TODO handle exceptions here
            var statuses = Enum.GetValues(typeof(NagisDataProcessingStages)).Cast<NagisDataProcessingStages>()
               .ToList().Select(t => new { Name = t.ToDescription(), Value = (int)t });
            return statuses.SingleOrDefault(c => c.Value == ProccessStage).Name;
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordGeneratorOptions opts = null)
        {
            if (opts == null) opts = new PasswordGeneratorOptions()
            {
                RequiredLength = 14,
                RequiredUniqueChars = 6,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[]
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
             };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        /// <summary>
        /// Inserts a space before an upper character in a substring
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string InsertSpaceBeforeUpperCase(string str)
        {
            var sb = new StringBuilder();
            char previousChar = char.MinValue; // Unicode '\0'

            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    // If not the first character and previous character is not a space, insert a space before uppercase
                    if (sb.Length != 0 && previousChar != ' ')
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append(c);
                previousChar = c;
            }
            return sb.ToString();
        }


        /// <summary>
        /// validate the length of a stringValue
        /// </summary>
        /// <param name="stringValue">string value</param>
        /// <param name="headerValue">excel header or label value </param>
        /// <param name="allowEmpty">allow empty value</param>
        /// <param name="errorMsg">error message</param>
        /// <param name="minValidLength">minimum string value length</param>
        /// <param name="maxValidLength">maximum string value length</param>
        /// <returns>validated string</returns>
        public static string ValidateStringLength(string stringValue, string headerValue, bool allowEmpty, ref string errorMsg, int minValidLength = 0, int maxValidLength = 0)
        {
            errorMsg = string.Empty;
            if (string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue))
            {
                if (allowEmpty) { return stringValue; }
                errorMsg = string.Format("{0} is empty.", headerValue);
                return stringValue;
            }
            stringValue = stringValue.Trim();

            if (minValidLength > 0)
            {
                if (stringValue.Length < minValidLength)
                {
                    errorMsg = string.Format("{0} value is too small. Enter a valid {0}. Minimum number of characters is {1}", headerValue, minValidLength);
                    return stringValue;
                }
            }

            if (maxValidLength > 0)
            {
                if (stringValue.Length > maxValidLength)
                {
                    errorMsg = string.Format("{0} value is too long. Enter a valid {0}. Maximum number of characters is {1}", headerValue, maxValidLength);
                    return stringValue;
                }
            }
            return stringValue;
        }


        /// <summary>
        /// Render razor view to string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        /// <summary>
        /// Get a content type using a file name with an extension
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>string</returns>
        public static string GetFileContentType(string fileName)
        {
            return MimeMapping.GetMimeMapping(fileName);
        }

        public static string GetPulseSMSTemplateName(string siteName)
        {
            StateConfig.StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            StateConfig.Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.PulseSMSTemplateName.ToString()).FirstOrDefault();

            if (string.IsNullOrEmpty(node?.Value))
            {
                return $"CBS.SMS";
            }
            return node.Value;
        }

        /// <summary>
        /// Compute a hash
        /// </summary>
        /// <param name="signature"></param>
        /// <returns>string</returns>
        public static string Sha256Hash(string signature)
        {
            var bytes = Encoding.UTF8.GetBytes(signature);
            using (var hash = SHA256.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("x2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        /// <summary>
        /// Computes a hash
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns>string</returns>
        public static string Encrypt(string text, string key, string iv)
        {
            byte[] byteArray;
            using (Aes aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(text);
                        byteArray = ms.ToArray();
                    }
                }
            }

            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Decrypts a Hashed string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns>string</returns>
        public static string Decrypt(string text, string key, string iv)
        {
            using (Aes aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] cypherBytes = new byte[text.Length / 2];
                using (var sr = new StringReader(text))
                {
                    for (int i = 0; i < (text.Length / 2); i++)
                    {
                        cypherBytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
                    }
                }

                using (MemoryStream ms = new MemoryStream(cypherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes a hash
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        public static string Encrypt(string text, string key)
        {
            byte[] byteArray;
            using (Aes aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(text);
                        byteArray = ms.ToArray();
                    }
                }
            }

            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Decrypts a Hashed string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        public static string Decrypt(string text, string key)
        {
            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 128;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] cypherBytes = new byte[text.Length / 2];
                using (var sr = new StringReader(text))
                {
                    for (int i = 0; i < (text.Length / 2); i++)
                    {
                        cypherBytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
                    }
                }

                using (MemoryStream ms = new MemoryStream(cypherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates MD5 Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}