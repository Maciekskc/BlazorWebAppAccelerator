using Application.DTOs.Example;

namespace Application.Interfaces;

public interface IExampleService
{
    public MessageResponse? GetExample();
    public List<ExampleResponse> GetExampleList();
}