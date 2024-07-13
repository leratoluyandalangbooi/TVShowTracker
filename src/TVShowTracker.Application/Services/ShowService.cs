namespace TVShowTracker.Application.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _showRepository;

    public ShowService(IShowRepository showRepository)
    {
        _showRepository = showRepository;
    }

    public async Task SaveTopShowsToDatabaseAsync(List<TopShowDto> topShows)
    {
        foreach (var show in topShows)
        {
            var existingShow = await _showRepository.GetTopShowByIdAsync(int.Parse(show.Id));
            if (existingShow == null)
            {
                await _showRepository.AddAsync(new TopShow
                {
                    Name = show.Name,
                    OriginalName = show.OriginalName,
                    FirstAirDate = show.FirstAirDate,
                    VoteAverage = show.VoteAverage,
                    VoteCount = show.VoteCount,
                    Adult = show.Adult,
                    BackdropPath = show.BackdropPath,
                    GenreIds = show.GenreIds,
                    OriginCountry = show.OriginCountry,
                    OriginalLanguage = show.OriginalLanguage,
                    Overview = show.Overview,
                    Popularity = show.Popularity,
                    PosterPath = show.PosterPath,
                });
            }
            else
            {
                existingShow.Name = show.Name;
                existingShow.OriginalName = show.OriginalName;
                existingShow.FirstAirDate = show.FirstAirDate;
                existingShow.VoteAverage = show.VoteAverage;
                existingShow.VoteCount = show.VoteCount;
                existingShow.Adult = show.Adult;
                existingShow.BackdropPath = show.BackdropPath;
                existingShow.GenreIds = show.GenreIds;
                existingShow.OriginCountry = show.OriginCountry;
                existingShow.OriginalLanguage = show.OriginalLanguage;
                existingShow.Overview = show.Overview;
                existingShow.Popularity = show.Popularity;
                existingShow.PosterPath = show.PosterPath;
            }
        }
        await _showRepository.SaveAsync();
    }

    public Task<List<TopShowDto>> GetTopShowsFromDatabase(int page, int pageSize)
    {
        return Task.FromResult(_showRepository.GetTopShowsFromDatabase().GetAwaiter().GetResult()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new TopShowDto
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                OriginalName = s.OriginalName,
                FirstAirDate = s.FirstAirDate,
                VoteAverage = s.VoteAverage,
                VoteCount = s.VoteCount,
                Adult = s.Adult,
                BackdropPath = s.BackdropPath,
                GenreIds = s.GenreIds,
                OriginCountry = s.OriginCountry,
                OriginalLanguage = s.OriginalLanguage,
                Overview = s.Overview,
                Popularity = s.Popularity,
                PosterPath = s.PosterPath,
            })
            .ToList());
    }
}
