namespace TVShowTracker.Application.DTOs.Entities;

public class WatchedEpisodeDto : BaseEntityDto
{
    public int UserId { get; set; }
    public int EpisodeId { get; set; }
    public DateTime WatchedDate { get; set; }

    public UserDto? User { get; set; }
    public EpisodeDto? Episode { get; set; }
}
