namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class ShowConfiguration : BaseEntityConfiguration<Show>
{
    public override void Configure(EntityTypeBuilder<Show> builder)
    {
        base.Configure(builder);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CreatedAt).HasColumnType("datetime2");

        builder.Property(s => s.ShowId);

        builder.HasIndex(u => u.ShowId)
            .IsUnique();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Overview)
            .HasMaxLength(1000);

        builder.Property(s => s.FirstAirDate);

        builder.Property(s => s.Popularity)
            .HasPrecision(3, 1);

        builder.HasMany(s => s.Episodes)
            .WithOne(e => e.Show)
            .HasForeignKey(e => e.ShowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.PosterPath)
            .HasMaxLength(2048);

        builder.HasIndex(s => s.Name);
    }
}