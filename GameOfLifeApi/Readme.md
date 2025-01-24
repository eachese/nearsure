# Conway's Game of Life API

## Problem Description
Conway's Game of Life is a cellular automaton devised by mathematician John Conway. It consists of a grid of cells, where each cell can be in one of two states: **alive** (`true`) or **dead** (`false`). The state of the grid evolves over discrete time steps according to the following rules:

1. Any live cell with fewer than two live neighbors dies (underpopulation).
2. Any live cell with two or three live neighbors lives on to the next generation.
3. Any live cell with more than three live neighbors dies (overpopulation).
4. Any dead cell with exactly three live neighbors becomes a live cell (reproduction).

This API provides functionalities to:
- Upload an initial board state.
- Retrieve the next state of the board.
- Retrieve the board state after a specified number of steps.
- Retrieve the final state of the board, detecting cycles or stabilization.

## API Endpoints

### Upload a New Board
**POST** `/api/gameoflife/upload`

Uploads a new board and returns a unique ID for it.

**Request Body:**
```json
{
  "rows": 3,
  "columns": 3,
  "state": [[true, false, false], [false, true, false], [false, false, true]]
}
```

**Response:**
```json
{
  "id": "<GUID>"
}
```

---

### Get the Next State of a Board
**GET** `/api/gameoflife/{id}/next`

Retrieves the next state of the board.

**Response:**
```json
{
  "id": "<GUID>",
  "rows": 3,
  "columns": 3,
  "state": [[false, true, false], [false, true, false], [false, true, false]]
}
```

---

### Get Future State of a Board
**GET** `/api/gameoflife/{id}/future/{steps}`

Retrieves the board state after a specified number of steps.

**Response:**
```json
{
  "id": "<GUID>",
  "rows": 3,
  "columns": 3,
  "state": [[false, false, false], [true, true, true], [false, false, false]]
}
```

---

### Get Final State of a Board
**GET** `/api/gameoflife/{id}/final`

Retrieves the final state of the board. If the board does not stabilize or repeat after `X` iterations, an error is returned.

**Response (Final State):**
```json
{
  "id": "<GUID>",
  "rows": 3,
  "columns": 3,
  "state": [[false, false, false], [false, false, false], [false, false, false]]
}
```

**Response (Error):**
```json
{
  "error": "Board did not stabilize after 1000 iterations."
}
```

---

## Assumptions
1. **Maximum Board Size**: The maximum board size is assumed to be `1000x1000`.
2. **Initial State Validation**: All boards must have valid dimensions and contain only `true` or `false` values.
3. **Final State Rules**:
   - A stable state occurs when no changes occur between two consecutive steps.
   - A repeating cycle occurs when a previously seen state recurs.
4. **Persistence**: All board states are persisted, and the service is resilient to restarts.
5. **Performance**: The service is designed to handle boards with a high number of cells and iterations efficiently.

---

## Edge Cases

### Small Boards
- **1x1 Board**: A single cell that dies due to underpopulation.
- **1xN or Nx1 Board**: A single row or column where all cells eventually die.

### Empty Board
- **All Dead Cells**: The board remains unchanged.

### Full Board
- **All Live Cells**: Cells on the edges die due to underpopulation, and inner cells stabilize.

### Irregular Patterns
- **Oscillators**: Patterns like the "blinker" alternate states correctly.
- **Still Lifes**: Patterns like the "block" remain stable.
- **Spaceships**: Patterns like the "glider" move across the board correctly.

### Large Boards
- Performance remains acceptable for large boards (up to 1000x1000).

---

## How to Run Locally

### Prerequisites
- .NET 7.0 SDK
- Docker (optional, for containerized deployment)

### Steps
1. Clone the repository:
   ```bash
   git clone <repo_url>
   cd game-of-life-api
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access Swagger UI at `https://localhost:5001/swagger`.

---

## Testing

### Unit Tests
Run unit tests with:
```bash
dotnet test
```

### Edge Cases
Unit tests cover:
1. Small boards (1x1, 1xN).
2. Empty boards.
3. Full boards.
4. Oscillators, still lifes, and spaceships.
5. Large boards.

---

## Deployment

### Docker
1. Build the Docker image:
   ```bash
   docker build -t game-of-life-api .
   ```
2. Run the container:
   ```bash
   docker run -p 5000:80 game-of-life-api
   ```

Access the API at `http://localhost:5000`.

---

## Future Improvements
1. Authentication and authorization.
2. Enhanced performance for massive boards using parallelization.
3. WebSocket integration for real-time updates.
4. UI for visualizing board states.
