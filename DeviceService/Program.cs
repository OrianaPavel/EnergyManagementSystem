using DeviceService.Repositories;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);
/* LOGGER SERILOG*/
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Hour)
    .CreateLogger();

builder.Host.UseSerilog();
/* HASHIDS*/
builder.Services.AddSingleton<IHashids>(_ => new Hashids(builder.Configuration.GetSection("Hashids:Salt").Value!, 11));
/* =================================================== */
// Add services to the container.
/* DataBase Context Dependency Injection */
var dbHost = "localhost";
var dbName = "dm_device";
var dbUser = "root";
var dbPassword = "root";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID={dbUser};Password={dbPassword}";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseMySQL(connectionString));
/* =================================================== */
builder.Services.AddScoped<DeviceService.Service.DeviceService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IDeviceRepo, DeviceRepo>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();



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
});*/
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
