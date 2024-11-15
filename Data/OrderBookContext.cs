using Microsoft.EntityFrameworkCore;

public class OrderBookContext : DbContext
{
    public DbSet<OrderBookEntry> OrderBookEntries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=OrderBook.db");
    }
}
