namespace HiLoGamingApi.Models;

public class Guess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public int Value { get; set; }
    public string Result { get; set; } = string.Empty;
}