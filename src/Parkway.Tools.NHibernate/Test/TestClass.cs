using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Parkway.Tools.NHibernate.Cfg;
using System.Configuration;

namespace Parkway.Tools.NHibernate.Test
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        void CreateConfig()
        {

            System.Configuration.Configuration config =
              ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
             MappingAssembliesElement mappingAssembliesElement = new MappingAssembliesElement();
             MappingAssemblyCollection mapColl = new MappingAssemblyCollection();
             MappingAssemblyElement mapElement = new MappingAssemblyElement()
             {
                 Assembly = "test.dll",
                 File = "test.txt",
                 Resource = "xtra.res"
             };
             mapColl.Add(mapElement);
             mappingAssembliesElement.MappingAssemblies = mapColl;

            NHibernateConfigurationSection section = new NHibernateConfigurationSection();
            SessionFactoryConfigurationElement sessFactElement = new SessionFactoryConfigurationElement()
            {
                DelayInitialization = false,
                Enabled = true,
                ExportSchemaToDB = true,
                MappingAssemblies = mappingAssembliesElement,
                Name = "Test_SessionFactory",
                SchemaOutputFile = "'C:\\test\\testsql.sql"
            };
            
            #region New Elements

            AssemblyElement assembly = new AssemblyElement() { Name = "MassPAY_NNPC_HQ" };
            AssemblyElement assembly2 = new AssemblyElement() { Name = "MassPAY_NNPC_GT" };
            AssemblyCollection assemblies = new AssemblyCollection();
            assemblies.Add(assembly);
            assemblies.Add(assembly2);

            ClassElement class1 = new ClassElement() { Name = "MassPAY_NNPC_HQ.Common.Data.PaymentItem" };
            ClassElement class2 = new ClassElement() { Name = "MassPAY_NNPC_GT.Common.Data.PaymentBatch" };
            ClassCollection classes = new ClassCollection();
            classes.Add(class1);
            classes.Add(class2);

            FileElement file = new FileElement() { Name = "MassPAY_NNPC_HQ.hbm.xml" };
            FileElement file2 = new FileElement() { Name = "MassPAY_NNPC_GT.hbm.xml" };
            FileCollection files = new FileCollection();
            files.Add(file);
            files.Add(file2);

            MappingsElement mappingsElement = new MappingsElement() { Classes = classes, Assemblies = assemblies, Files = files };
            sessFactElement.Mappings = mappingsElement;
            #endregion

            section.SessionFactoryConfigs.Add(sessFactElement);

            section.SectionInformation.ForceSave = true;
            section.SectionInformation.ForceDeclaration(true);
            config.Sections.Add("TestPkwy.Cfg", section);
            config.Save(ConfigurationSaveMode.Full, true);
            //ForceDeclaration(true)
        }
    }
}
