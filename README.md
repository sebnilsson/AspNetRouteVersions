# ASP.NET Route Versions

Inspired by a discussion with a colleague and the great article [**Your API versioning is wrong** by Troy Hunt](https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/), where he concludes that **you don't need a war of preferrences between different ways of versioning your API**, you can actually support multiple ways in the same API.

In his article, Troy lists 3 ways (to do it wrong), which I have implemented for ASP.NET Core, and **added support for one more way**, which is URL versioning. This library supports the following ways to version your API:

- URL versioning
- Query string versioning
- Custom request header
- Content type

**URL versioning**:
```
HTTP GET:
https://my-web-app.com/api/v2/customers
```

**Query string versioning**:
```
HTTP GET:
https://my-web-app.com/api/customers?api-version=2
```

**Custom request header**:
```
HTTP GET:
https://my-web-app.com/api/customers
api-version: 2
```

**Content type**:
```
HTTP GET:
https://my-web-app.com/api/customers
Accept: application/vnd.api-version.v2+json
```

## `[RouteVersion]`-attribute

All you need to do is use the `[RouteVersion]`-attribute on the Controller-Actions you want to version and provide the route-version as argument:

```
[Route("api/v{api-version}/[controller]")]
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    [HttpGet]
    [RouteVersion(1)]
    public ActionResult<string> GetV1()
    {
        return "Get Customers Version 1";
    }

    [HttpGet]
    [RouteVersion(2)]
    public ActionResult<string> GetV2()
    {
        return "Get Customers Version 2";
    }

    [HttpPost]
    [RouteVersion(1)]
    public ActionResult<string> PostV1()
    {
        return "Post Customers Version 1";
    }

    [HttpPost]
    [RouteVersion(2)]
    public ActionResult<string> PostV2()
    {
        return "Post Customers Version 2";
    }
}
```

The attribute will only resolve versioning between Controller-Actions, **everything else is handled by the regular ASP.NET Core routing**, and behave as you're used to.

## Configuration

In your `Startup.cs` you can configure what ways of API-versioning you want to support (all activated by default). You can also change the keys of the routing, query string, custom header and content type.

```
public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureRouteVersions(options =>
    {        
        options.UseRoute = true;
        options.UseQuery = true;
        options.UseCustomHeader = true;
        options.UseAcceptHeader = true;

        // Set route-name in template used. For example: "api/v{version}/[controller]"
        // Default: "api-version"
        options.RouteKey = "version";

        // Set query string-key used. For example: "/api/customers?v=1"
        // Default: "api-version"
        options.QueryKey = "v"; // To use: '/api/customers?v=1'

        // Set custom version header used. For example: "my-app-api-version"
        // Default: "api-version"
        options.CustomHeaderKey = "my-app-api-version";

        // Set Accept-header vendor used. For example: "application/vnd.my-custom-api-header.v1+json"
        // Default: "application/vnd.api-version.v1+json"
        options.SetAcceptHeader("my-custom-api-header");

        // Set Accept-header regex-pattern. For example: "application/pre.my-custom-vendor-api.v1+json"
        options.AcceptRegexPattern = @"application\/pre\.my-custom-vendor-api\.v([\d]+)\+json";
    });

    services.AddMvc();
}
```

## Default version

If you know that **your new version of an API-endpoint is compatible with previous version**, and if you want to support it, you can use the `IsDefault`-parameter with the `[RouteVersion]`-attribute. For example, if you've just added new fields to the next version and you find that is compatible enough to be the default version of the API-endpoint:

```
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    [HttpGet]
    [RouteVersion(1)]
    public ActionResult<string> GetV1()
    {
        return "Get Customers Version 1";
    }

    [HttpGet]
    [RouteVersion(2, IsDefault = true)]
    public ActionResult<string> GetV2()
    {
        return "Get Customers Version 2";
    }
}
```

Then you can make a call to the URL for the API-endpoint **without specifying the version** and get the default version, which in this example is v2:

```
HTTP GET:
https://my-web-app.com/api/customers/
> "Get Customers Version 2"
```

## Contributing

You can find the [source code on GitHub](http://github.com/sebnilsson/AspNetRouteVersions/), the [newest unstable build on MyGet](https://www.myget.org/feed/sebnilsson/package/nuget/AspNetRouteVersions) and the [latest version of the library on NuGet](https://www.nuget.org/packages/AspNetRouteVersions/)