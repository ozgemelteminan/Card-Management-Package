using Xunit;
using MerchantApp.Service;

namespace MerchantApp.Test
{
    public class ServiceSmokeTests
    {
        [Fact]
        public void Service_Project_Loads()
        {
            // Smoke Test: Check if the service layer is accessible and can be instantiated.
            // This does NOT test any business logic, just ensures the project and references load correctly.
            
            var serviceClass = new Class1();  // Instantiate a class from the service project
            Assert.NotNull(serviceClass);     // Verify that the object is not null
        }
    }
}
