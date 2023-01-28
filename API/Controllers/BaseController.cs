using Azure;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult SendResponse(ServiceResponse response)
        {
            switch (response.ResponseType)
            {
                case HttpStatusCode.OK:
                    return Ok();

                case HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case HttpStatusCode.Forbidden:
                    return Forbid();

                case HttpStatusCode.NotFound:
                    return NotFound(response.Errors);

                case HttpStatusCode.NoContent:
                    return NoContent();

                case HttpStatusCode.Created:
                    return StatusCode(201);

                default:
                    return BadRequest();
            }
        }

        protected IActionResult SendResponse<T>(ServiceResponse<T> response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(response.Payload);

                case HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case HttpStatusCode.NotFound:
                    return NotFound(response.Errors);

                case HttpStatusCode.Forbidden:
                    return Forbid();

                case HttpStatusCode.Created:
                    return Created("uri", response.Payload);

                default:
                    return BadRequest();
            }
        }
    }
}
