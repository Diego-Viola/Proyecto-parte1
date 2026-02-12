using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route(RouteRoot)]
    public class BaseApiController : ControllerBase
    {
        public const string RouteRoot = "api/v{version:apiVersion}/[controller]";
    }
}
