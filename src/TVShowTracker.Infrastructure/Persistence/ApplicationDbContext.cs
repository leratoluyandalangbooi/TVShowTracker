using TVShowTracker.Infrastructure.Persistence.Configurations;

namespace TVShowTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Show> Shows { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Watchlist> Watchlist { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ShowConfiguration());
        modelBuilder.ApplyConfiguration(new EpisodeConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new WatchlistConfiguration());

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
