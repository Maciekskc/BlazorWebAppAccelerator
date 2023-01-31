using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Shared.DTOs.Example;
using Shared.Utilities;

namespace Application.Services
{
    public class ExampleService : BaseService, IExampleService
    {
        public ExampleService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<MessageResponse?> GetExampleAsync()
        {
            return await HttpClient.Get<MessageResponse>(EndpointMap.ExampleControllerPrefix + EndpointMap.ExampleController_HelloWorld);
        }

        public async Task<List<ExampleResponse>> GetExampleListAsync()
        {
            return await HttpClient.Get<List<ExampleResponse>>(EndpointMap.ExampleControllerPrefix + EndpointMap.ExampleController_ExampleCollection);
        }
    }
}