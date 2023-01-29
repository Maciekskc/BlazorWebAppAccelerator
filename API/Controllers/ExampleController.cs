using Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Utilities;
using System.Net;

namespace API.Controllers
{
    public class ExampleController : BaseController
    {
        [Produces(typeof(string))]
        [HttpGet(EndpointMap.ExampleController_HelloWorld)]
        public IActionResult Hello()
        {
            return SendResponse(new ServiceResponse<string>(HttpStatusCode.OK, "Hello World!"));
        }

        [Produces(typeof(string))]
        [HttpGet(EndpointMap.ExampleController_BadRequest)]
        public IActionResult BadRequest()
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
}
