using Orchard.Localization;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Commands;
using Orchard.Environment.Configuration;
using Orchard;
using Orchard.Environment;
using Orchard.Data;
using Orchard.Environment.State;
using Orchard.Exceptions;
using System.Reflection;
using Orchard.FileSystems.VirtualPath;
using Orchard.Caching;
using Orchard.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Payee.Models;

namespace Parkway.CBS.OrchardTenantStarter.Host
{
    /// <summary>
    /// This is the guy instantiated by the orchard.exe host. It is reponsible for
    /// executing a single command.
    /// <see cref="CommandHostAgent"/>
    /// </summary>
    public class CBSTenantHostAgent
    {
        private IContainer _hostContainer;

        public CBSTenantHostAgent()
        {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public CommandReturnCodes StartHost(/*IPPISMarshalled obj*/)
        {
            try
            {
                _hostContainer = CreateHostContainer();
                return CommandReturnCodes.Ok;
            }
            catch (OrchardCommandHostRetryException exception)
            {
                // Special "Retry" return code for our host
                //output.WriteLine(T("{0} (Retrying...)", ex.Message));
                return CommandReturnCodes.Retry;
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                //OutputException(output, T("Error starting up Orchard command line host"), ex);
                return CommandReturnCodes.Fail;
            }
        }

        public CommandReturnCodes StopHost()
        {
            try
            {
                if (_hostContainer != null)
                {
                    _hostContainer.Dispose();
                    _hostContainer = null;
                }
                return CommandReturnCodes.Ok;
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                //OutputException(output, T("Error shutting down Orchard command line host"), ex);
                return CommandReturnCodes.Fail;
            }
        }

        //private void OutputException(TextWriter output, LocalizedString title, Exception exception)
        //{
        //    // Display header
        //    output.WriteLine();
        //    output.WriteLine(T("{0}", title));

        //    // Push exceptions in a stack so we display from inner most to outer most
        //    var errors = new Stack<Exception>();
        //    for (var scan = exception; scan != null; scan = scan.InnerException)
        //    {
        //        errors.Push(scan);
        //    }

        //    // Display inner most exception details
        //    exception = errors.Peek();
        //    output.WriteLine(T("--------------------------------------------------------------------------------"));
        //    output.WriteLine();
        //    output.WriteLine(T("{0}", exception.Message));
        //    output.WriteLine();

        //    if (!((exception is OrchardException ||
        //        exception is OrchardCoreException) &&
        //        exception.InnerException == null))
        //    {

        //        output.WriteLine(T("Exception Details: {0}: {1}", exception.GetType().FullName, exception.Message));
        //        output.WriteLine();
        //        output.WriteLine(T("Stack Trace:"));
        //        output.WriteLine();

        //        // Display exceptions from inner most to outer most
        //        foreach (var error in errors)
        //        {
        //            output.WriteLine(T("[{0}: {1}]", error.GetType().Name, error.Message));
        //            output.WriteLine(T("{0}", error.StackTrace));
        //            output.WriteLine();
        //        }
        //    }

        //    // Display footer
        //    output.WriteLine("--------------------------------------------------------------------------------");
        //    output.WriteLine();
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private IContainer CreateHostContainer()
        {
            var hostContainer = OrchardStarter.CreateHostContainer(ContainerRegistrations);
            var host = hostContainer.Resolve<IOrchardHost>();

            host.Initialize();
            return hostContainer;
        }

        public IWorkContextScope CreateStandaloneEnvironment(string tenant)
        {
            var host = _hostContainer.Resolve<IOrchardHost>();
            var tenantManager = _hostContainer.Resolve<IShellSettingsManager>();

            // Retrieve settings for speficified tenant.
            var settingsList = tenantManager.LoadSettings();
            if (settingsList.Any())
            {
                var settings = settingsList.SingleOrDefault(s => String.Equals(s.Name, tenant, StringComparison.OrdinalIgnoreCase));
                if (settings == null)
                {
                    throw new OrchardCoreException(T("Tenant {0} does not exist", tenant));
                }

                var env = host.CreateStandaloneEnvironment(settings);
                return env;
            }
            else
            {
                // In case of an uninitialized site (no default settings yet), we create a default settings instance.
                var settings = new ShellSettings { Name = ShellSettings.DefaultName, State = TenantState.Uninitialized };
                return host.CreateStandaloneEnvironment(settings);
            }
        }

        protected void ContainerRegistrations(ContainerBuilder builder)
        {
            MvcSingletons(builder);

            builder.RegisterType<CBSTenantHostEnvironment>().As<IHostEnvironment>().SingleInstance();
            builder.RegisterType<CBSTenantHostVirtualPathMonitor>().As<IVirtualPathMonitor>().As<IVolatileProvider>().SingleInstance();
            builder.RegisterInstance(CreateShellRegistrations()).As<IShellContainerRegistrations>();
        }

        private CommandHostShellContainerRegistrations CreateShellRegistrations()
        {
            return new CommandHostShellContainerRegistrations
            {
                Registrations = shellBuilder => {
                    shellBuilder.RegisterType<CBSTenantHostVirtualPathMonitor>()
                        .As<IVirtualPathMonitor>()
                        .As<IVolatileProvider>()
                        .InstancePerMatchingLifetimeScope("shell");
                    shellBuilder.RegisterType<CBSTenantBackgroundService>()
                        .As<IBackgroundService>()
                        .InstancePerLifetimeScope();
                }
            };
        }

        static void MvcSingletons(ContainerBuilder builder)
        {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }

        private class CommandHostShellContainerRegistrations : IShellContainerRegistrations
        {
            public Action<ContainerBuilder> Registrations { get; set; }
        }
    }
}
