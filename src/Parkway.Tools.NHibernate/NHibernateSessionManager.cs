using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Web;
using System.Runtime.Remoting.Messaging;
using NHibernate.Cache;
using System.Data;
//using Parkway.Tools.DotNETExtensions.Threading;
using NHibernate.Tool.hbm2ddl;
using log4net;
using Parkway.Tools.NHibernate.Cfg;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using System.IO;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping;
using NHibernate.Dialect;
using NHibernate.Id;
using NHibernate.SqlTypes;
using System.Linq;
using Iesi.Collections.Generic;
using System.Collections.Concurrent;
using NHibernate.Mapping.ByCode.Conformist;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Parkway.Tools.NHibernate
{
    public sealed class SessionManager
    {
        static ILog logger = LogManager.GetLogger("Parkway.Tools.NHibernate");
        private string _TRANSACTION_KEY;
        private string _SESSION_KEY;
        private string TRANSACTION_KEY
        {
            get
            {
                if (_TRANSACTION_KEY == null)
                {
                    _TRANSACTION_KEY = this.GetType().FullName + "_CONTEXT_TRANSACTION_";
                    if (!string.IsNullOrEmpty(InstanceKey))
                    {
                        _TRANSACTION_KEY += InstanceKey;
                    }
                }
                return _TRANSACTION_KEY;
            }
        }

        private string SESSION_KEY
        {
            get
            {
                if (_SESSION_KEY == null)
                {
                    _SESSION_KEY = this.GetType().FullName + "_SESSION_KEY";
                    if (!string.IsNullOrEmpty(InstanceKey))
                    {
                        _SESSION_KEY += InstanceKey;
                    }
                }
                return _SESSION_KEY;
            }
        }

        private static readonly Object sessionDictionaryLock = new Object();
        private static readonly Object sessionFactoryBuilderLock = new Object();

        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static SessionManager Instance
        {
            get
            {
                return Nested.SessionManager;
            }
        }

        private static int threadCounter = 0;
        private static int sessionThreadCounter = 0;

        //[Obsolete("Using this method could cause concurrency issues if multiple threads are in use. To be able to specify a session context, use GetInstance(string, string, Dictionary<string, string>)")]
        //public static SessionManager GetInstance(string sessionFactoryKey, Dictionary<string, string> sessionFactorySettings)
        //{
        //    return GetInstance(sessionFactoryKey, "default", sessionFactorySettings);
        //}
        public static SessionManager GetInstance(string sessionFactoryKey, string sessionKey, Dictionary<string, string> sessionFactorySettings)
        {
            InternalSessionFactory factory = null;
            if (!sessionFactories.ContainsKey(sessionFactoryKey.ToUpper()))
            {
                factory = InitSessionFactory(sessionFactoryKey.ToUpper(), sessionFactorySettings);
            }
            else
            {
                factory = sessionFactories[sessionFactoryKey.ToUpper()];
            }

            string workingSessionKey = string.Concat(sessionFactoryKey, "_", sessionKey).ToUpper();
            if (!sessionManagers.ContainsKey(workingSessionKey))
            {
                lock (sessionDictionaryLock)
                {
                    try
                    {
                        if (!sessionManagers.ContainsKey(workingSessionKey))
                        {
                            sessionManagers.AddOrUpdate(workingSessionKey, new SessionManager(sessionFactoryKey, factory), (x, y) => y);
                            logger.Debug(string.Format("Session manager [{0}] built successfully", workingSessionKey));
                        }
                        return sessionManagers[workingSessionKey];
                    }
                    catch (Exception ex)
                    {
                        logger.Error(string.Format("Error building Session Manager [{0}]", workingSessionKey), ex);
                        throw;
                    }
                    finally
                    {
                        sessionThreadCounter--;
                    }
                }
            }
            return sessionManagers[workingSessionKey];
        }

        private static string GetWorkingFactoryKey(string inputKey)
        {
            string retVal = null;
            try
            {
                if (Utility.ConfigurationSettings.ContainsKey(retVal = inputKey.ToUpper()))
                    ;
                else if (Utility.ConfigurationSettings.ContainsKey(retVal = string.Concat(inputKey, "_SessionFactory").ToUpper()))
                    ;
                else
                    retVal = null;
            }
            catch (Exception ex) { logger.Error(ex); }
            return retVal;
        }

        [Obsolete("Using this method could cause concurrency issues if multiple threads are in use. To be able to specify a session context, use GetInstance(string, string)")]
        public static SessionManager GetInstance(string sessionFactoryKey)
        {
            return GetInstance(sessionFactoryKey, "default");
        }

        public static SessionManager GetInstance(string sessionFactoryKey, string sessionKey)
        {
            if (sessionFactoryKey == null)
                return Instance;
            else
            {
                string workingKey, workingFactoryKey = GetWorkingFactoryKey(sessionFactoryKey);
                if (string.IsNullOrEmpty(workingFactoryKey))
                {
                    logger.Info(string.Format("Session factory [{0}] was either not configured, or not enabled", sessionFactoryKey));
                    return null;
                }
                else
                {
                    workingKey = string.Concat(workingFactoryKey, "_", sessionKey).ToUpper();
                    if (sessionManagers.ContainsKey(workingKey))
                        return sessionManagers[workingKey];
                    else
                    {
                        //Utility.ConfigurationSettings
                        return GetInstance(workingFactoryKey, sessionKey, Utility.ConfigurationSettings[workingFactoryKey]);
                    }
                }
            }
        }

        public static int CloseAllSessions()
        {
            if (sessionManagers != null) //?? && sessionManagers.Count > 1)
            {
                foreach (SessionManager mgr in sessionManagers.Values)
                {
                    try
                    {
                        mgr.CloseSession();
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(String.Format("Error performing CloseAllSessions for session [{0}]: {1}{2}", mgr.SESSION_KEY, System.Environment.NewLine, ex.Message));
                    }
                }
                return sessionManagers.Count;
            }
            return 0;
        }

        /// <summary>
        /// Initializes the NHibernate session factory upon instantiation.
        /// </summary>
        private SessionManager()
        {
            internalSessionFactory = InitSessionFactory();
        }

        /// <summary>
        /// Initializes the NHibernate session factory upon instantiation.
        /// </summary>
        private SessionManager(string sessionWorkingKey, InternalSessionFactory factory)
        {
            InstanceKey = sessionWorkingKey;
            internalSessionFactory = factory;
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private class Nested
        {
            static Nested() { }
            internal static readonly SessionManager SessionManager =
                new SessionManager();
        }

        #endregion

        private class InternalSessionFactory
        {
            internal String Name { get; set; }
            //public IDictionary<string, string> InstanceSettings = null;
            public IDictionary<String, String> InstanceSettings = null;
            public List<string> MappingAssemblies = new List<string>();
            public List<string> MappingClasses = new List<string>();
            public List<string> MappingFiles = new List<string>();
            internal Boolean UseDefaultConfig { get; set; }
            internal Boolean UseConventionMapping { get; set; }
            internal String BaseEntityClass { get; set; }
            ISessionFactory m_SessionFactory;
            internal ISessionFactory SessionFactory
            {
                get
                {
                    if (m_SessionFactory == null)
                    {
                        lock (sessionFactoryBuilderLock)
                        {
                            if (m_SessionFactory == null)
                            {
                                logger.DebugFormat("...building Session Factory factory [{0}]", Name);
                                m_SessionFactory = Config.BuildSessionFactory();
                                logger.DebugFormat("...Session Factory [{0}] built successfully ", Name);
                            }
                        }
                    }
                    return m_SessionFactory;
                }
            }

            HbmMapping GetMappings(Assembly assembly)
            {
                var mapper = new ModelMapper();

                mapper.AddMappings(assembly.GetExportedTypes());
                var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                return mapping;
            }


            private static IAuxiliaryDatabaseObject CreateHighLowScript(IModelInspector inspector, IEnumerable<Type> entities)
            {
                var script = new StringBuilder(3072);
                script.AppendLine("");
                script.AppendLine("DELETE FROM NextHighVaues;");
                script.AppendLine("ALTER TABLE NextHighVaues ADD EntityName VARCHAR(128) NOT NULL;");
                script.AppendLine("CREATE NONCLUSTERED INDEX IdxNextHighVauesEntity ON NextHighVaues (EntityName ASC);");
                script.AppendLine("GO");
                foreach (var entity in entities.Where(x => inspector.IsRootEntity(x)))
                {
                    script.AppendLine(string.Format("INSERT INTO [NextHighVaues] (EntityName, NextHigh) VALUES ('{0}',1);", entity.Name.ToLowerInvariant()));
                }
                logger.InfoFormat(script.ToString());
#if NOT_RUNNING_ON_4
                return new SimpleAuxiliaryDatabaseObject(script.ToString(), null, new HashedSet<string> { typeof(MsSql2005Dialect).FullName, typeof(MsSql2008Dialect).FullName });
#else
                return new SimpleAuxiliaryDatabaseObject(script.ToString(), null, new HashSet<string> { typeof(MsSql2005Dialect).FullName, typeof(MsSql2008Dialect).FullName });
#endif
            }
            
            class SimpleModelInspector: ExplicitlyDeclaredModel{

                public override bool IsRootEntity(Type type)
                {
                    if (IsRootEntity(type))
                    {
                        return true;
                    }
                    return base.IsRootEntity(type);
                }
            }

            internal Configuration Config
            {
                get
                {
                    Configuration cfg = new Configuration();
                    if (Utility.InheritFromRoot || UseDefaultConfig)
                    {
                        cfg.Configure();
                    }
                    if (UseDefaultConfig)
                        InstanceSettings = cfg.Properties;
                    else
                        cfg.SetProperties(InstanceSettings);




                    //using convention?
                    if (UseConventionMapping)
                    {
                        var mapper = new ConventionModelMapper();
                        var baseEntityType = Type.GetType(BaseEntityClass);
                        var entityTypes = new List<Type>();
                        mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
                        mapper.IsRootEntity((t, declared) => baseEntityType.Equals(t.BaseType));

                        mapper.BeforeMapManyToOne += (insp, prop, map) => map.Column(prop.LocalMember.Name + "ID");
                        mapper.BeforeMapManyToOne += (insp, prop, map) => map.Cascade(Cascade.Persist);
                        //Bags
                        mapper.BeforeMapBag += (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "ID"));
                        mapper.BeforeMapBag += (insp, prop, map) => map.Cascade(Cascade.All);
                        //Sets
                        mapper.BeforeMapSet += (insp, prop, map) => map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "ID"));
                        mapper.BeforeMapSet += (insp, prop, map) => map.Cascade(Cascade.All);


                        var assemblyNamespaceFilters = InstanceSettings["mapping-assemblies-namespace-filters"];
                        var assemblyNamespaceFilterList = assemblyNamespaceFilters.Split(';');
                        int assemblyIndex = -1;

                        var exportedTypes = new List<Type>();
                        var auxObjects = new List<IAuxiliaryDatabaseObject>();
                        var overrideMapClasses = new HashSet<Type>();
                        foreach (string assemblyName in MappingAssemblies)
                        {
                            assemblyIndex++;
                            try
                            {
                                //logger.Debug(string.Format("...adding xml mappings for assembly [{0}]", assemblyName));
                                //cfg.AddAssembly(assemblyName.Trim());
                                {
                                    var assembly = Assembly.Load(assemblyName.Trim());
                                    //HbmMapping hbmMappings = null;
                                    if (assembly != null)
                                    {
                                        exportedTypes = assembly.ExportedTypes.Where(t => assemblyNamespaceFilterList[assemblyIndex] == "[null]" || t.Namespace.EndsWith(assemblyNamespaceFilterList[assemblyIndex])).ToList();

                                        var modelInspector = FindModelInspector(assembly);
                                        if (modelInspector != null)
                                        {
                                            auxObjects.Add(CreateHighLowScript(modelInspector, assembly.GetExportedTypes()));
                                        }


                                        //add other override maps
                                        {
                                            foreach(var t in assembly.ExportedTypes.Where(t=> Utility.IsSubclassOfRawGeneric(typeof(ClassMapping<>), t)))
                                            {
                                                overrideMapClasses.Add(t);
                                            }
                                        }
                                    }
                                }


                                logger.Debug(string.Format("added mapping assembly [{0}]", assemblyName));

                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                //throw;
                            }
                        }

                        //add classes or types if any
                        foreach (string className in MappingClasses)
                        {
                            try
                            {
                                logger.Debug(string.Format("...adding override mapping class [{0}]", className));
                                Type classType = Type.GetType(className);
                                if (classType == null)
                                {
                                    logger.WarnFormat("could not add override mapping class [{0}] because its type could not be resolved.", className);
                                    continue;
                                }
                                overrideMapClasses.Add(classType);
                                logger.Debug(string.Format("added override mapping class [{0}]", className));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                throw;
                            }
                        }

                        //add files if any
                        foreach (string fileName in MappingFiles)
                        {
                            try
                            {
                                logger.Debug(string.Format("...adding mapping file [{0}]", fileName));
                                FileInfo fInfo = new FileInfo(fileName);
                                if (!fInfo.Exists)
                                {
                                    logger.WarnFormat("could not add mapping file [{0}] because the file does not exist.", fileName);
                                    continue;
                                }
                                cfg.AddFile(fInfo);
                                logger.Debug(string.Format("added mapping file [{0}]", fileName));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                throw;
                            }
                        }

                        if (overrideMapClasses != null)
                            foreach (var c in overrideMapClasses)
                            {
                                try
                                {
                                    mapper.AddMapping(c);
                                }
                                catch (Exception ex)
                                {
                                    logger.WarnFormat("Unable to add override map [{0}] due to [{1}]", c, ex.Message);
                                }
                            }

                        var mapping = mapper.CompileMappingFor(exportedTypes);

                        cfg.AddDeserializedMapping(mapping, Name + "-Mapping");
                        if (auxObjects != null && auxObjects.Any())
                        {
                            foreach (var x in auxObjects)
                            {
                                cfg.AddAuxiliaryDatabaseObject(x);
                            }
                        }

                    }
                    //end Convention

                    else
                    {


                        //add assemblies if any
                        foreach (string assemblyName in MappingAssemblies)
                        {
                            try
                            {
                                logger.Debug(string.Format("...adding xml mappings for assembly [{0}]", assemblyName));
                                cfg.AddAssembly(assemblyName.Trim());
                                {
                                    var assembly = Assembly.Load(assemblyName.Trim());
                                    HbmMapping hbmMappings = null;
                                    if (assembly != null)
                                    {
                                        hbmMappings = GetMappings(assembly);
                                        if (hbmMappings.Items != null && hbmMappings.Items.Length > 0)
                                        {
                                            logger.Debug(string.Format("...adding mapping-by-code for assembly [{0}]", assemblyName));
                                            cfg.AddDeserializedMapping(hbmMappings, null);
                                            var modelInspector = FindModelInspector(assembly);
                                            if (modelInspector != null)
                                            {
                                                cfg.AddAuxiliaryDatabaseObject(CreateHighLowScript(modelInspector, assembly.GetExportedTypes()));
                                            }
                                        }
                                    }
                                }

                                logger.Debug(string.Format("added mapping assembly [{0}]", assemblyName));

                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                throw;
                            }
                        }

                        //add classes or types if any
                        foreach (string className in MappingClasses)
                        {
                            try
                            {
                                logger.Debug(string.Format("...adding mapping class [{0}]", className));
                                Type classType = Type.GetType(className);
                                if (classType == null)
                                {
                                    logger.WarnFormat("could not add mapping class [{0}] because its type could not be resolved.", className);
                                    continue;
                                }
                                cfg.AddClass(classType);
                                logger.Debug(string.Format("added mapping class [{0}]", className));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                throw;
                            }
                        }

                        //add files if any
                        foreach (string fileName in MappingFiles)
                        {
                            try
                            {
                                logger.Debug(string.Format("...adding mapping file [{0}]", fileName));
                                FileInfo fInfo = new FileInfo(fileName);
                                if (!fInfo.Exists)
                                {
                                    logger.WarnFormat("could not add mapping file [{0}] because the file does not exist.", fileName);
                                    continue;
                                }
                                cfg.AddFile(fInfo);
                                logger.Debug(string.Format("added mapping file [{0}]", fileName));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message, ex);
                                throw;
                            }
                        }
                    }
                    return cfg;
                }
            }

            private IModelInspector FindModelInspector(Assembly assembly)
            {
                var models = assembly.GetExportedTypes().Where(x => typeof(IModelInspector).IsAssignableFrom(x)).ToList();
                if (models.Count > 0)
                {
                    var instance = models[0].GetConstructor(new Type[] { }).Invoke(null) as IModelInspector;
                    return instance;
                }
                return null;
            }

            internal void CreateDB(String sqlFileName, bool createTables)
            {
                if (!string.IsNullOrEmpty(sqlFileName))
                    logger.InfoFormat("Generating database schema scripts to file {0}", sqlFileName);
                if (createTables)
                    logger.Info("Exporting database schema to the database");
                SchemaExport se = new SchemaExport(Config);
                if (!String.IsNullOrEmpty(sqlFileName))
                    se.SetOutputFile(sqlFileName);
                se.Create(!String.IsNullOrEmpty(sqlFileName), createTables);
                logger.Info("Database schema exported/generated successfully");
            }
        }

        private static IAuxiliaryDatabaseObject OneHiLoRowPerEntityScript(Configuration cfg, String columnName, String columnValue)
        {
            var dialect = Activator.CreateInstance(Type.GetType(cfg.GetProperty(global::NHibernate.Cfg.Environment.Dialect))) as Dialect;
            var script = new StringBuilder();

            script.AppendFormat("ALTER TABLE {0} {1} {2} {3} NULL;\n{4}\nINSERT INTO {0} ({5}, {2}) VALUES (1, '{6}');\n{4}\n", TableHiLoGenerator.DefaultTableName, dialect.AddColumnString, columnName, dialect.GetTypeName(SqlTypeFactory.GetAnsiString(100)), (dialect.SupportsSqlBatches == true ? "GO" : String.Empty), TableHiLoGenerator.DefaultColumnName, columnValue);

            return (new SimpleAuxiliaryDatabaseObject(script.ToString(), null));
        }

        public static void Init()
        {
            NHibernateConfigurationSection cfgSection = Utility.PkToolsConfigurationSection;// System.Configuration.ConfigurationManager.GetSection(Utility.ConfigurationTagName) as NHibernateConfigurationSection;
            int successCounter = 0, count = cfgSection.SessionFactoryConfigs.Count;//= Utility.ConfigurationSettings.Count;
            int delayedCounter = 0, disabledCounter = 0, failureCounter = 0;
            logger.Info(string.Format("Initializing [{0}] session {1}...", count, count == 1 ? "factory" : "factories"));
            //foreach (string sessionFact in Utility.ConfigurationSettings.Keys)
            foreach (SessionFactoryConfigurationElement sessionFact in cfgSection.SessionFactoryConfigs)
            {
                try
                {
                    try
                    {
                        if (!sessionFact.Enabled)
                        {
                            logger.Info(string.Format("Session factory [{0}] was not enabled.", sessionFact.Name));
                            disabledCounter++;
                        }
                        else if (sessionFact.DelayInitialization)
                        {
                            logger.Info(string.Format("Initialization of session factory [{0}] has been delayed.", sessionFact.Name));
                            delayedCounter++;
                        }
                        else
                        {
                            if (!Utility.ConfigurationSettings.ContainsKey(sessionFact.Name.ToUpper()))
                            {
                                logger.Info(string.Format("Session factory [{0}] was not shortlisted...", sessionFact.Name));
                                continue;
                            }
                            logger.Info(string.Format("Initializing session factory [{0}]", sessionFact.Name));
                            InitSessionFactory(sessionFact.Name.ToUpper(), Utility.ConfigurationSettings[sessionFact.Name.ToUpper()]);
                            successCounter++;
                        }
                    }
                    catch (Exception exp)
                    {
                        logger.Error(string.Format("Error initializing session factory [{0}]", sessionFact.Name), exp);
                    }
                }
                catch (Exception ex)
                {
                    failureCounter++;
                    logger.Error(string.Format("Error Initializing Session Factory [{0}]", sessionFact));
                }
            }
            logger.Info(string.Format("{0} session {1} initialized sucessfully,{2} delayed, {3} disabled, {4} failed...", successCounter, successCounter == 1 ? "factory" : "factories", delayedCounter, disabledCounter, failureCounter));
        }

        private static InternalSessionFactory InitSessionFactory()
        {
            string key = "DEFAULT";
            Configuration cfg = new Configuration();
            ISessionFactory sessionFactory = null;
            InternalSessionFactory retVal = new InternalSessionFactory()
            {
                UseDefaultConfig = true,
                Name = key
            };
            sessionFactories.AddOrUpdate(key, retVal, (x, y) => y);
            return retVal;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static InternalSessionFactory InitSessionFactory(string key, Dictionary<string, string> settings)
        {
            if (!sessionFactories.ContainsKey(key))
            {
                try
                {
                    //get the ellement from config
                    IEnumerator i = Utility.PkToolsConfigurationSection.SessionFactoryConfigs.GetEnumerator();
                    SessionFactoryConfigurationElement element = null;
                    while (i.MoveNext())
                    {
                        if ((element = (i.Current as SessionFactoryConfigurationElement)).Name.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                        {
                            break;
                        }
                    }


                    String[] mappingAssemblies = new string[0];
                    if (settings.ContainsKey("mapping-assemblies"))
                    {
                        mappingAssemblies = settings["mapping-assemblies"].Split(';');
                        settings.Remove("mapping-assemblies");
                    }

                    List<string> _MappingAssemblies = new List<string>();
                    foreach (string assembly in mappingAssemblies)
                    {
                        _MappingAssemblies.Add(assembly.Trim());
                    }
                    List<String> _MappingClasses = new List<string>();
                    if (settings.ContainsKey("mapping-classes"))
                    {
                        _MappingClasses = new List<string>(settings["mapping-classes"].Split(';'));
                        settings.Remove("mapping-classes");
                    }

                    List<String> _MappingFiles = new List<string>();
                    if (settings.ContainsKey("mapping-files"))
                    {
                        _MappingFiles = new List<string>(settings["mapping-files"].Split(';'));
                        settings.Remove("mapping-files");
                    }

                    var mappingsEl = element.Mappings;

                    InternalSessionFactory retVal = new InternalSessionFactory()
                    {
                        InstanceSettings = settings,
                        MappingAssemblies = _MappingAssemblies,
                        MappingClasses = _MappingClasses,
                        MappingFiles = _MappingFiles,
                        Name = key,
                        UseConventionMapping = mappingsEl == null ? false : mappingsEl.UseConventionModelMapper,
                        BaseEntityClass = mappingsEl == null ? null : mappingsEl.BaseEntityClass
                    };
                    sessionFactories.AddOrUpdate(key.ToUpper(), retVal, (x, y) => y);
                    logger.Debug(string.Format("Session factory [{0}] initialized", key));

                    if (element.ExportSchemaToDB || !string.IsNullOrEmpty(element.SchemaOutputFile))
                    {
                        retVal.CreateDB(element.SchemaOutputFile, element.ExportSchemaToDB);
                    }
                    return retVal;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Error Initializing Factory [{0}] ", key), ex);
                }
            }
            return sessionFactories[key];
        }

        public void CreateDB(string outputFile, bool script2DB)
        {
            internalSessionFactory.CreateDB(outputFile, script2DB);
        }

        /// <summary>
        /// Allows you to register an interceptor on a new session.  This may not be called if there is already
        /// an open session attached to the HttpContext.  If you have an interceptor to be used, modify
        /// the HttpModule to call this before calling BeginTransaction().
        /// </summary>
        public void RegisterInterceptor(IInterceptor interceptor)
        {
            ISession session = ContextSession;

            if (session != null && session.IsOpen)
            {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }

            GetSession(interceptor);
        }

        public ISession GetSession()
        {
            return GetSession(null);
        }

        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        public ISession GetSession(IInterceptor interceptor)
        {
            ISession session = ContextSession;

            if (session == null || !session.IsOpen)
            {
                if (interceptor != null)
                {
                    session = internalSessionFactory.SessionFactory.OpenSession(interceptor);
                }
                else
                {
                    session = internalSessionFactory.SessionFactory.OpenSession();
                }

                ContextSession = session;
            }

            return session;
        }

        /// <summary>
        /// Opens a new session
        /// </summary>
#warning This Session Must be Disposed Properly!!!
        public ISession NewSession() => internalSessionFactory.SessionFactory.OpenSession();

        /// <summary>
        /// Opens a new session
        /// </summary>
#warning This Session Must be Disposed Properly!!!
        public IStatelessSession NewStatelessSession() => internalSessionFactory.SessionFactory.OpenStatelessSession();

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSession()
        {
            ISession session = ContextSession;

            if (session != null && session.IsOpen)
            {
                try
                {
                    session.Flush();
                }
                catch (Exception ex)
                {
                    logger.Warn(String.Format("Error flushing session for [{0}]: {1}{2}", session.GetSessionImplementation().Connection.ConnectionString, System.Environment.NewLine, ex.Message));
                }
                session.Close();
            }

            ContextSession = null;
        }

        public void BeginTransaction()
        {
            //ITransaction transaction = ContextTransaction;

            //if (transaction == null)
            //{
            //    transaction = GetSession().BeginTransaction();
            //    ContextTransaction = transaction;
            //}
            TransactionBlock.Session = GetSession();
            TransactionBlock.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            TransactionBlock.BeginTransaction(GetSession(), isolationLevel);
        }

        public void CommitTransaction()
        {
            ITransaction transaction = ContextTransaction;

            try
            {
                if (HasOpenTransaction())
                {
                    transaction.Commit();
                    ContextTransaction = null;
                }
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }

        public bool HasOpenTransaction()
        {
            ITransaction transaction = ContextTransaction;

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransaction()
        {
            //ITransaction transaction = ContextTransaction;

            //try
            //{
            //    if (HasOpenTransaction())
            //    {
            //        transaction.Rollback();
            //    }

            //    ContextTransaction = null;
            //}
            //finally
            //{
            //    CloseSession();
            //}
            TransactionBlock.RollbackTransaction();
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ITransaction ContextTransaction
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ITransaction)HttpContext.Current.Items[TRANSACTION_KEY];

                }
                else
                {
                    return (ITransaction)CallContext.GetData(TRANSACTION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[TRANSACTION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(TRANSACTION_KEY, value);
                }
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ISession ContextSession
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ISession)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    return (ISession)CallContext.GetData(SESSION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[SESSION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(SESSION_KEY, value);
                }
            }
        }

        private bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        private InternalSessionFactory internalSessionFactory;
        private static ConcurrentDictionary<string, SessionManager> sessionManagers = new ConcurrentDictionary<string, SessionManager>();
        private static ConcurrentDictionary<string, InternalSessionFactory> sessionFactories = new ConcurrentDictionary<string, InternalSessionFactory>();

        private String InstanceKey = null;



    }
}