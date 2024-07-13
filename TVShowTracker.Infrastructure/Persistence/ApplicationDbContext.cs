namespace TVShowTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{

    public DbSet<TopShow> TopShows { get; set; }
    //public DbSet<Episode> Episodes { get; set; }
    public DbSet<User> Users { get; set; }
    //public DbSet<Watchlist> Watchlist { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
