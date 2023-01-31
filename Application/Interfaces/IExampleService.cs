using Shared.DTOs.Example;

namespace Application.Interfaces;

public interface IExampleService
{
    public Task<MessageResponse?> GetExampleAsync();
}