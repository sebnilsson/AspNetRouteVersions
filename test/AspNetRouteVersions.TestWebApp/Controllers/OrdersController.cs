﻿using Microsoft.AspNetCore.Mvc;

namespace AspNetRouteVersions.TestWebApp.Controllers
{
    [Route("api/v{api-version}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        [RouteVersion(1)]
        public ActionResult<string> PostV1()
        {
            return "Post Orders Version 1";
        }

        [HttpPost]
        [RouteVersion(2)]
        public ActionResult<string> PostV2()
        {
            return "Post Orders Version 2";
        }

        [HttpGet]
        [RouteVersion(1)]
        public ActionResult<string> GetV1()
        {
            return "Get Orders Version 1";
        }

        [HttpGet]
        [RouteVersion(2)]
        public ActionResult<string> GetV2()
        {
            return "Get Orders Version 2";
        }

        [HttpGet]
        [RouteVersion(3, IsDefault = true)]
        public ActionResult<string> GetV3()
        {
            return "Get Orders Version 3";
        }

        [HttpGet("{id:min(1):max(255)}")]
        public ActionResult<string> Get(int id)
        {
            return $"Get Order Value: {id}";
        }
    }
}