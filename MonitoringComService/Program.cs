using System.Text;
using MonitoringComService.Profiles;
using MonitoringComService.Data;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using MonitoringComService.Services;
using System.Net.WebSockets;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/* LOGGER SERILOG*/
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.
/* HASHIDS*/
builder.Services.AddSingleton<IHashids>(_ => new Hashids(builder.Configuration.GetSection("Hashids:Salt").Value!, 11));
/* =================================================== */
// Add services to the container.
/* DataBase Context Dependency Injection */
var dbHost = builder.Configuration.GetSection("LOCALDB:dbHost").Value!;
var dbPort = builder.Configuration.GetSection("LOCALDB:dbPort").Value!;
//var dbName = "dm_user";
var dbName = builder.Configuration.GetSection("LOCALDB:dbName").Value!;
//var dbUser = "root";
var dbUser = builder.Configuration.GetSection("LOCALDB:dbUser").Value!;
//var dbPassword = "root";
var dbPassword = builder.Configuration.GetSection("LOCALDB:dbPassword").Value!;

var connectionString = $"Data Source={dbHost};Port={dbPort};Initial Catalog={dbName};User ID={dbUser};Password={dbPassword}";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseMySQL(connectionString));
/* =================================================== */
builder.Services.AddScoped<MonitoringComService.Services.DeviceService>();
builder.Services.AddScoped<MonitoringComService.Services.MeasurementService>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IMeasurementRepository, MeasurementRepository>();
builder.Services.AddScoped<WebSocketHandler>();
//services.AddSingleton<RabbitMQConsumerService>();

builder.Services.AddScoped<RabbitMQConsumerService>(sp => 
{
    var rabbitMQConnection = builder.Configuration.GetSection("RabbitMQ:Connection").Value!;
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var logger = sp.GetRequiredService<ILogger<RabbitMQConsumerService>>();
    return new RabbitMQConsumerService(rabbitMQConnection, scopeFactory, logger);
});




// Add services to the container.
builder.Services.AddControllers();
    builder.Services.AddSignalR();
builder.Services.AddAutoMapper((serviceProvider, automapper) =>
{
    automapper.AddProfile(new MappingProfile(serviceProvider.GetRequiredService<IHashids>()));
}, typeof(MappingProfile));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

/*
builder.Services.AddSwaggerGen(options =>{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("Jwt:Token").Value!))
    };
});
*/

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Start consuming messages
/*
var rabbitMQConsumerService = app.Services.GetRequiredService<RabbitMQConsumerService>();
rabbitMQConsumerService.StartConsuming("device_create");
rabbitMQConsumerService.StartConsuming("device_update");
rabbitMQConsumerService.StartConsuming("device_delete");
rabbitMQConsumerService.StartConsuming("EnergyConsumptionData");
*/
using (var scope = app.Services.CreateScope())
{
    var rabbitMQConsumerService = scope.ServiceProvider.GetRequiredService<RabbitMQConsumerService>();
    rabbitMQConsumerService.StartConsuming("device_create");
    rabbitMQConsumerService.StartConsuming("device_update");
    rabbitMQConsumerService.StartConsuming("device_delete");
    rabbitMQConsumerService.StartConsuming("EnergyConsumptionData");
}
/*
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var token = context.Request.Query["token"];
            var logger = context.RequestServices.GetRequiredService<ILogger<TokenService>>();
            var tokenService = new TokenService(builder.Configuration.GetSection("Jwt:Token").Value!,logger);
            var userId = tokenService.ValidateTokenAndGetUserId(token);
            
            if (userId != null)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                webSocketHandler.HandleConnection(userId, webSocket);
            }
            else
            {
                context.Response.StatusCode = 401; 
            }
        }
        else
        {
            context.Response.StatusCode = 400; 
        }
    }
    else
    {
        await next();
    }
});*/





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Setup CORS
app.UseCors(policy => 
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<SocketHub>("/socket-hub");
app.MapControllers();

app.Run();
