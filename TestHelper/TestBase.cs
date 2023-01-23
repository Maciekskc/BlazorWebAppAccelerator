using Microsoft.EntityFrameworkCore;
using Persistance;

namespace TestHelpers
{
    public class TestBase<TManager> : IDisposable
    {
        protected TManager _sut;
        protected ApplicationDbContext _testContext;
        private bool disposedValue;
        protected readonly DbContextOptions<ApplicationDbContext> _dbOptions;

        public TestBase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase($"InMemoryDBName{Guid.NewGuid()}");
            optionsBuilder.EnableSensitiveDataLogging();

            _dbOptions = optionsBuilder.Options;

            _testContext = new ApplicationDbContext(_dbOptions);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                _testContext.Database.EnsureDeleted();
                _testContext.Dispose();
            }
            catch (Exception) { }
            finally
            {
                _testContext.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
