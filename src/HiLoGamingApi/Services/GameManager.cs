using System.Collections.Concurrent;
using HiLoGamingApi.Models;

namespace HiLoGamingApi.Services;

public class GameManager
{
    private readonly ConcurrentDictionary<Guid, GameSession> _games = new();

    public GameSession CreateGame(int min, int max)
    {
        var rnd = new Random();
        var mysteryNumber = rnd.Next(min, max + 1);

        var game = new GameSession
        {
            Min = min,
            Max = max,
            MysteryNumber = mysteryNumber
        };

        _games[game.Id] = game;
        return game;
    }

    public GameSession? GetGame(Guid id)
    {
        _games.TryGetValue(id, out var game);
        return game;
    }

    public Guess MakeGuess(Guid gameId, int value)
    {
        if (!_games.TryGetValue(gameId, out var game) || game.IsFinished)
            throw new InvalidOperationException("Game not found or already finished.");

        var result = value < game.MysteryNumber ? "HI" :
            value > game.MysteryNumber ? "LO" :
            "CORRECT";

        var guess = new Guess { Value = value, Result = result };
        game.Guesses.Add(guess);

        if (result == "CORRECT")
            game.IsFinished = true;

        return guess;
    }
}