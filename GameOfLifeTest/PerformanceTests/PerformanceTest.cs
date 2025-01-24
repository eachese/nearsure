//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Running;
//using Moq;
//using StackExchange.Redis;
//using GameOfLifeApi.Repository;
//using Microsoft.Extensions.Logging;

//[MemoryDiagnoser]
//public class GameOfLifePerformanceTests
//{
//    private readonly GameOfLifeService _service;
//    private readonly Mock<IDatabase> _redisDbMock;

//    public GameOfLifePerformanceTests()
//    {
//        var redisMock = new Mock<IConnectionMultiplexer>();
//        _redisDbMock = new Mock<IDatabase>();
//        redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), null)).Returns(_redisDbMock.Object);

//        var loggerMock = new Mock<ILogger<GameOfLifeService>>();
//        _service = new GameOfLifeService(new GameOfLifeRepository(redisMock.Object), 1000, loggerMock.Object);

//        _redisDbMock
//            .Setup(db => db.StringGet(It.IsAny<RedisKey>(), CommandFlags.None))
//            .Returns((RedisValue)string.Empty);

//        _redisDbMock
//            .Setup(db => db.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, false, When.Always, CommandFlags.None))
//            .Returns(true);
//    }

//    [Benchmark]
//    public void SimulateLargeBoard()
//    {
//        var board = new Board
//        {
//            Rows = 1000,
//            Columns = 1000,
//            State = Enumerable.Range(0, 1000)
//                .Select(_ => Enumerable.Repeat(false, 1000).ToList())
//                .ToList()
//        };

//        _service.UploadBoard(board);

//        _service.GetNextState(board.Id);
//    }
//}

//#if DEBUG
//public static class PerformanceTestRunner
//{
//    public static void Main(string[] args)
//    {
//        BenchmarkRunner.Run<GameOfLifePerformanceTests>();
//    }
//}
//#endif