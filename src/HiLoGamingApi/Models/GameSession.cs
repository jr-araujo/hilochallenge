namespace HiLoGamingApi.Models;

public class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Min { get; set; }
    public int Max { get; set; }
    public int MysteryNumber { get; set; }
    public bool IsFinished { get; set; } = false;

    public List<Player> Players { get; set; } = new();
    public int CurrentPlayerIndex { get; set; } = 0;
    public List<Guess> Guesses { get; set; } = new();
}