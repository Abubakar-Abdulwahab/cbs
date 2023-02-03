using NUnit.Framework;
using Parkway.CBS.HangFireInterface.Configuration;

namespace Parkway.CBS.ClientServicesTests
{
    [TestFixture]
    public class HangFireSchedulerTests
    {
        [Test]
        [TestCase("parkwaycbs")]
       public void GetConnectionString_WhenCalledWIthValidTenantName_ReturnNotNull(string tenantName)
        {
            var result = HangFireScheduler.GetConnectionString(tenantName);

            Assert.That(result, Is.Not.EqualTo(null));
        }

        [Test]
        [TestCase("nasarawa")]
        public void GetConnectionString_WhenCalledWIthInvalidTenantName_ReturnNull(string tenantName)
        {
            var result = HangFireScheduler.GetConnectionString(tenantName);

            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        [TestCase("parkwaycbs", "Server=(local);initial catalog=CentralBillingSystem;User ID=sa;Password=password@1;Connection Timeout=100000")]
        public void GetConnectionString_WhenCalledWIthValidTenantName_ReturnString(string tenantName, string expectedValue)
        {
            var result = HangFireScheduler.GetConnectionString(tenantName);

            Assert.That(result, Is.EqualTo(expectedValue));
        }
    }
}
