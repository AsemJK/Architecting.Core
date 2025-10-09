using Microsoft.AspNetCore.Mvc;
using test_minimals.DTOs;
using test_minimals.infra;

var builder = WebApplication.CreateBuilder(args);
// Register DataModule services
builder.Services.AddApplicationDataModule(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

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

var employees = new List<EmployeeDto>
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

var jsonGroup = app.MapGroup("/json");


jsonGroup.MapGet("/data/{rate}/rate", (string rating) =>
{
}).AddEndpointFilter(async (context, next) =>
{
    var rating = context.GetArgument<string>(0);
    return await next(context);
});

jsonGroup.MapGet("/data", () =>
{
    var data = new { Message = "Hello, World!", Timestamp = DateTime.UtcNow };
    return TypedResults.Json(data, new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.KebabCaseUpper,
    });
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
