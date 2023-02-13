namespace Application.DTOs.Example;

public record MessageResponse(string Message);
public record ExampleResponse(string FirstName, string LastName, int Age);

public record ExampleResponseWrapped(List<ExampleResponse> Items);

