using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Cfg;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Runtime.Remoting.Messaging;
using System.Configuration;
using Parkway.Tools.NHibernate.Cfg;
using log4net;

namespace Parkway.Tools.NHibernate
{
    internal class Utility
    {
        static ILog logger = LogManager.GetLogger("Parkway.Tools.NHibernate");
        public static bool InheritFromRoot
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings["Parkway.Tools.NHibernate.InheritFromRoot"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        static internal readonly string ConfigurationTagName = "parkway.tools.nhibernate";
        static readonly object _ConfigurationSettingsLock = new object();
        static Dictionary<string, Dictionary<string, string>> _ConfigurationSettings = null;

        public static NHibernateConfigurationSection PkToolsConfigurationSection
        {
            get
            {
                NHibernateConfigurationSection cfgSection = ConfigurationManager.GetSection(ConfigurationTagName) as NHibernateConfigurationSection;
                if (cfgSection == null)
                {
                    logger.FatalFormat("Parkway NH section [{0}] was not found in the configuration file.", ConfigurationTagName);
                }
                return cfgSection;
            }
        }

        public static Dictionary<string, Dictionary<string, string>> ConfigurationSettings
        {
            get
            {
                if (_ConfigurationSettings == null)
                {
                    lock (_ConfigurationSettingsLock)
                    {
                        if (_ConfigurationSettings == null)
                        {
                            var _cfgSettings = new Dictionary<string, Dictionary<string, string>>();
                            Dictionary<string, string> factorySettings = null;
                            NHibernateConfigurationSection cfgSection = PkToolsConfigurationSection;// ConfigurationManager.GetSection(ConfigurationTagName) as NHibernateConfigurationSection;
                            if (cfgSection == null)
                            {
                                logger.Info(string.Format("Section [{0}] does not exit", ConfigurationTagName));
                            }
                            else
                            {
                                foreach (SessionFactoryConfigurationElement factoryElement in cfgSection.SessionFactoryConfigs)
                                {
                                    if (!factoryElement.Enabled)
                                    {
                                        logger.Info(string.Format("Not registering session factory [{0}], factory was not enabled ", factoryElement.Name));
                                        continue;
                                    }
                                    factorySettings = new Dictionary<string, string>();
                                    string mappingAssemblies = string.Empty;
                                    //string mappingAssemblyNamespaces = string.Empty;
                                    string mappingAssemblyNamespaceFilters = string.Empty;
                                    string mappingClasses = string.Empty;
                                    string mappingFiles = string.Empty;

                                    foreach (FactorySettingElement setting in factoryElement.Settings)
                                    {
                                        if (setting.Name == "mapping-assemblies")
                                        {
                                            mappingAssemblies += setting.Value.Trim(';', ' ') + ';';
                                            //mappingAssemblyNamespaces+="[null];";
                                            mappingAssemblyNamespaceFilters += "[null];";
                                        }
                                        else
                                            factorySettings.Add(setting.Name, setting.Value);
                                    }
                                    if (factoryElement.MappingAssemblies != null)
                                        foreach (MappingAssemblyElement mappingElement in factoryElement.MappingAssemblies.MappingAssemblies)
                                        {
                                            mappingAssemblies += mappingElement.Assembly + ';';
                                            //mappingAssemblyNamespaces += "[null];";
                                            mappingAssemblyNamespaceFilters += "[null];";
                                        }
                                    //mappingAssemblies = mappingAssemblies.Trim(';', ' ');

                                    if (factoryElement.Mappings != null)
                                    {
                                        //add mapping assemblies
                                        foreach (AssemblyElement assemblyElement in factoryElement.Mappings.Assemblies)
                                        {
                                            mappingAssemblies += assemblyElement.Name + ';';
                                            //mappingAssemblyNamespaces += String.IsNullOrWhiteSpace(assemblyElement.Namespace) ? "[null]" : assemblyElement.Namespace + ";";
                                            mappingAssemblyNamespaceFilters += String.IsNullOrWhiteSpace(assemblyElement.NamespaceFilter) ? "[null]" : assemblyElement.NamespaceFilter + ";";

                                        }
                                        mappingAssemblies = mappingAssemblies.Trim(';', ' ');
                                        //mappingAssemblyNamespaces = mappingAssemblyNamespaces.Trim(';', ' ');
                                        mappingAssemblyNamespaceFilters = mappingAssemblyNamespaceFilters.Trim(';', ' ');

                                        //add mapping classes
                                        foreach (ClassElement classElement in factoryElement.Mappings.Classes)
                                        {
                                            mappingClasses += classElement.Name + ';';
                                        }
                                        mappingClasses = mappingClasses.Trim(';', ' ');

                                        //add mapping files
                                        foreach (FileElement fileElement in factoryElement.Mappings.Files)
                                        {
                                            mappingFiles += fileElement.Name + ';';
                                        }
                                        mappingFiles = mappingFiles.Trim(';', ' ');
                                    }

                                    if (!String.IsNullOrEmpty(mappingAssemblies))
                                    {
                                        factorySettings.Add("mapping-assemblies", mappingAssemblies.Trim(';', ' '));
                                        //factorySettings.Add("mapping-assemblies-namespaces", mappingAssemblyNamespaces.Trim(';', ' '));
                                        factorySettings.Add("mapping-assemblies-namespace-filters", mappingAssemblyNamespaceFilters.Trim(';', ' '));
                                    }

                                    if (!String.IsNullOrEmpty(mappingClasses))
                                        factorySettings.Add("mapping-classes", mappingClasses.Trim(';', ' '));

                                    if (!String.IsNullOrEmpty(mappingFiles))
                                        factorySettings.Add("mapping-files", mappingFiles.Trim(';', ' '));

                                    _cfgSettings.Add(factoryElement.Name.ToUpper(), factorySettings);
                                }

                                _ConfigurationSettings = _cfgSettings;
                            }
                        }
                    }
                }
                return _ConfigurationSettings;
            }
        }

        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}