using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations
{
    public class AppConfig
    {
        public static string DownloadTemplatesConfigFile
        {
            get { return ConfigurationManager.AppSettings["DownloadTemplatesConfigFile"]; }
        }

    }
}
