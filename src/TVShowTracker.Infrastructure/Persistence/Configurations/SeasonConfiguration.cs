namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class SeasonConfiguration : BaseEntityConfiguration<Season>
{
    public override void Configure(EntityTypeBuilder<Season> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.TMDbSeasonId).IsRequired();
        builder.Property(s => s.ShowId).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(500).IsRequired(false);
        builder.Property(s => s.AirDate);
        builder.Property(s => s.EpisodeCount);
        builder.Property(s => s.SeasonNumber).IsRequired();
        builder.Property(s => s.Overview);
        builder.Property(s => s.PosterPath);

        builder.HasOne(s => s.TVShow)
               .WithMany(t => t.Seasons)
               .HasForeignKey(s => s.ShowId);
    }
}


