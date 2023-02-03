using Orchard;
using Orchard.Host;
using Orchard.HostContext;
using Orchard.Parameters;
using Parkway.CBS.OrchardTenantStarter.Host.X;
using Parkway.CBS.OrchardTenantStarter.HostContext.X.Contracts;
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

namespace Parkway.CBS.OrchardTenantStarter.HostContext.X
{
    public class CBSHostContextProviderImplX : ICBSHostContextProviderX
    {
        private readonly string[] _args;
        private TextWriter _output;
        private TextReader _input;

        public CBSHostContextProviderImplX()
        {
            _input = Console.In;
            _output = Console.Out;
            _args = new string[] { };
        }


        //[SecurityCritical]
        ////public CommandHostContext CreateContext()
        //public H CreateContext<H>() where H : CBSTenantHostContext
        //{
        //    //H n = new H { };
        //    var context = (H)Activator.CreateInstance(typeof(H), new object[] { });
        //    //var context = new CBSTenantHostContext { RetryResult = CommandReturnCodes.Retry };
        //    Initialize(context);
        //    return context;
        //}


        [SecurityCritical]
        public CBSTenantHostContextX CreateContext()
        {
            var context = new CBSTenantHostContextX { RetryResult = CommandReturnCodes.Retry };
            Initialize(context);
            return context;
        }


        private void Initialize(CBSTenantHostContextX context)
        {
            context.Arguments = new OrchardParameters
            {
                Arguments = new List<string>(),
                //ResponseFiles = new List<string>(),
                //Switches = new Dictionary<string, string>()
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
            context.CBSTenantHostX = CreateWorkerAppDomainWithHost(context.Arguments.VirtualPath, context.OrchardDirectory.FullName, typeof(CBSTenantHostX));

            //LogInfo(context, "Starting Orchard session");
            //context.StartSessionResult = context.CBSTenantHostX.StartSession();
        }


        //public C CreateWorkAppDomain<C>(CBSTenantHostContext context) where C : CBSTenantHost
        //{
        //    return (C)CreateWorkerAppDomainWithHost(context.Arguments.VirtualPath, context.OrchardDirectory.FullName, typeof(C));
        //}

        public CBSTenantHostX CreateWorkAppDomain(CBSTenantHostContextX context)
        {
            return (CBSTenantHostX)CreateWorkerAppDomainWithHost(context.Arguments.VirtualPath, context.OrchardDirectory.FullName, typeof(CBSTenantHostX));
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


        private static CBSTenantHostX CreateWorkerAppDomainWithHost(string virtualPath, string physicalPath, Type hostType)
        {
            var clientBuildManager = new ClientBuildManager(virtualPath, physicalPath);
            // Fix for https://github.com/OrchardCMS/Orchard/issues/1749
            // By forcing the CBM to build App_Code, etc, we ensure that the ASP.NET BuildManager
            // is in a state where it can safely (i.e. in a multi-threaded safe way) process
            // multiple concurrent calls to "GetCompiledAssembly".
            clientBuildManager.CompileApplicationDependencies();
            return (CBSTenantHostX)clientBuildManager.CreateObject(hostType, false);
        }


        private void LogInfo(CBSTenantHostContextX context, string format, params object[] args)
        {
            if (context.Logger != null)
                context.Logger.LogInfo(format, args);
        }



        [SecurityCritical]
        public void Shutdown(CBSTenantHostContextX context)
        {
            try
            {
                if (context.CBSTenantHostX != null)
                {
                    LogInfo(context, "Shutting down Orchard session...");
                    //context.CommandHost.StopSession(_input, _output);
                    context.StopSession();
                }
            }
            catch (AppDomainUnloadedException)
            {
                LogInfo(context, "   (AppDomain already unloaded)");
            }

            if (context.CBSTenantHostX != null)
            {
                LogInfo(context, "Shutting down ASP.NET AppDomain...");
                ApplicationManager.GetApplicationManager().ShutdownAll();
            }
        }
    }
}
