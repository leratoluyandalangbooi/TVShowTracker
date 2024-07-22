namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class TVShowConfiguration : BaseEntityConfiguration<TVShow>
{
    public override void Configure(EntityTypeBuilder<TVShow> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.TMDbShowId).IsRequired();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Overview).HasMaxLength(5000);
        builder.Property(s => s.FirstAirDate);
        builder.Property(s => s.Popularity).HasPrecision(10, 3);
        builder.Property(s => s.PosterPath).HasMaxLength(5000);
    }
}