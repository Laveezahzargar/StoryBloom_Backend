using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P9_Blog_Generator_AI_Backend.Data;
using P9_Blog_Generator_AI_Backend.Middlewares;
using P9_Blog_Generator_AI_Backend.Services.Implementations;
using P9_Blog_Generator_AI_Backend.Services.Interfaces;
//using Serilog;
using Serilog.Events;
using System.Text;

//Log.Logger = new LoggerConfiguration()

//    .WriteTo.File(
//        "Logs/info-.txt",
//        rollingInterval: RollingInterval.Day,
//        restrictedToMinimumLevel: LogEventLevel.Information,
//        outputTemplate:
//        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")

//    .WriteTo.File(
//        "Logs/error-.txt",
//        rollingInterval: RollingInterval.Day,
//        restrictedToMinimumLevel: LogEventLevel.Error,
//        outputTemplate:
//        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")

//    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithOrigins("http://localhost:5175");
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddHttpClient<IAIService, AIService>();


builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowReact");

//app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

//app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();