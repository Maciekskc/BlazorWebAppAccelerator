using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistance;

namespace Infrastructure.Services
{
    public class BaseService
    {
        protected ApplicationDbContext DbContext { get; }
        protected UserManager<ApplicationUser> UserManager { get; }


        public BaseService(IServiceProvider serviceProvider)
        {
            DbContext = serviceProvider.GetService<ApplicationDbContext>() ?? throw new Exception("Cannot get DB context from IServiceProvider.");
            UserManager = serviceProvider.GetService<UserManager<ApplicationUser>>() ?? throw new Exception("Cannot get UserManager from IServiceProvider."); ;
        }
    }
}
