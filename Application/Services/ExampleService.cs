using Application.DTOs.Example;
using Application.Interfaces;
using Infrastructure.Services;
using Infrastructure.Utilities;
using System.Net;

namespace Application.Services
{
    public class ExampleService : BaseService, IExampleService
    {
        public ExampleService(IServiceProvider serviceProvider) : base (serviceProvider)
        {
        }

        public MessageResponse? GetExample()
        {
            return new MessageResponse("Hello there!");
        }

        public List<ExampleResponse> GetExampleList()
        {
            var templateResponseObject = new ExampleResponse("User", "", 0);

            var response = new List<ExampleResponse>();
            for (int i = 1; i <= 10; i++)
                response.Add(templateResponseObject with { LastName = i.ToString(), Age = new Random().Next(16, 32) });

            return response;
        }
    }
}