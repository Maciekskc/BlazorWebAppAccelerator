using Microsoft.Extensions.DependencyInjection;
using Persistance;

namespace Application.Services
{
    public class BaseService
    {
        protected ApplicationDbContext DbContext { get; }

        public BaseService(IServiceProvider serviceProvider) => DbContext = serviceProvider.GetService<ApplicationDbContext>() ?? throw new Exception("Cannot get DB context from IServiceProvider.");
    }
}
