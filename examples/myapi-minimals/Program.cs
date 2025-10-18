using identity.server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using myapi_minimals.DTOs;
using myapi_minimals.infra;
using myapi_minimals.infra.Data;
using myapi_minimals.infra.Models;
using myapi_minimals.Repository;
using myapi_minimals.Services;
using myapi_minimals.SignalR;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

#region Register Services

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

// Register DataModule services
builder.Services.AddApplicationDataModule(builder.Configuration);
builder.Services.AddScoped<DapperRepository>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Test Minimal API",
        Version = "v1"
    });

    // 🔒 Add JWT security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your access token.\nExample: **Bearer eyJhbGciOiJIUzI1NiIs...**"
    });

    // 🔒 Require JWT token globally (optional)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<INewsLetterService, NewsLetterService>();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSignalR();

#endregion

#region Configure Services
var app = builder.Build();
DataSeeder.Seed(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || 1 == 1)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure SignalR hub
app.MapHub<UpdateHub>("/hub/update");

app.UseHttpsRedirection();
app.UseMiddleware<myapi_minimals.Helpers.GlobalExceptionHandler>();

//Middlewares
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


#endregion

#region Extra tools

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

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
        result = db.Employees.Where(e => e.Name.Contains(filter.Name)).ToList();
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
    var employee = db.Employees.FirstOrDefault(e => e.Id == Guid.Parse(id));
    if (employee == null)
    {
        return Results.NotFound();
    }
    var dto = new EmployeeDto
    {
        Id = employee.Id.ToString(),
        Name = employee.Name,
        Position = employee.Position,
        Salary = employee.Salary
    };
    return Results.Ok(dto);
})
    .WithName("GetEmployeeById")
    .RequireAuthorization()
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
    db.Employees.Add(new myapi_minimals.infra.Models.Employee
    {
        Id = Guid.Parse(payload.Id),
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


#endregion

#region Identity

var identityGroup = app.MapGroup("/identity");
identityGroup.MapPost("/register", async ([FromBody] RegisterDto model) =>
{
    using var scope = app.Services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var result = await userService.Register(model);
    if ((HttpStatusCode)result.Status != HttpStatusCode.OK)
    {
        return Results.BadRequest(result);
    }
    return Results.Ok(result);
}).WithName("RegisterUser");
identityGroup.MapPost("/login", async ([FromBody] LoginDto model) =>
{
    using var scope = app.Services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var result = await userService.Login(model);
    if (string.IsNullOrEmpty(result.Id))
    {
        return Results.BadRequest(result);
    }
    return Results.Ok(result);
}).WithName("LoginUser");
#endregion

#region NewsLetter Updates
var _updates = new[]
{
    new NewsLetterDto { Id = Guid.NewGuid().ToString() , Title = "Update 1", Date = DateTime.UtcNow.AddDays(-1) },
    new NewsLetterDto { Id = Guid.NewGuid().ToString() , Title = "Update 2", Date = DateTime.UtcNow.AddDays(-2) },
    new NewsLetterDto { Id = Guid.NewGuid().ToString() , Title = "Update 3", Date = DateTime.UtcNow.AddDays(-3) }
};
var updatesGroup = app.MapGroup("/updates");
updatesGroup.MapGet("/", async ([FromQuery] DateTime? since = null) =>
{
    var scope = app.Services.CreateScope();
    var newsLetterService = scope.ServiceProvider.GetRequiredService<INewsLetterService>();

    var updates = (await newsLetterService.List(since)).ToList();

    return Results.Ok(updates);
}).WithName("GetUpdates").RequireAuthorization();
updatesGroup.MapPost("/", async ([FromBody] NewsLetterDto update, [FromServices] IHubContext<UpdateHub> hubContext) =>
{
    var scope = app.Services.CreateScope();
    var newsLetterService = scope.ServiceProvider.GetRequiredService<INewsLetterService>();

    update.Id = Guid.NewGuid().ToString();
    update.Date = DateTime.UtcNow.Date;

    await newsLetterService.AddAsync(update, autoSave: true);
    await hubContext.Clients.All.SendAsync("ReceiveUpdate", update);
    return Results.Created($"/updates/{update.Id}", update);
}).WithName("CreateUpdate").RequireAuthorization();

#endregion

#region Dapper

app.MapGet("/dapper/employees", async ([FromServices] DapperRepository repository) =>
{
    var employees = await repository.GetEmployeesAsync();
    return Results.Ok(employees);
}).WithName("GetDapperEmployees").RequireAuthorization();

#endregion

#region ToDo
var todoGroup = app.MapGroup("/todos");
todoGroup.MapPost("/", async ([FromBody] List<ToDoDto> todos) =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    foreach (var todo in todos)
    {
        todo.Id = Guid.NewGuid().ToString();
        db.ToDos.Add(new ToDo
        {
            Id = Guid.Parse(todo.Id),
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            Date = todo.Date,
            EntityName = todo.EntityName,
            GroupName = todo.GroupName,
            Tags = todo.Tags,
            IsDeleted = todo.IsDeleted,
            CreatedAt = DateTime.UtcNow
        });
    }
    await db.SaveChangesAsync();
    return Results.Created($"/todos", todos);
}).WithName("CreateToDo").RequireAuthorization();

todoGroup.MapGet("/", async () =>
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var todos = await db.ToDos.ToListAsync();
    return Results.Ok(todos);
}).WithName("GetToDos").RequireAuthorization();

#endregion

app.Run();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
