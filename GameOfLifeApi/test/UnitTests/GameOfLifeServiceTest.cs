using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using GameOfLifeApi.src.Utils;

public class GameOfLifeServiceTest
{
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<ILogger<GameOfLifeService>> _loggerMock;
    private readonly GameOfLifeService _service;

    public GameOfLifeServiceTest()
    {
        _redisDbMock = new Mock<IDatabase>();
        _redisMock = new Mock<IConnectionMultiplexer>();
        _loggerMock = new Mock<ILogger<GameOfLifeService>>();

        _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), null)).Returns(_redisDbMock.Object);

        _service = new GameOfLifeService(_redisMock.Object, 10, _loggerMock.Object);
    }

    [Fact]
    public void UploadBoard_ShouldSerializeAndSaveToRedis()
    {
        // Arrange
        var board = new Board
        {
            Rows = 3,
            Columns = 3,
            State = new List<List<bool>>
            {
                new List<bool> { true, false, true },
                new List<bool> { false, true, false },
                new List<bool> { true, false, true }
            }
        };

        _redisDbMock
            .Setup(db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ))
            .Returns(true);

        // Act
        var id = _service.UploadBoard(board);

        // Assert
        _redisDbMock.Verify(
            db => db.StringSet(
                It.Is<RedisKey>(key => key == board.Id.ToString()),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ),
            Times.Once
        );

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public void GetNextState_ShouldReturnUpdatedState()
    {
        // Arrange
        var board = new Board
        {
            Rows = 2,
            Columns = 2,
            State = new List<List<bool>>
            {
                new List<bool> { true, false },
                new List<bool> { false, true }
            }
        };

        _redisDbMock
            .Setup(db => db.StringGet(
                It.IsAny<RedisKey>(),
                CommandFlags.None 
            ))
            .Returns(RedisHelper.Serialize(board));

        _redisDbMock
            .Setup(db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ))
            .Returns(true);

        // Act
        var updatedBoard = _service.GetNextState(board.Id);

        // Assert
        Assert.NotNull(updatedBoard);
        _redisDbMock.Verify(
            db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ),
            Times.Once
        );
    }

    [Fact]
    public void GetFutureState_ShouldReturnStateAfterSteps()
    {
        // Arrange
        var board = new Board
        {
            Rows = 2,
            Columns = 2,
            State = new List<List<bool>>
        {
            new List<bool> { true, false },
            new List<bool> { false, true }
        }
        };

        _redisDbMock
            .Setup(db => db.StringGet(
                It.IsAny<RedisKey>(),
                CommandFlags.None
            ))
            .Returns(RedisHelper.Serialize(board));

        _redisDbMock
            .Setup(db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ))
            .Returns(true);

        // Act
        var updatedBoard = _service.GetFutureState(board.Id, 2);

        // Assert
        Assert.NotNull(updatedBoard);
        Assert.Equal(2, updatedBoard.Rows);
        Assert.Equal(2, updatedBoard.Columns);


        // Additional Debugging Logs
        _redisDbMock.Verify(db => db.StringGet(It.IsAny<RedisKey>(), CommandFlags.None), Times.Once);
        
    }

    [Fact]
    public void GetFinalState_ShouldDetectStabilization()
    {
        // Arrange
        var board = new Board
        {
            Rows = 2,
            Columns = 2,
            State = new List<List<bool>>
            {
                new List<bool> { true, false },
                new List<bool> { false, true }
            }
        };

        _redisDbMock
            .Setup(db => db.StringGet(
                It.IsAny<RedisKey>(),
                CommandFlags.None
            ))
            .Returns(RedisHelper.Serialize(board));

        _redisDbMock
            .Setup(db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ))
            .Returns(true);

        // Act
        var finalState = _service.GetFinalState(board.Id);

        // Assert
        Assert.NotNull(finalState);
        _redisDbMock.Verify(
            db => db.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                null,
                false,
                When.Always,
                CommandFlags.None
            ),
            Times.AtLeastOnce
        );
    }
}
