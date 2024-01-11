using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Repositories;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HashidsNet;
using UserService.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

/* LOGGER SERILOG*/
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
/* HASHIDS*/
builder.Services.AddSingleton<IHashids>(_ => new Hashids(builder.Configuration.GetSection("Hashids:Salt").Value!, 11));
/* =================================================== */
// Add services to the container.
/* DataBase Context Dependency Injection */
//var dbHost = "localhost";
//var dbHost = builder.Configuration.GetSection("LOCALDB:dbHost").Value!;
var dbHost = builder.Configuration["DB:dbHost"] ?? "localhost";
//var dbPort = builder.Configuration.GetSection("LOCALDB:dbPort").Value!;
var dbPort = builder.Configuration["DB:dbPort"] ?? "3306";
//var dbName = "dm_user";
//var dbName = builder.Configuration.GetSection("LOCALDB:dbName").Value!;
var dbName = builder.Configuration["DB:dbName"] ?? "dm_user";
//var dbUser = "root";
//var dbUser = builder.Configuration.GetSection("LOCALDB:dbUser").Value!;
var dbUser = builder.Configuration["DB:dbUser"] ?? "root";
//var dbPassword = "root";
//var dbPassword = builder.Configuration.GetSection("LOCALDB:dbPassword").Value!;
var dbPassword = builder.Configuration["DB:dbPassword"] ?? "root";

var connectionString = $"Data Source={dbHost};Port={dbPort};Initial Catalog={dbName};User ID={dbUser};Password={dbPassword}";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseMySQL(connectionString));
/* =================================================== */
builder.Services.AddScoped<UserService.Service.UserService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddHttpClient<HelperCallDeviceService>("DeviceUserClient", client =>
{
    //client.BaseAddress = new Uri(builder.Configuration.GetSection("DeviceServiceApi:BaseAddress").Value!);
    client.BaseAddress = new Uri(builder.Configuration["DeviceServiceApi:BaseAddress"] ?? "http://localhost:5214");
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                //builder.Configuration.GetSection("Jwt:Token").Value!))
                builder.Configuration["Jwt:Token"] ?? "AcrViINqhjkdEM4zOO8f04KdUBEOAzjJ61VfPajoFdDz3WvEctJg"))

    };
    options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                Log.Information($"Received access token: {accessToken}, Path: {path}");
                if (string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatHub/")))
                {
                    context.Token = accessToken;
                    context.HttpContext.Request.Headers.Add("Authorization", "Bearer " + accessToken);
                }
                return Task.CompletedTask;
            }
        };
});
builder.Services.AddSignalR(opts =>
            {
                opts.EnableDetailedErrors = true;
            });
var app = builder.Build();


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
          .AllowAnyHeader()
          .SetIsOriginAllowed((host) => true));

app.UseAuthentication();
app.UseAuthorization();


app.UseWebSockets();

app.MapHub<ChatHub>("chatHub");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
