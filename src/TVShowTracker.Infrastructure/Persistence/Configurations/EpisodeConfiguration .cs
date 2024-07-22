namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class EpisodeConfiguration : BaseEntityConfiguration<Episode>
{
    public override void Configure(EntityTypeBuilder<Episode> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.TMDbEpisodeId).IsRequired();
        builder.Property(e => e.Name) .HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.SeasonId).IsRequired();
        builder.Property(e => e.SeasonNumber);
        builder.Property(e => e.EpisodeNumber);
        builder.Property(e => e.AirDate);
        builder.Property(e => e.Overview).HasMaxLength(5000).IsRequired(false);
        builder.Property(e => e.StillPath).HasMaxLength(2048).IsRequired(false);
        builder.Property(e => e.Runtime);

        builder.HasOne(e => e.Season)
               .WithMany(s => s.Episodes)
               .HasForeignKey(e => e.SeasonId);
    }
}