[Back to Home](README.md)

# Section 2: Designing with ASP.NET Core
## Chapter 5 
*Minimal APIs*
* Top Level Statements: Now in Entry point(program.cs) we can stop using namespaces
* Minimal Hosting: Combine two files (startup & program) into single file "program.cs" this laverage to minimize the biolerplate code necessary to bootstrap the application
```C#
WebApplicationBuilder builder = WebApplication.Create(args);
webApplication app = builder.Build();
app.Run();
```
**Minimal APIs**: minimal here means Lean not mean small or not enough.
- Serve essential features
- Using Minimal APIs We map routes to delegate
    - Inline Delegate (Arrow function)
    - Method Delegate (External method)
- Most endpoints have _**Inputs**_ (Implicity , Explicity)
- Inputs binding sources: Route, Query, Header, Body, Form, Services, Custom
- Special Types: 
    _HttpContext_: This compass all requests and responses
    _HttpRequest_: Same HttpContext.Request
    _HttpResponse_: Same HttpContext.Response
    _CancellationToken_:Same HttpContext.RequestAborted
    _ClaimsPrincipal_:Same HttpContext.USer

- **RFC 7807**: Is a specification for API error details as a _standardized format_
The typical response has: title, status, detail, instance
Example: 
```
HTTP/1.1 404 Not Found
Content-Type: application/problem+json

{
  "type": "https://example.com/probs/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "The requested user with id=123 was not found.",
  "instance": "/users/123"
}
```
- **Metadata**: Is a way to add additional information to the API
    - Swagger: Is a tool that help us to document the API
    - OpenAPI: Is a specification that define the format of the documentation
- Example
```c#
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi(operation =>
{
    operation.Description = "Gets a 5-day weather forecast.";
    operation.Summary = "Get Weather Forecast";
    operation.Deprecated = true;
    return operation;
});
```