namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class WatchlistConfiguration : BaseEntityConfiguration<Watchlist>
{
    public override void Configure(EntityTypeBuilder<Watchlist> builder)
    {
        base.Configure(builder);

        builder.HasKey(w => w.Id);

        builder.Property(w => w.AddedDate)
            .IsRequired();

        builder.HasOne(w => w.User)
            .WithMany(u => u.Watchlist)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Show)
            .WithMany()
            .HasForeignKey(w => w.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Episode)
            .WithMany()
            .HasForeignKey(w => w.EpisodeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(w => new { w.UserId, w.ShowId })
            .IsUnique();
    }
}
