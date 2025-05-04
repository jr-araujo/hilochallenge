using HiLoGamingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HiLoGamingApi.Data;

public class HiLoDbContext(DbContextOptions<HiLoDbContext> options) : DbContext(options)
{
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Guess> Guesses => Set<Guess>();
}