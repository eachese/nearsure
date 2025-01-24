namespace GameOfLifeApi.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates the dimensions and state of a Game of Life board.
        /// </summary>
        /// <param name="board">The board to validate.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        public static bool IsValidBoard(Board board, out string errorMessage)
        {
            if (board.Rows <= 0 || board.Columns <= 0)
            {
                errorMessage = "Rows and columns must be positive integers.";
                return false;
            }

            if (board.State == null || board.State.Count != board.Rows || board.State.Any(row => row.Count != board.Columns))
            {
                errorMessage = "Board state dimensions must match the specified rows and columns.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

}
