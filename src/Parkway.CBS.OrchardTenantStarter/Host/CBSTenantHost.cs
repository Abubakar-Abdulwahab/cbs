using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Web.Hosting;
using Orchard;
using Orchard.Host;
using Orchard.Users.Models;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Payee.Models;

namespace Parkway.CBS.OrchardTenantStarter.Host
{

    /// <summary>
    /// The CBSTenantHost runs inside the ASP.NET AppDomain and serves as an intermediate
    /// between the command line and the HostAgent, which is known to the Orchard
    /// Framework and has the ability to execute commands.
    /// <see cref="Orchard.Host.CommandHost"/>
    /// </summary>
    public class CBSTenantHost : MarshalByRefObject, IRegisteredObject
    {
        protected object _agent;

        public CBSTenantHost()
        {
            HostingEnvironment.RegisterObject(this);
        }


        public object GetAgent() { return _agent; }


        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            // never expire the cross-AppDomain lease on this object
            return null;
        }

        private static void ExtendLifeTimeLeases<T>(T obj) where T : MarshalByRefObject
        {
            // Orchard objects passed as parameters into this AppDomain should derive from MarshalByRefObject and have
            // infinite lease timeouts by means of their InitializeLifetimeService overrides.  For the input/output 
            // stream objects we approximate that behavior by immediately renewing the lease for 30 days.
            ExtendLifeTimeLease(obj);
        }

        private static void ExtendLifeTimeLease(MarshalByRefObject obj)
        {
            if (RemotingServices.IsObjectOutOfAppDomain(obj))
            {
                var lease = (ILease)RemotingServices.GetLifetimeService(obj);
                lease.Renew(TimeSpan.FromDays(30));
            }
        }

        internal void ProcessIPPISFile(string implementingClassName, string tenant, string fullFilePath, int month, int year)
        {
            using (var workContext = (IWorkContextScope)_agent.GetType().GetMethod("CreateStandaloneEnvironment").Invoke(_agent, new object[] { tenant }))
            {
                try
                {
                    //IPPISMarshalled f = new IPPISMarshalled { ImplementingClassName = "dfd" };
                    //ExtendLifeTimeLeases<IPPISMarshalled>(f);
                    //string gh = "";
                    //
                    //var wk = workContext.Resolve<ICoreInvoiceService>();
                    //wk.CheckRequestReference(null, null);
                        //.ProcessIPPISFromExcelFilex();

                    //get grouped data
                    //var groupedPayee = _payeeAdapter.GetPayeesGroup<IPPISAssessmentLineRecordModel, IPPISBatchGroup>(payeResponse.Payees);
                    //save the batch record response


                    //var payeeSummaryList = _payeeAdapter.GetIPPISPayeeMinistrySummary(payeResponse.Payees);

                    var result = workContext.Resolve<IOrchardServices>().TransactionManager.GetSession().Get<UserPartRecord>(2);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public void CreateStandaloneEnvironment(string tenant)
        {
            using (var sds = (IWorkContextScope)_agent.GetType().GetMethod("CreateStandaloneEnvironment").Invoke(_agent, new object[] { tenant }))
            {
                try
                {
                    var result = sds.Resolve<IOrchardServices>().TransactionManager.GetSession().Get<UserPartRecord>(2);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        internal CommandReturnCodes StartSession(/*IPPISMarshalled obj*/)
        {
            //ExtendLifeTimeLeases<IPPISMarshalled>(obj);
            _agent = CreateAgent();
            return StartHost(_agent/*, obj*/);
        }

        protected object CreateAgent()
        {
            return Activator.CreateInstance("Parkway.CBS.OrchardTenantStarter", "Parkway.CBS.OrchardTenantStarter.Host.CBSTenantHostAgent").Unwrap();
        }


        [SecuritySafeCritical]
        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }

        internal void StopSession()
        {
            if (_agent != null)
            {
                StopHost(_agent);
                _agent = null;
            }
        }

        protected CommandReturnCodes StopHost(object agent)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StopHost").Invoke(agent, new object[] { });
        }

        protected CommandReturnCodes StartHost(object agent/*, IPPISMarshalled obj*/)
        {
            return (CommandReturnCodes)agent.GetType().GetMethod("StartHost").Invoke(agent, new object[] { /*obj*/ });
        }
    }
}
