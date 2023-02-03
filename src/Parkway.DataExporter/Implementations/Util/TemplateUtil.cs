using Parkway.DataExporter.Implementations.Enums;
using Parkway.DataExporter.Implementations.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Parkway.DataExporter.Implementations.Util
{
    public class TemplateUtil
    {
        public static String HTMLTemplateFor(string name, string tenants)
        {
            return GetTemplatePath(TemplateType.HTML, name, tenants);
        }


        /// <summary>
        /// Get template with the name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tenants"></param>
        /// <returns>Template</returns>
        public static Template RazorTemplateFor(string name, string tenants = "Default")
        {
            return GetTemplate(TemplateType.Razor, name, tenants);
        }


        public static IDictionary<string, string> KeyValueTemplateFor(string name, string tenants = "Default")
        {
            var template = GetTemplate(TemplateType.KeyValue, name, tenants);
            return template.Fields.ToDictionary(x => x.Key, x => x.Value);
        }


        public static Template KeyValueReturnTemplateFor(string name, string tenants)
        {
            return GetTemplate(TemplateType.KeyValue, name, tenants);
        }


        static string GetTemplatePath(TemplateType type, string name, string tenants)
        {
            var template = GetTemplate(type, name, tenants);
            return System.Web.Hosting.HostingEnvironment.MapPath(string.Format("{0}/{1}", template.BasePath, template.File));
        }


        /// <summary>
        /// This method gets the template with the given template type and name.
        /// If the tenants name is not given the value Default is used
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="tenants"></param>
        /// <returns>Template</returns>
        static Template GetTemplate(TemplateType type, string name, string tenants = "Default")
        {
            var config = LoadTemplates();
            var templates = config.Groups.Single(x => x.Type.Equals(type));
            return templates.Collections.OrderByDescending(x => x.Tenants).FirstOrDefault(x =>
                (x.Tenants.Equals(tenants, StringComparison.InvariantCultureIgnoreCase)) &&
                    x.Templates.Any(z => z.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                ).Templates.Single(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        }


        static ExportTemplates LoadTemplates()
        {
            string filePath = HttpContext.Current.Server.MapPath(AppConfig.DownloadTemplatesConfigFile);
            FileInfo fileInfo = new FileInfo(filePath);
            ExportTemplates config;

            using (var reader = new StreamReader(fileInfo.FullName))
            {
                XmlSerializer Deserializer = new XmlSerializer(typeof(ExportTemplates));
                config = (ExportTemplates)Deserializer.Deserialize(reader);
            }
            return config;
        }

    }
}
