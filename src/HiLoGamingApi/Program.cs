using HiLoGamingApi.Data;
using HiLoGamingApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HiLoDbContext>(opt => opt.UseInMemoryDatabase("HiLoDB"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("/game/start", async (int min, int max, HiLoDbContext db) =>
{
    var rnd = new Random();
    var mystery = rnd.Next(min, max + 1);
    var game = new GameSession { Min = min, Max = max, MysteryNumber = mystery };
    db.GameSessions.Add(game);
    await db.SaveChangesAsync();
    return Results.Ok(game);
});

app.MapPost("/player/join", async (Guid gameId, string name, HiLoDbContext db) =>
{
    var game = await db.GameSessions.Include(g => g.Players).FirstOrDefaultAsync(g => g.Id == gameId);
    if (game == null || game.IsFinished) return Results.NotFound("Game not found or finished");

    var player = new Player { Name = name, GameSessionId = gameId };
    
    db.Players.Add(player);
    game.Players.Add(player);
    await db.SaveChangesAsync();
    return Results.Ok(player);
});

app.MapPost("/game/{gameId}/guess", async (Guid gameId, Guid playerId, int value, HiLoDbContext db) =>
{
    var game = await db.GameSessions.Include(g => g.Players).FirstOrDefaultAsync(g => g.Id == gameId);
    if (game == null || game.IsFinished) return Results.NotFound("Game not found or finished");

    var player = game.Players.FirstOrDefault(p => p.Id == playerId);
    if (player == null) return Results.NotFound("Player not found in this game");

    var currentPlayer = game.Players[game.CurrentPlayerIndex];
    if (currentPlayer.Id != playerId) return Results.BadRequest("Not your turn");

    var result = value < game.MysteryNumber ? "HI"
        : value > game.MysteryNumber ? "LO"
        : "CORRECT";

    var guess = new Guess { PlayerId = playerId, Value = value, Result = result };
    db.Guesses.Add(guess);
    game.Guesses.Add(guess);

    if (result == "CORRECT")
        game.IsFinished = true;
    else
        game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;

    await db.SaveChangesAsync();
    return Results.Ok(guess);
});

app.MapGet("/game/{gameId}", async (Guid gameId, HiLoDbContext db) =>
{
    var game = await db.GameSessions
        .Include(g => g.Players)
        .Include(g => g.Guesses)
        .FirstOrDefaultAsync(g => g.Id == gameId);
    return game == null ? Results.NotFound() : Results.Ok(game);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

