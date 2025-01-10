using Serilog;
using StackExchange.Redis;


        var builder = WebApplication.CreateBuilder(args);

        // Configure Redis connection
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnection = builder.Configuration.GetValue<string>("RedisSettings:ConnectionString");
            return ConnectionMultiplexer.Connect(redisConnection);
        });

        // Get the value for maxFinalStateAttempts from the configuration
        var maxAttempts = builder.Configuration.GetValue<int>("GameOfLifeSettings:MaxFinalStateAttempts");

        // Register the GameOfLifeService with the value for maxFinalStateAttempts
        builder.Services.AddScoped<GameOfLifeService>(sp =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var logger = sp.GetRequiredService<ILogger<GameOfLifeService>>();
            return new GameOfLifeService(redis, maxAttempts, logger);
        });

        // Add other services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Swagger middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Configure middleware
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
