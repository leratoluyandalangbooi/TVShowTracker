namespace TVShowTracker.Application.DTOs;

public class EpisodeDto
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; }
    public DateTime? AirDate { get; set; }

    public EpisodeDto(Episode episode)
    {
        Id = episode.Id;
        ShowId = episode.ShowId;
        SeasonNumber = episode.SeasonNumber;
        EpisodeNumber = episode.EpisodeNumber;
        Title = episode.Title;
        AirDate = episode.AirDate;
    }
}
