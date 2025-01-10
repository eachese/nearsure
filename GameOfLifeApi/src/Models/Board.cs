using System.Text.Json.Serialization;

public class Board
{
    [JsonIgnore]
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<List<bool>> State { get; set; }
}