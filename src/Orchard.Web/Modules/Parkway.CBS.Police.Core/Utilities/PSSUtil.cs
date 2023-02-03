using System.Linq;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Parkway.CBS.Police.Core.Utilities
{
    public class PSSUtil
    {
        public static string GetOfficerExcelFilePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "officers.xlsx";
        }

        public static string GetChartSheetExcelFilePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "chartsheet.xlsx";
        }


        /// <summary>
        /// Get fee confirmation call back URL for requests
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="queryParam"></param>
        /// <returns>string</returns>
        public static string GetURLForFeeConfirmation(string siteName, string queryParam)
        {
            return Util.GetTenantConfigBySiteName(siteName).Node.Where(x => x.Key == TenantConfigKeys.RequestFeeAPICallBack.ToString()).FirstOrDefault().Value + "/?requestToken=" + queryParam;
        }

        /// <summary>
        /// Returns the base url from the state configuration
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static string GetBaseUrl(string siteName)
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).FirstOrDefault();
            return node.Value;
        }

        /// <summary>
        /// Returns the base url from the state configuration
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static string GetAdminBaseUrl(string siteName)
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.AdminBaseURL.ToString()).FirstOrDefault();
            return node.Value;
        }

        /// <summary>
        /// Checks if email notification is configured for the tenant
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public static bool IsEmailEnabled(string siteName)
        {
            bool canSendNotification = false;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsEmailEnabled.ToString()).FirstOrDefault();

            if (!string.IsNullOrEmpty(node?.Value))
            {
                bool.TryParse(node.Value, out bool isEmailEnabled);
                if (isEmailEnabled)
                {
                    canSendNotification = true;
                }
            }
            return canSendNotification;
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Outsource Global analytics dashboard AES encryption
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="encryptionKey"></param>
        /// <returns>string</returns>
        public static string AnalyticsDashboardEncrypt(string encryptString, string encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        /// <summary>
        /// Outsource Global analytics dashboard AES decryption
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="encryptionKey"></param>
        /// <returns>string</returns>
        public static string AnalyticsDashboardDecrypt(string cipherText, string encryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}