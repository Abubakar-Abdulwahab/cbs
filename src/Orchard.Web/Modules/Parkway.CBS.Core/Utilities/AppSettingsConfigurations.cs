using System.Configuration;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Utilities
{

    public static class AppSettingsConfigurations
    {
        public static string EncryptionSecret
        { get { return GetSettingsValue("EncryptionSecret"); } }

        public static string AESEncryptionSecret
        { get { return GetSettingsValue("AESEncryptionSecret"); } }

        /// <summary>
        /// Get the app settings value for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string</returns>
        public static string GetSettingsValue(AppSettingEnum key)
        { return ConfigurationManager.AppSettings[key.ToString()]; }


        public static string GetSettingsValue(string key)
        { return ConfigurationManager.AppSettings[key]; }
    }
}