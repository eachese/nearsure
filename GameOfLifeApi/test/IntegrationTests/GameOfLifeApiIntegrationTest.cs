//using System.Net.Http.Json;
//using Xunit;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using StackExchange.Redis;
//using Moq;
//using GameOfLifeApi;
//using GameOfLifeApi.src.Utils;

//public class GameOfLifeApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
//{
//    private readonly HttpClient _client;
//    private readonly Mock<IDatabase> _redisDbMock;

//    public GameOfLifeApiIntegrationTests(WebApplicationFactory<Program> factory)
//    {
//        _redisDbMock = new Mock<IDatabase>();

//        var customizedFactory = factory.WithWebHostBuilder(builder =>
//        {
//            builder.ConfigureServices(services =>
//            {
//                // Remove the existing Redis service if any
//                var descriptor = services.SingleOrDefault(
//                    d => d.ServiceType == typeof(IConnectionMultiplexer));
//                if (descriptor != null)
//                {
//                    services.Remove(descriptor);
//                }

//                // Add mocked Redis service
//                var redisMock = new Mock<IConnectionMultiplexer>();
//                redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), null))
//                         .Returns(_redisDbMock.Object);
//                services.AddSingleton(redisMock.Object);

//                // Optionally configure logging
//                services.AddLogging(logging => logging.ClearProviders());
//            });
//        });

//        _client = customizedFactory.CreateClient();
//    }

//    [Fact]
//    public async Task UploadBoard_ShouldReturnBoardId()
//    {
//        // Arrange
//        var board = new
//        {
//            Rows = 3,
//            Columns = 3,
//            State = new List<List<bool>>
//            {
//                new List<bool> { true, false, true },
//                new List<bool> { false, true, false },
//                new List<bool> { true, false, true }
//            }
//        };

//        // Act
//        var response = await _client.PostAsJsonAsync("/api/GameOfLife/upload", board);
//        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

//        // Assert
//        response.EnsureSuccessStatusCode();
//        Assert.NotNull(result);
//        Assert.True(result.ContainsKey("Id"));
//        Assert.NotNull(result["Id"]);
//    }

//    [Fact]
//    public async Task GetNextState_ShouldReturnNextState()
//    {
//        // Arrange
//        var board = new
//        {
//            Rows = 2,
//            Columns = 2,
//            State = new List<List<bool>>
//            {
//                new List<bool> { true, true },
//                new List<bool> { true, true }
//            }
//        };
//        _redisDbMock
//            .Setup(db => db.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, false, When.Always, CommandFlags.None))
//            .Returns(true);

//        _redisDbMock
//            .Setup(db => db.StringGet(It.IsAny<RedisKey>(), CommandFlags.None))
//            .Returns((RedisValue)RedisHelper.Serialize(board));

//        var uploadResponse = await _client.PostAsJsonAsync("/api/GameOfLife/upload", board);
//        var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
//        var boardId = (string)uploadResult["Id"];

//        // Act
//        var response = await _client.GetAsync($"/api/GameOfLife/{boardId}/next");
//        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

//        // Assert
//        response.EnsureSuccessStatusCode();
//        Assert.NotNull(result);
//        Assert.True(result.ContainsKey("State"));
//    }
//}
