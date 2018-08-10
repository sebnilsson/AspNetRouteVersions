using Microsoft.AspNetCore.Mvc;

namespace AspNetRouteVersions.TestWebApp.Controllers
{
    [Route("api/v{api-version}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        [HttpPost]
        [RouteVersion(1)]
        public ActionResult<string> PostV1()
        {
            return "Post Items Version 1";
        }

        [HttpPost]
        [RouteVersion(2)]
        public ActionResult<string> PostV2()
        {
            return "Post Items Version 2";
        }

        [HttpGet]
        [RouteVersion(1)]
        public ActionResult<string> GetV1()
        {
            return "Get Items Version 1";
        }

        [HttpGet]
        [RouteVersion("1.5")]
        public ActionResult<string> GetV1_5()
        {
            return "Get Items Version 1.5";
        }

        [HttpGet]
        [RouteVersion(2)]
        public ActionResult<string> GetV2()
        {
            return "Get Items Version 2";
        }

        [HttpGet]
        [RouteVersion(3)]
        public ActionResult<string> GetV3()
        {
            return "Get Items Version 3";
        }
    }
}