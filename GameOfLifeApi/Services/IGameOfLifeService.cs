namespace GameOfLifeApi.Services
{
    public interface IGameOfLifeService
    {
        Guid UploadBoard(Board board);
        Board GetNextState(Guid boardId);
        Board GetFutureState(Guid boardId, int steps);
        Board GetFinalState(Guid boardId);
    }
}
