using GameOfLifeApi.Helpers;
using StackExchange.Redis;

namespace GameOfLifeApi.Repository
{
    public class GameOfLifeRepository : IGameOfLifeRepository
    {
        private readonly IDatabase _redisDb;

        public GameOfLifeRepository(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public void SaveBoard(Board board)
        {
            var serializedBoard = RedisHelper.Serialize(board);
            _redisDb.StringSet(board.Id.ToString(), serializedBoard);
        }

        public Board GetBoard(Guid boardId)
        {
            var serializedBoard = _redisDb.StringGet(boardId.ToString());
            if (string.IsNullOrEmpty(serializedBoard))
            {
                throw new KeyNotFoundException($"Board with ID {boardId} not found.");
            }
            return RedisHelper.Deserialize<Board>(serializedBoard);
        }
    }
}
