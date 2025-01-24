using System.Text.Json;

namespace GameOfLifeApi.Helpers
{
    public static class RedisHelper
    {
        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T Deserialize<T>(string serialized)
        {
            return JsonSerializer.Deserialize<T>(serialized) ?? throw new InvalidOperationException("Deserialization failed.");
        }
    }
}
