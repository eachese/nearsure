namespace GameOfLifeApi.Repository
{
    public interface IGameOfLifeRepository
    {
        void SaveBoard(Board board);
        Board GetBoard(Guid boardId);
    }
}
