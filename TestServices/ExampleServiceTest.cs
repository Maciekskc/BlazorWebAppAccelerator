using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers;

namespace TestServices
{
    public class ExampleServiceTest : ServiceTestBase<BaseService>
    {
        public ExampleServiceTest()
        {
            _sut = new BaseService(_serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public void ExampleTest()
        {
            //Arrange - create test data
            var expectedServiceType = typeof(BaseService);

            //Example add sth to test context
            //_testContext.Add<TEntity>()


            //Act - sut do something
            var typeOfSut = _sut.GetType();

            //Assert - service output with expected results
            Assert.IsType(expectedServiceType, _sut);
            Assert.IsType<BaseService>(_sut);
            Assert.Equal(expectedServiceType, typeOfSut);
        }
    }
}