using Orchard.Tasks;

namespace Parkway.CBS.OrchardTenantStarter.Host.X
{
    internal class CBSTenantBackgroundServiceX : IBackgroundService
    {
        public void Sweep() { /*Don't run any background service in command line*/ }
    }
}
