using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using GameOfLifeApi.Repository;
using GameOfLifeApi.Exceptions;
using GameOfLifeApi.Services;
using GameOfLifeApi.Configurations;

//namespace GameOfLifeApi;
//public partial class Program
//{
//    public static void Main(string[] args)
//    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
                };
            });

        // Redis connection
        var redis = ConnectionMultiplexer.Connect(builder.Configuration["RedisSettings:ConnectionString"]);
        builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

        // Dependency injection for services and repositories
        builder.Services.AddScoped<IGameOfLifeRepository, GameOfLifeRepository>();
        builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();

        // Configure app settings binding
        builder.Services.Configure<GameOfLifeSettings>(builder.Configuration.GetSection("GameOfLifeSettings"));

        var app = builder.Build();

        // Use middleware
        app.UseMiddleware<ExceptionHandling>();

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
//    }
//}
