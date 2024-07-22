namespace TVShowTracker.Application.DTOs.Show;

public class GetEpisodeDetailsDto
{
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
}
