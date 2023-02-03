using Parkway.CBS.HangFireInterface.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.CBS.HangFireInterface.Utilities
{
    public class ConfigUtil
    {
        /// <summary>
        /// Get the mail setting config object
        /// <para>This gets the config details by provider</para>
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static Config GetMailConfigByProvider(string providerName)
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(MailSettings).Name) as string);
            XmlSerializer serializer = new XmlSerializer(typeof(MailSettings));
            MailSettings mailSettingConfigs = new MailSettings();
            using (StringReader reader = new StringReader(xmlstring))
            { mailSettingConfigs = (MailSettings)serializer.Deserialize(reader); }
            return mailSettingConfigs.Config.Where(x => x.Provider == providerName).FirstOrDefault();
        }
    }

}
