using Microsoft.EntityFrameworkCore;

namespace Rentopoly.Data;

public class BoardGameContext : DbContext
{
    public DbSet<BoardGame> BoardGames => Set<BoardGame>();
    public DbSet<Rental> Rentals => Set<Rental>();

    public BoardGameContext()
    {
        
    }

    public BoardGameContext(DbContextOptions<BoardGameContext> options)
        : base(options)
    { }
}
