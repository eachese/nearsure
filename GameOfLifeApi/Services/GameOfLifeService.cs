using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using GameOfLifeApi.Helpers;
using GameOfLifeApi.Repository;
using GameOfLifeApi.Services;

/// <summary>
/// Service to manage Conway's Game of Life boards and operations using Redis.
/// </summary>
public class GameOfLifeService : IGameOfLifeService
{
    private readonly int _maxFinalStateAttempts;
    private readonly ILogger<GameOfLifeService> _logger;
    private readonly GameOfLifeRepository _repository;

    private readonly object _lock = new(); // Lock for critical sections

    /// <summary>
    /// Initializes a new instance of the GameOfLifeService class with the specified maximum final state attempts.
    /// </summary>
    /// <param name="redis">Redis connection multiplexer.</param>
    /// <param name="maxFinalStateAttempts">The maximum number of attempts to calculate the final state.</param>
    /// <param name="logger">The logger instance for capturing logs.</param>
    public GameOfLifeService(GameOfLifeRepository repository, int maxFinalStateAttempts, ILogger<GameOfLifeService> logger)
    {
        _repository = repository;
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
            if (!ValidationHelper.IsValidBoard(board, out var errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            _repository.SaveBoard(board);
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
            var board = _repository.GetBoard(boardId);

            var nextState = BoardUtils.GenerateNextState(
                BoardUtils.ConvertTo2DArray(board.State),
                board.Rows,
                board.Columns
            );

            board.State = BoardUtils.ConvertToNestedList(nextState, board.Rows, board.Columns);
            _repository.SaveBoard(board);

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
            var board = _repository.GetBoard(boardId);

            for (int i = 0; i < steps; i++)
            {
                var nextState = BoardUtils.GenerateNextState(
                    BoardUtils.ConvertTo2DArray(board.State),
                    board.Rows,
                    board.Columns
                );

                board.State = BoardUtils.ConvertToNestedList(nextState, board.Rows, board.Columns);
            }

            _repository.SaveBoard(board);
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
            var board = _repository.GetBoard(boardId);
            var seenStates = new HashSet<string>();
            var attempts = 0;

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
                attempts++;
            }

            throw new InvalidOperationException("Board did not stabilize after the maximum allowed attempts.");
        }
    }
}
