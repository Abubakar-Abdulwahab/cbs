using Hangfire;
using log4net;
using Microsoft.Owin.Hosting;
using Parkway.CBS.HangfireScheduler.Configuration;
using Parkway.CBS.HangfireScheduler.Configuration.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.HangfireScheduler
{
    public partial class BillingSchedulerService : ServiceBase
    {
        private IHangFireScheduler _hangFireScheduler;
        public BillingSchedulerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Get all the scheduler Items
            _hangFireScheduler = new HangFireScheduler();
            List<HangFireSchedulerItem> hangFireSchedulerItems = _hangFireScheduler.GetHangFireSchedulerItems();

            foreach (var scheduleItem in hangFireSchedulerItems)
            {
                try
                {
                    //Start the hangfire server
                    _hangFireScheduler.StartHangFireDashboard(scheduleItem.TenantName, scheduleItem.ConnectionString, scheduleItem.DashboardUrl);
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected override void OnStop()
        {

        }

    }
}
