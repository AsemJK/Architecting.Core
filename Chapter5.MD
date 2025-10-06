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
Minimal APIs represent a shift in ASP.NET Core toward a leaner approach, highly inspired by technologies such as Node.js and Express. The sources present a comprehensive view of Minimal APIs, detailing their structure, customization, organization, and interaction with established architectural patterns like Data Transfer Objects (DTOs)
- Serve essential features
- Using Minimal APIs We map routes to delegate
    - Inline Delegate (Arrow function)
    - Method Delegate (External method)
- Most endpoints have _**Inputs**_ (Implicity , Explicity)
- Inputs binding sources: Route, Query, Header, Body, Form, Services, Custom
- Special Types: 
    - _HttpContext_: This compass all requests and responses.
    - _HttpRequest_: Same HttpContext.Request.
    - _HttpResponse_: Same HttpContext.Response.
    - _CancellationToken_:Same HttpContext.RequestAborted
    - _ClaimsPrincipal_:Same HttpContext.USer

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
- **Json Serialization Options**:
    - Camel Case: Is the default naming policy such as "firstName"
    - Snake Case: such as "first_name"
    - Kebab Case: such as "first-name"
    - Pascal Case: such as "FirstName"
    #### _Customize Serialization Options_
    - Example
    ```c#
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    ``` 
- **MapGroup**
    - Is a way to group endpoints
    - Example: I will apply JsonSerializerOptions to the group
```c#
var jsonGroup = app.MapGroup("/json");
jsonGroup.MapGet("/data", () =>
{
    var data = new { Message = "Hello, World!", Timestamp = DateTime.UtcNow };
    return TypedResults.Json(data, new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.KebabCaseUpper,
    });
});
//Result 
/*
{
  "MESSAGE": "Hello, World!",
  "TIMESTAMP": "2025-10-03T13:09:10.7468014Z"
}
 */
```
- Endpoint Filters
    - Is a way to add cross-cutting concerns to the endpoints such as logging, validation, authentication, authorization, etc.
    - Example: I will create an endpoint filter that log the execution time of the endpoint
```c#
 public class EmployeeFilter : IEndpointFilter
    {
        public string? Name { get; set; }
        public decimal? Salary { get; set; }

        public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // You can add custom filtering logic here if needed
            var filter = context.Arguments.OfType<EmployeeFilter>().FirstOrDefault();
            if (filter != null)
            {
                // Example: Log the filter criteria
                Console.WriteLine($"Filtering employees with Name: {filter.Name}, Salary: {filter.Salary}");
                if (filter.Salary.HasValue && filter.Salary <= 0)
                {
                    return ValueTask.FromResult<object?>(Results.BadRequest("Salary must be non-negative"));
                }
            }
            return next(context);
        }
    }
```
- Example of using the filter
```c#
v ar employees = new List<EmployeeDto>
{
    new EmployeeDto { Id = 1, Name = "Alice", Position = "Developer" },
    new EmployeeDto { Id = 2, Name = "Bob", Position = "Manager" },
    new EmployeeDto { Id = 3, Name = "Charlie", Position = "Designer" }
};
var employeeGroup = app.MapGroup("/employees");

employeeGroup.MapPost("/", ([FromBody] EmployeeFilter filter) =>
{
    var result = employees.AsQueryable();
    if (!string.IsNullOrEmpty(filter.Name))
    {
        result = result.Where(e => e.Name != null && e.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
    }
    if (filter.Salary.HasValue)
    {
        result = result.Where(e => e.Salary.HasValue && e.Salary.Value >= filter.Salary.Value);
    }
    return Results.Ok(result.ToList());
})
    .WithName("GetEmployees")
    .AddEndpointFilter<EmployeeFilter>();
```

[Next Chapter](Chapter6.MD)
