using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Shared.Utilities;

namespace Application.Services
{
    public class ExampleService : BaseService, IExampleService
    {
        public ExampleService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<string?> GetExampleAsync()
        {
            return await HttpClient.Get<string>(EndpointMap.ExampleControllerPrefix + EndpointMap.ExampleController_HelloWorld);
        }
    }
}