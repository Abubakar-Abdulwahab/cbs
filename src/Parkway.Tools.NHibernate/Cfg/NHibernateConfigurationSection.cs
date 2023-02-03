using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Parkway.Tools.NHibernate.Cfg
{
    public class NHibernateConfigurationSection : System.Configuration.ConfigurationSection
    {
        public NHibernateConfigurationSection()
        {
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SessionFactoryConfigurationCollection SessionFactoryConfigs
        {
            get
            {
                return (SessionFactoryConfigurationCollection)base[""];
            }
        }
    }

    //[ConfigurationCollection(typeof(SessionFactoryConfigurationElement),CollectionType= ConfigurationElementCollectionType.BasicMap)]
    public class SessionFactoryConfigurationCollection : System.Configuration.ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get
            {
                return "SessionFactory";
            }
        }

        #region Indexers
        public SessionFactoryConfigurationElement this[int index]
        {
            get { return base.BaseGet(index) as SessionFactoryConfigurationElement; }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public SessionFactoryConfigurationElement this[string name]
        {
            get { return base.BaseGet(name) as SessionFactoryConfigurationElement; }
        }
        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new SessionFactoryConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as SessionFactoryConfigurationElement).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        #endregion

        public void Add(SessionFactoryConfigurationElement element)
        {
            base.BaseAdd(element);
        }
    }

    public sealed class SessionFactoryConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue=true)]
        public Boolean Enabled
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["enabled"]);
                }
                catch { return false; }
            }
            set
            {
                this["enabled"] = value;
            }
        }

        [ConfigurationProperty("delay-initialization", IsRequired = false, DefaultValue=false)]
        public Boolean DelayInitialization
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["delay-initialization"]);
                }
                catch { return false; }
            }
            set
            {
                this["delay-initialization"] = value;
            }
        }

        [ConfigurationProperty("schema-output-file", IsRequired = false)]
        public String SchemaOutputFile
        {
            get
            {
                return this["schema-output-file"] as string;
            }
            set
            {
                this["schema-output-file"] = value;
            }
        }


        [ConfigurationProperty("export-schema-to-db", IsRequired = false, DefaultValue = false)]
        public Boolean ExportSchemaToDB
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["export-schema-to-db"]);
                }
                catch { return false; }
            }
            set
            {
                this["export-schema-to-db"] = value;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SessionFactorySettingsCollection Settings
        {
            get
            {
                return this[""] as SessionFactorySettingsCollection;
            }
        }

        [ConfigurationProperty("MappingAssemblies",IsRequired = false)]
        public MappingAssembliesElement MappingAssemblies
        {
            get
            {
                return base["MappingAssemblies"] as MappingAssembliesElement;
            }
            set { base["MappingAssemblies"] = value; }
        }

        [ConfigurationProperty("Mappings",IsRequired = false)]
        public MappingsElement Mappings
        {
            get
            {
                return base["Mappings"] as MappingsElement;
            }
            set { base["Mappings"] = value; }
        }
    }

    public class SessionFactorySettingsCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new FactorySettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactorySettingElement)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "property";
            }
        }

        public FactorySettingElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as FactorySettingElement;
            }
        }

        public new FactorySettingElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as FactorySettingElement;
            }
        }


        public void Add(FactorySettingElement element)
        {
            base.BaseAdd(element);
        }
    }

    public class FactorySettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public String Name
        {
            get
            {
                return this["name"] as String;
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = false)]
        public String Value
        {
            get
            {
                return this["value"] as String;
            }
            set
            {
                this["value"] = value;
            }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }

    public class MappingAssembliesElement : ConfigurationElement
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public MappingAssemblyCollection MappingAssemblies
        {
            get
            {
                return this[""] as MappingAssemblyCollection;
            }
            set
            {
                this[""] = value;
            }
        }
    }

    public class MappingAssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MappingAssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MappingAssemblyElement)element).Assembly;
        }

        protected override string ElementName
        {
            get
            {
                return "mapping";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }


        public MappingAssemblyElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as MappingAssemblyElement;
            }
        }

        public new MappingAssemblyElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as MappingAssemblyElement;
            }
        }


        public void Add(MappingAssemblyElement element)
        {
            base.BaseAdd(element);
        }
    }
    public class MappingAssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = false)]
        public String Assembly
        {
            get
            {
                return this["assembly"] as String;
            }
            set
            {
                this["assembly"] = value;
            }
        }

        [ConfigurationProperty("file", IsRequired = false)]
        public String File
        {
            get
            {
                return this["file"] as String;
            }
            set
            {
                this["file"] = value;
            }
        }
        [ConfigurationProperty("resource", IsRequired = false)]
        public String Resource
        {
            get
            {
                return this["resource"] as String;
            }
            set
            {
                this["resource"] = value;
            }
        }
        public override string ToString()
        {
            return this.Assembly;
        }
    }



    #region New Mappings Element

    public class MappingsElement : ConfigurationElement
    {

        [ConfigurationProperty("use-convention-model-mapper", IsRequired = false, DefaultValue = false)]
        public Boolean UseConventionModelMapper
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["use-convention-model-mapper"]);
                }
                catch { return false; }
            }
            set
            {
                this["use-convention-model-mapper"] = value;
            }
        }

        [ConfigurationProperty("base-entity-class", IsRequired = false)]
        public String BaseEntityClass
        {
            get
            {
                return this["base-entity-class"] as string;
            }
            set
            {
                this["base-entity-class"] = value;
            }
        }
        [ConfigurationProperty("assemblies", IsDefaultCollection = true)]
        public AssemblyCollection Assemblies
        {
            get
            {
                return this["assemblies"] as AssemblyCollection;
            }
            set
            {
                this["assemblies"] = value;
            }
        }

        [ConfigurationProperty("classes", IsDefaultCollection = true)]
        public ClassCollection Classes
        {
            get
            {
                return this["classes"] as ClassCollection;
            }
            set
            {
                this["classes"] = value;
            }
        }

        [ConfigurationProperty("files", IsDefaultCollection = true)]
        public FileCollection Files
        {
            get
            {
                return this["files"] as FileCollection;
            }
            set
            {
                this["files"] = value;
            }
        }
    }

    public class AssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyElement)element).Name;
        }

        protected override string ElementName
        {
            get
            {
                return "mappingassembly";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public AssemblyElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as AssemblyElement;
            }
        }

        public new AssemblyElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as AssemblyElement;
            }
        }

        public void Add(AssemblyElement element)
        {
            base.BaseAdd(element);
        }
    }

    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public String Name
        {
            get
            {
                return this["name"] as String;
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("namespaceFilter", IsRequired = false)]
        public String NamespaceFilter
        {
            get
            {
                return this["namespaceFilter"] as String;
            }
            set
            {
                this["namespaceFilter"] = value;
            }
        }
        //[ConfigurationProperty("classSufixFilter", IsRequired = false)]
        //public String ClassSufixFilter
        //{
        //    get
        //    {
        //        return this["classSufixFilter"] as String;
        //    }
        //    set
        //    {
        //        this["classSufixFilter"] = value;
        //    }
        //}

        public override string ToString()
        {
            var name = this.Name;
            //if (!String.IsNullOrWhiteSpace(NamespaceFilter))
            //{
            //    name += String.Format(" Namespace[{0}]", NamespaceFilter);
            //}

            //if (!String.IsNullOrWhiteSpace(ClassSufixFilter))
            //{
            //    name += String.Format(" Classes-Ending-With[{0}]", ClassSufixFilter);
            //}
            return name;
        }
    }

    public class FileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).Name;
        }

        protected override string ElementName
        {
            get
            {
                return "mappingfile";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public AssemblyElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as AssemblyElement;
            }
        }

        public new AssemblyElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as AssemblyElement;
            }
        }


        public void Add(FileElement element)
        {
            base.BaseAdd(element);
        }
    }

    public class FileElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public String Name
        {
            get
            {
                return this["name"] as String;
            }
            set
            {
                this["name"] = value;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class ClassCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClassElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClassElement)element).Name;
        }

        protected override string ElementName
        {
            get
            {
                return "classfile";
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public AssemblyElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as AssemblyElement;
            }
        }

        public new AssemblyElement this[string key]
        {
            get
            {
                return base.BaseGet(key) as AssemblyElement;
            }
        }

        public void Add(ClassElement element)
        {
            base.BaseAdd(element);
        }
    }

    public class ClassElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false)]
        public String Name
        {
            get
            {
                return this["name"] as String;
            }
            set
            {
                this["name"] = value;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }



    }

    #endregion
}