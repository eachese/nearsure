public static class BoardUtils
{
    /// <summary>
    /// Counts the number of live neighbors for a given cell in the grid.
    /// </summary>
    public static int CountLiveNeighbors(bool[,] board, int x, int y, int rows, int cols)
    {
        int count = 0;
        int[] directions = { -1, 0, 1 };

        foreach (var dx in directions)
        {
            foreach (var dy in directions)
            {
                if (dx == 0 && dy == 0)
                    continue; // Skip the cell itself

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && ny >= 0 && nx < rows && ny < cols && board[nx, ny])
                {
                    count++;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Converts a nested list of booleans to a 2D array.
    /// </summary>
    public static bool[,] ConvertTo2DArray(List<List<bool>> state)
    {
        int rows = state.Count;
        int cols = state[0].Count;
        var array = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = state[i][j];
            }
        }

        return array;
    }

    /// <summary>
    /// Converts a 2D array of booleans to a nested list.
    /// </summary>
    public static List<List<bool>> ConvertToNestedList(bool[,] array, int rows, int cols)
    {
        var list = new List<List<bool>>();

        for (int i = 0; i < rows; i++)
        {
            var row = new List<bool>();
            for (int j = 0; j < cols; j++)
            {
                row.Add(array[i, j]);
            }
            list.Add(row);
        }

        return list;
    }

    /// <summary>
    /// Serializes a 2D array or nested list to a string for cycle detection.
    /// </summary>
    public static string SerializeState(List<List<bool>> state)
    {
        return string.Join(",", state.Select(row => string.Join("", row.Select(cell => cell ? "1" : "0"))));
    }

    public static string SerializeState(bool[,] state)
    {
        var rows = state.GetLength(0);
        var cols = state.GetLength(1);
        var serialized = new List<string>();

        for (int i = 0; i < rows; i++)
        {
            var row = new List<string>();
            for (int j = 0; j < cols; j++)
            {
                row.Add(state[i, j] ? "1" : "0");
            }
            serialized.Add(string.Join("", row));
        }

        return string.Join(",", serialized);
    }

    public static bool[,] GenerateNextState(bool[,] currentState, int rows, int cols)
    {
        var nextState = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int liveNeighbors = CountLiveNeighbors(currentState, i, j, rows, cols);

                if (currentState[i, j])
                {
                    // Rules for live cells
                    nextState[i, j] = liveNeighbors == 2 || liveNeighbors == 3;
                }
                else
                {
                    // Rules for dead cells
                    nextState[i, j] = liveNeighbors == 3;
                }
            }
        }

        return nextState;
    }

}
