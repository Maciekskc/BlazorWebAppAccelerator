using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers
{
    public class ServiceTestBase<TManager> : TestBase<TManager>
    {
        protected IServiceCollection _serviceCollection;

        public ServiceTestBase():base()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddScoped(x => _testContext);
        }
    }
}
