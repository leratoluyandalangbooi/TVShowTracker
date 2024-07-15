namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class EpisodeConfiguration : BaseEntityConfiguration<Episode>
{
    public override void Configure(EntityTypeBuilder<Episode> builder)
    {
        base.Configure(builder);

        builder.HasKey(s => s.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.SeasonNumber);

        builder.Property(e => e.EpisodeNumber);

        builder.Property(e => e.AirDate);

        builder.Property(e => e.Overview)
            .HasMaxLength(1000);

        builder.Property(p => p.StillPath)
            .HasMaxLength(2048);

        builder.Property(s => s.Runtime);

        builder.HasOne(e => e.Show)
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.ShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.ShowId, e.SeasonNumber, e.EpisodeNumber })
            .IsUnique();
    }
}