using Shared.DTOs.Example;

namespace Application.Interfaces;

public interface IExampleService
{
    public Task<MessageResponse?> GetExampleAsync();
    public Task<List<ExampleResponse>> GetExampleListAsync();
}