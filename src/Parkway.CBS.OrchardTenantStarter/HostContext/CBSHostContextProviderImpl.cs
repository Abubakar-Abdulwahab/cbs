using Orchard;
using Orchard.Host;
using Orchard.HostContext;
using Orchard.Parameters;
using Parkway.CBS.OrchardTenantStarter.Host;
using Parkway.CBS.OrchardTenantStarter.HostContext.Contracts;
using Parkway.CBS.Payee.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Parkway.CBS.OrchardTenantStarter.HostContext
{
    class CBSHostContextProviderImpl : ICBSHostContextProvider 
    {
        private readonly string[] _args;
        private TextWriter _output;
        private TextReader _input;
        private IPPISMarshalled obj;

        public CBSHostContextProviderImpl()
        {
            _input = Console.In;
            _output = Console.Out;
            obj = new IPPISMarshalled { ImplementingClassName = "ctor" };
            _args = new string[] { };
        }


        [SecurityCritical]
        public CBSTenantHostContext CreateContext()
        {
            var context = new CBSTenantHostContext { RetryResult = CommandReturnCodes.Retry };
            Initialize(context);
            return context;
        }


        private void Initialize(CBSTenantHostContext context)
        {
            context.Arguments = new OrchardParameters
            {
                Arguments = new List<string>(),
            };

            context.Logger = new Logger(context.Arguments.Verbose, _output);

            if (string.IsNullOrEmpty(context.Arguments.VirtualPath))
                context.Arguments.VirtualPath = "/";
            LogInfo(context, "Virtual path: \"{0}\"", context.Arguments.VirtualPath);

            if (string.IsNullOrEmpty(context.Arguments.WorkingDirectory))
                context.Arguments.WorkingDirectory = ConfigurationManager.AppSettings["PathToAppDirectory"];

            LogInfo(context, "Working directory: \"{0}\"", context.Arguments.WorkingDirectory);

            LogInfo(context, "Detecting orchard installation root directory...");
            context.OrchardDirectory = GetOrchardDirectory(context.Arguments.WorkingDirectory);
            LogInfo(context, "Orchard root directory: \"{0}\"", context.OrchardDirectory.FullName);

            LogInfo(context, "Creating ASP.NET AppDomain for command agent...");
            context.CBSTenantHost = CreateWorkerAppDomainWithHost(context.Arguments.VirtualPath, context.OrchardDirectory.FullName, typeof(CBSTenantHost));

            LogInfo(context, "Starting Orchard session");
            context.StartSessionResult = context.CBSTenantHost.StartSession();
        }


        public CBSTenantHost CreateWorkAppDomain(CBSTenantHostContext context)
        {
            return (CBSTenantHost)CreateWorkerAppDomainWithHost(context.Arguments.VirtualPath, context.OrchardDirectory.FullName, typeof(CBSTenantHost));
        }


        private DirectoryInfo GetOrchardDirectory(string directory)
        {
            for (var directoryInfo = new DirectoryInfo(directory); directoryInfo != null; directoryInfo = directoryInfo.Parent)
            {
                if (!directoryInfo.Exists)
                {
                    throw new ApplicationException(string.Format("Directory \"{0}\" does not exist", directoryInfo.FullName));
                }

                // We look for 
                // 1) .\web.config
                // 2) .\bin\Orchard.Framework.dll
                var webConfigFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "web.config"));
                if (!webConfigFileInfo.Exists)
                    continue;

                var binDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, "bin"));
                if (!binDirectoryInfo.Exists)
                    continue;

                var orchardFrameworkFileInfo = new FileInfo(Path.Combine(binDirectoryInfo.FullName, "Orchard.Framework.dll"));
                if (!orchardFrameworkFileInfo.Exists)
                    continue;

                return directoryInfo;
            }

            throw new ApplicationException(
                string.Format("Directory \"{0}\" doesn't seem to contain an Orchard installation", new DirectoryInfo(directory).FullName));
        }


        private static CBSTenantHost CreateWorkerAppDomainWithHost(string virtualPath, string physicalPath, Type hostType)
        {
            var clientBuildManager = new ClientBuildManager(virtualPath, physicalPath);
            // Fix for https://github.com/OrchardCMS/Orchard/issues/1749
            // By forcing the CBM to build App_Code, etc, we ensure that the ASP.NET BuildManager
            // is in a state where it can safely (i.e. in a multi-threaded safe way) process
            // multiple concurrent calls to "GetCompiledAssembly".
            clientBuildManager.CompileApplicationDependencies();
            return (CBSTenantHost)clientBuildManager.CreateObject(hostType, false);
        }


        private void LogInfo(CommandHostContext context, string format, params object[] args)
        {
            if (context.Logger != null)
                context.Logger.LogInfo(format, args);
        }



        [SecurityCritical]
        public void Shutdown(CBSTenantHostContext context)
        {
            try
            {
                if (context.CommandHost != null)
                {
                    LogInfo(context, "Shutting down Orchard session...");
                    context.CBSTenantHost.StopSession();
                }
            }
            catch (AppDomainUnloadedException)
            {
                LogInfo(context, "   (AppDomain already unloaded)");
            }

            if (context.CBSTenantHost != null)
            {
                LogInfo(context, "Shutting down ASP.NET AppDomain...");
                ApplicationManager.GetApplicationManager().ShutdownAll();
            }
        }
    }
}
