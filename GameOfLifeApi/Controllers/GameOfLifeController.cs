using GameOfLifeApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GameOfLifeController : ControllerBase
{
    private readonly GameOfLifeService _service;

    public GameOfLifeController(GameOfLifeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Uploads a new board to the Game of Life system.
    /// </summary>
    /// <param name="board">The board object containing dimensions and state.</param>
    /// <returns>The ID of the uploaded board.</returns>
    /// <response code="200">Returns the ID of the newly uploaded board.</response>
    /// <response code="400">Invalid board dimensions or state.</response>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Upload a new board", Description = "Uploads a board with specified rows, columns, and state.")]
    public IActionResult UploadBoard([FromBody] Board board)
    {
        if (!ValidationHelper.IsValidBoard(board, out var errorMessage))
            return BadRequest(ApiResponse.Fail(errorMessage));

        var id = _service.UploadBoard(board);
        return Ok(ApiResponse.Success(new { Id = id }));
    }

    /// <summary>
    /// Retrieves the next state of the specified board.
    /// </summary>
    /// <param name="id">The unique ID of the board.</param>
    /// <returns>The board in its next state.</returns>
    /// <response code="200">Returns the board's next state.</response>
    /// <response code="404">Board not found.</response>
    [HttpGet("{id}/next")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get the next state of a board", Description = "Calculates and retrieves the next state of the specified board.")]
    public IActionResult GetNextState(Guid id)
    {
        var result = _service.GetNextState(id);
        if (result == null)
            return NotFound(ApiResponse.Fail("Board not found."));

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Retrieves the state of the board after a specified number of steps.
    /// </summary>
    /// <param name="id">The unique ID of the board.</param>
    /// <param name="steps">The number of steps to simulate.</param>
    /// <returns>The board state after the specified number of steps.</returns>
    /// <response code="200">Returns the board's state after the specified steps.</response>
    /// <response code="400">Invalid step count.</response>
    /// <response code="404">Board not found.</response>
    [HttpGet("{id}/future/{steps}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get future state of a board", Description = "Simulates the board state for the specified number of steps.")]
    public IActionResult GetFutureState(Guid id, int steps)
    {
        if (steps <= 0)
            return BadRequest(ApiResponse.Fail("Steps must be a positive integer."));

        var result = _service.GetFutureState(id, steps);
        if (result == null)
            return NotFound(ApiResponse.Fail("Board not found."));

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Retrieves the final state of the specified board.
    /// </summary>
    /// <param name="id">The unique ID of the board.</param>
    /// <returns>The final state of the board.</returns>
    /// <response code="200">Returns the board's final state.</response>
    /// <response code="400">Board did not stabilize or reached invalid state.</response>
    /// <response code="404">Board not found.</response>
    [HttpGet("{id}/final")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get final state of a board", Description = "Calculates and retrieves the board's final state, detecting stabilization or cycles.")]
    public IActionResult GetFinalState(Guid id)
    {
        try
        {
            var result = _service.GetFinalState(id);
            return Ok(ApiResponse.Success(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }
}
