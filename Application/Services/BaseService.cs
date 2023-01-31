using Application.Utilities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class BaseService
    {
        protected HttpClientWrapper HttpClient { get; }

        public BaseService(IConfiguration configuration)
        {
            HttpClient = new HttpClientWrapper(configuration);
        }
    }
}
