<<<<<<< Updated upstream
var builder = WebApplication.CreateBuilder(args);
=======
using System.Text;
using DeviceService.Profiles;
using DeviceService.Repositories;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);
/* LOGGER SERILOG*/
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();
>>>>>>> Stashed changes

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
<<<<<<< Updated upstream
builder.Services.AddEndpointsApiExplorer();
=======
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAutoMapper((serviceProvider, automapper) =>
{
    automapper.AddProfile(new DeviceProfile(serviceProvider.GetRequiredService<IHashids>()));
}, typeof(DeviceProfile));


builder.Services.AddEndpointsApiExplorer();



builder.Services.AddEndpointsApiExplorer();

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
>>>>>>> Stashed changes
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
