using identity.server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using test_minimals.DTOs;
using test_minimals.infra;
using test_minimals.infra.Data;
using test_minimals.infra.Models;

var builder = WebApplication.CreateBuilder(args);

#region Register Services

// Register DataModule services
builder.Services.AddApplicationDataModule(builder.Configuration);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Minimals API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:7161/connect/authorize"),
                TokenUrl = new Uri("https://localhost:7161/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "test-minimals.read", "Read access to Test Minimals API" },
                    { "test-minimals.write", "Write access to Test Minimals API" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7161"; // IdentityServer URL
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

#endregion

#region Configure Services
var app = builder.Build();
DataSeeder.Initialize(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<test_minimals.Helpers.GlobalExceptionHandler>();

//Middlewares
//app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();


#endregion

#region APIs

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
    new EmployeeDto { Id = Guid.NewGuid().ToString(), Name = "Alice", Position = "Developer" },
    new EmployeeDto { Id = Guid.NewGuid().ToString(), Name = "Bob", Position = "Manager" },
    new EmployeeDto { Id = Guid.NewGuid().ToString(), Name = "Charlie", Position = "Designer" }
};
var employeeGroup = app.MapGroup("/employees");


employeeGroup.MapPost("/search", ([FromBody] EmployeeFilter filter) =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var result = new List<Employee>();
    if (!string.IsNullOrEmpty(filter.Name))
    {
        result = db.Employees.Where(e => e.Name != null && e.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    if (filter.Salary.HasValue)
    {
        result = db.Employees.Where(e => e.Salary.HasValue && e.Salary.Value >= filter.Salary.Value).ToList();
    }
    return Results.Ok(result.ToList());
})
    .WithName("GetEmployees")
    .AddEndpointFilter<EmployeeFilter>()
    ;
employeeGroup.MapGet("/{id}", (string id) =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var employee = db.Employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    var dto = new EmployeeDto
    {
        Id = employee.Id,
        Name = employee.Name,
        Position = employee.Position,
        Salary = employee.Salary
    };
    return Results.Ok(dto);
})
    .WithName("GetEmployeeById")
    //.RequireAuthorization()
    ;

employeeGroup.MapPost("/", async ([FromBody] EmployeeDto payload) =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var employee = db.Employees.FirstOrDefault(e => e.Name == payload.Name);
    if (employee != null)
    {
        return Results.Conflict($"Employee with name {payload.Name} already exists.");
    }
    payload.Id = Guid.NewGuid().ToString();
    db.Employees.Add(new test_minimals.infra.Models.Employee
    {
        Id = payload.Id,
        Name = payload.Name,
        Position = payload.Position,
        Salary = payload.Salary
    });
    await db.SaveChangesAsync();
    return Results.Created($"/employees/{payload.Id}", payload);
})
    .WithName("CreateEmployee");


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
}).RequireAuthorization();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

#endregion

