namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class WatchedEpisodeConfiguration : BaseEntityConfiguration<WatchedEpisode>
{
    public override void Configure(EntityTypeBuilder<WatchedEpisode> builder)
    {
        base.Configure(builder);

        builder.Property(we => we.UserId).IsRequired();
        builder.Property(we => we.EpisodeId).IsRequired();
        builder.Property(we => we.WatchedDate).IsRequired();

        builder.HasOne(we => we.User)
               .WithMany(u => u.WatchedEpisodes)
               .HasForeignKey(we => we.UserId);

        builder.HasOne(we => we.Episode)
               .WithMany(e => e.WatchedEpisodes)
               .HasForeignKey(we => we.EpisodeId);
    }

}
