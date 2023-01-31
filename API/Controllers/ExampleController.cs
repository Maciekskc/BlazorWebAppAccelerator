using Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Example;
using Shared.Utilities;
using System.Net;

namespace API.Controllers;

public class ExampleController : BaseController
{
    [Produces(typeof(string))]
    [Route(EndpointMap.ExampleController_HelloWorld)]
    [HttpGet(EndpointMap.ExampleController_HelloWorld)]
    public IActionResult Hello()
    {
        return SendResponse(new ServiceResponse<string>(HttpStatusCode.OK, "Hello World!"));
    }

    [Produces(typeof(List<ExampleResponse>))]
    [HttpGet(EndpointMap.ExampleController_ExampleCollection)]
    public IActionResult ExampleListResponse()
    {
        var templateResponseObject = new ExampleResponse("User","",0);
 
        var response = new List<ExampleResponse>();
        for (int i = 1; i <= 10; i++)
            response.Add(templateResponseObject with { LastName = i.ToString(), Age = new Random().Next(16, 32) });

        return SendResponse(new ServiceResponse<List<ExampleResponse>>(HttpStatusCode.OK, response));
    }

    [Produces(typeof(string))]
    [HttpGet(EndpointMap.ExampleController_BadRequest)]
    public IActionResult BadRequestEndpoint()
    {
        return SendResponse(new ServiceResponse<string>(HttpStatusCode.BadRequest, "BadRequest"));
    }

    [Authorize]
    [Produces(typeof(string))]
    [HttpGet(EndpointMap.ExampleController_Authorized)]
    public IActionResult Authorized()
    {
        return SendResponse(new ServiceResponse<string>(HttpStatusCode.OK, "Authorized"));
    }
}
