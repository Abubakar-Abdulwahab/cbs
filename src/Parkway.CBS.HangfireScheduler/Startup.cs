using Hangfire;
using Owin;

namespace Parkway.CBS.HangfireScheduler
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard();
        }
    }
}
