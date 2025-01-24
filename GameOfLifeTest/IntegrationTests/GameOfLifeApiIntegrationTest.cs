//using Xunit;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Moq;
//using StackExchange.Redis;
//using System.Net.Http.Json;
//using GameOfLifeApi.Helpers;
//using Microsoft.Extensions.DependencyInjection;

//public class GameOfLifeApiIntegrationTests : IClassFixture<WebApplicationFactory<GameOfLifeApi.Program>>
//{
//    private readonly HttpClient _client;
//    private readonly Mock<IDatabase> _redisDbMock;

//    public GameOfLifeApiIntegrationTests(WebApplicationFactory<GameOfLifeApi.Program> factory)
//    {
//        _redisDbMock = new Mock<IDatabase>();

//        var customizedFactory = factory.WithWebHostBuilder(builder =>
//        {
//            builder.ConfigureServices(services =>
//            {
//                var redisMock = new Mock<IConnectionMultiplexer>();
//                redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), null)).Returns(_redisDbMock.Object);
//                services.AddSingleton(redisMock.Object);
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
//                new List<bool> { true, false },
//                new List<bool> { false, true }
//            }
//        };

//        _redisDbMock
//            .Setup(db => db.StringGet(It.IsAny<RedisKey>(), CommandFlags.None))
//            .Returns(RedisHelper.Serialize(board));

//        var uploadResponse = await _client.PostAsJsonAsync("/api/GameOfLife/upload", board);
//        var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
//        var boardId = uploadResult["Id"].ToString();

//        // Act
//        var response = await _client.GetAsync($"/api/GameOfLife/{boardId}/next");
//        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

//        // Assert
//        response.EnsureSuccessStatusCode();
//        Assert.NotNull(result);
//        Assert.True(result.ContainsKey("State"));
//    }
//}
