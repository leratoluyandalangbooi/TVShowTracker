namespace TVShowTracker.Infrastructure.Persistence.Configurations;

public class ShowConfiguration : IEntityTypeConfiguration<TopShow>
{
    public void Configure(EntityTypeBuilder<TopShow> builder)
    {
        //builder.ToTable("TopShow");

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(255) 
            .IsRequired(); 

        builder.Property(p => p.OriginalName)
            .HasMaxLength(255);


        builder.Property(p => p.FirstAirDate);
            //.HasColumnType("datetime"); 

        builder.Property(p => p.VoteAverage);

        builder.Property(p => p.VoteCount);

        builder.Property(p => p.Adult);

        builder.Property(p => p.BackdropPath)
            .HasMaxLength(2048);

        builder.Property(p => p.GenreIds)
            .HasMaxLength(500)
            .HasConversion(
                value => string.Join(",", value),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            );

        builder.Property(p => p.OriginCountry);

        builder.Property(p => p.OriginalLanguage)
            .HasMaxLength(10); 

        builder.Property(p => p.Overview);

        builder.Property(p => p.Popularity);

        builder.Property(p => p.PosterPath)
            .HasMaxLength(2048);
    }
}
