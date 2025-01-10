using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using GameOfLifeApi.src.Utils;

/// <summary>
/// Service to manage Conway's Game of Life boards and operations using Redis.
/// </summary>
public class GameOfLifeService
{
    private readonly IDatabase _redisDb;
    private readonly int _maxFinalStateAttempts;
    private readonly ILogger<GameOfLifeService> _logger;
    private readonly object _lock = new(); // Lock for critical sections

    /// <summary>
    /// Initializes a new instance of the GameOfLifeService class with the specified maximum final state attempts.
    /// </summary>
    /// <param name="redis">Redis connection multiplexer.</param>
    /// <param name="maxFinalStateAttempts">The maximum number of attempts to calculate the final state.</param>
    /// <param name="logger">The logger instance for capturing logs.</param>
    public GameOfLifeService(IConnectionMultiplexer redis, int maxFinalStateAttempts, ILogger<GameOfLifeService> logger)
    {
        _redisDb = redis.GetDatabase();
        _maxFinalStateAttempts = maxFinalStateAttempts;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a new board to the system.
    /// </summary>
    /// <param name="board">The board to upload.</param>
    /// <returns>The unique ID of the uploaded board.</returns>
    public Guid UploadBoard(Board board)
    {
        lock (_lock)
        {
            var serializedBoard = RedisHelper.Serialize(board);
            _redisDb.StringSet(board.Id.ToString(), serializedBoard);

            _logger.LogInformation("Uploaded new board with ID: {BoardId}", board.Id);
            return board.Id;
        }
    }

    /// <summary>
    /// Retrieves the next state of the board.
    /// </summary>
    /// <param name="boardId">The unique ID of the board.</param>
    /// <returns>The updated board with its next state.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the board ID is not found.</exception>
    public Board GetNextState(Guid boardId)
    {
        lock (_lock)
        {
            var serializedBoard = _redisDb.StringGet(boardId.ToString());
            if (string.IsNullOrEmpty(serializedBoard))
            {
                _logger.LogWarning("Attempted to access non-existent board with ID: {BoardId}", boardId);
                throw new KeyNotFoundException("Board not found");
            }

            var board = RedisHelper.Deserialize<Board>(serializedBoard);

            _logger.LogInformation("Calculating next state for board with ID: {BoardId}", boardId);

            var nextState = BoardUtils.GenerateNextState(
                BoardUtils.ConvertTo2DArray(board.State),
                board.Rows,
                board.Columns
            );

            board.State = BoardUtils.ConvertToNestedList(nextState, board.Rows, board.Columns);
            _redisDb.StringSet(board.Id.ToString(), RedisHelper.Serialize(board));

            _logger.LogInformation("Next state calculated for board with ID: {BoardId}", boardId);
            return board;
        }
    }

    /// <summary>
    /// Retrieves the board state after a specified number of steps.
    /// </summary>
    /// <param name="boardId">The unique ID of the board.</param>
    /// <param name="steps">The number of steps to calculate.</param>
    /// <returns>The updated board after the specified steps.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the board ID is not found.</exception>
    public Board GetFutureState(Guid boardId, int steps)
    {
        lock (_lock)
        {
            var serializedBoard = _redisDb.StringGet(boardId.ToString());
            if (string.IsNullOrEmpty(serializedBoard))
            {
                _logger.LogWarning("Attempted to access non-existent board with ID: {BoardId}", boardId);
                throw new KeyNotFoundException("Board not found");
            }

            var board = RedisHelper.Deserialize<Board>(serializedBoard);

            _logger.LogInformation("Calculating future state for board with ID: {BoardId} and Steps: {Steps}", boardId, steps);

            for (int i = 0; i < steps; i++)
            {
                var nextState = BoardUtils.GenerateNextState(
                    BoardUtils.ConvertTo2DArray(board.State),
                    board.Rows,
                    board.Columns
                );

                board.State = BoardUtils.ConvertToNestedList(nextState, board.Rows, board.Columns);
            }

            // Save the updated board state back to Redis
            _redisDb.StringSet(board.Id.ToString(), RedisHelper.Serialize(board));

            _logger.LogInformation("Future state calculated for board with ID: {BoardId} after {Steps} steps", boardId, steps);

            return board;
        }
    }

    /// <summary>
    /// Retrieves the final state of the board, detecting stabilization or cycles.
    /// </summary>
    /// <param name="boardId">The unique ID of the board.</param>
    /// <returns>The board in its final state.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the board ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the board does not stabilize or repeat within the maximum attempts.</exception>
    public Board GetFinalState(Guid boardId)
    {
        lock (_lock)
        {
            var serializedBoard = _redisDb.StringGet(boardId.ToString());
            if (string.IsNullOrEmpty(serializedBoard))
            {
                _logger.LogWarning("Attempted to access non-existent board with ID: {BoardId}", boardId);
                throw new KeyNotFoundException("Board not found");
            }

            var board = RedisHelper.Deserialize<Board>(serializedBoard);
            var seenStates = new HashSet<string>();
            var attempts = 0;

            _logger.LogInformation("Calculating final state for board with ID: {BoardId}", boardId);

            while (attempts < _maxFinalStateAttempts)
            {
                var currentStateString = BoardUtils.SerializeState(board.State);
                if (seenStates.Contains(currentStateString))
                {
                    _logger.LogInformation("Cycle detected for board with ID: {BoardId}", boardId);
                    return board;
                }

                seenStates.Add(currentStateString);

                var nextState = BoardUtils.GenerateNextState(
                    BoardUtils.ConvertTo2DArray(board.State),
                    board.Rows,
                    board.Columns
                );

                var isStable = BoardUtils.SerializeState(board.State) == BoardUtils.SerializeState(nextState);
                if (isStable)
                {
                    _logger.LogInformation("Stable state detected for board with ID: {BoardId}", boardId);
                    return board;
                }

                board.State = BoardUtils.ConvertToNestedList(nextState, board.Rows, board.Columns);
                _redisDb.StringSet(board.Id.ToString(), RedisHelper.Serialize(board));
                attempts++;
            }

            _logger.LogError("Board with ID: {BoardId} did not stabilize after {Attempts} attempts", boardId, _maxFinalStateAttempts);
            throw new InvalidOperationException("Board did not stabilize after the maximum allowed attempts.");
        }
    }
}
