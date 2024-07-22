using TVShowTracker.Application.DTOs.Entities;
using TVShowTracker.Application.DTOs.Request;
using TVShowTracker.Infrastructure.ExternalServices.Models;

namespace TVShowTracker.Infrastructure.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity to DTO mappings
        CreateMap<TVShow, TVShowDto>()
            .ForMember(dest => dest.TMDbShowId, opt => opt.MapFrom(src => src.TMDbShowId))
            .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.Seasons))
            .ReverseMap();

        CreateMap<Season, SeasonDto>()
            .ForMember(dest => dest.TMDbSeasonId, opt => opt.MapFrom(src => src.TMDbSeasonId))
            .ForMember(dest => dest.ShowId, opt => opt.MapFrom(src => src.ShowId))
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes))
            .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.EpisodeCount))
            .ReverseMap();

        CreateMap<Episode, EpisodeDto>()
            .ForMember(dest => dest.TMDbEpisodeId, opt => opt.MapFrom(src => src.TMDbEpisodeId))
            .ReverseMap();

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
            .ReverseMap();

        CreateMap<User, RegisterUserDto>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<User, LoginDto>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ReverseMap();

        // WatchedEpisode mappings
        CreateMap<WatchedEpisode, WatchedEpisodeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.EpisodeId, opt => opt.MapFrom(src => src.EpisodeId))
            .ForMember(dest => dest.WatchedDate, opt => opt.MapFrom(src => src.WatchedDate))
            .ReverseMap();

        // WatchedEpisode Details
        CreateMap<WatchedEpisode, WatchedEpisodeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.EpisodeId, opt => opt.MapFrom(src => src.EpisodeId))
            .ForMember(dest => dest.WatchedDate, opt => opt.MapFrom(src => src.WatchedDate))
            .ForMember(dest => dest.Episode, opt => opt.MapFrom(src => src.Episode));

        // Collection mappings
        CreateMap<List<TVShowDto>, IEnumerable<TVShow>>()
            .ConvertUsing((src, _, context) => src.Select(dto => context.Mapper.Map<TVShow>(dto)));

        CreateMap<IEnumerable<TVShow>, List<TVShowDto>>()
            .ConvertUsing((src, _, context) => src.Select(show => context.Mapper.Map<TVShowDto>(show)).ToList());

        // TMDb response mappings
        CreateMap<TMDbShowResponse, TVShowDto>()
           .ForMember(dest => dest.TMDbShowId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.FirstAirDate, opt => opt.MapFrom(src => ParseDate(src.FirstAirDate)));

        CreateMap<TMDbTopShowsResponse, IEnumerable<TVShowDto>>()
            .ConvertUsing((src, _, context) =>
                src.Results.Select(show => context.Mapper.Map<TVShowDto>(show)).ToList());

        CreateMap<TMDbShowDetailsResponse, TVShowDto>()
            .ForMember(dest => dest.TMDbShowId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.Seasons))
            .ForMember(dest => dest.FirstAirDate, opt => opt.MapFrom(src => ParseDate(src.FirstAirDate)))
            .ForMember(dest => dest.LastEpisodeToAir, opt => opt.MapFrom(src => src.LastEpisodeToAir))
            .ForMember(dest => dest.NextEpisodeToAir, opt => opt.MapFrom(src => src.NextEpisodeToAir))
            .ForMember(dest => dest.NumberOfEpisodes, opt => opt.MapFrom(src => src.NumberOfEpisodes))
            .ForMember(dest => dest.NumberOfSeasons, opt => opt.MapFrom(src => src.NumberOfSeasons))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.VoteCount));

        CreateMap<TMDbSeasonResponse, SeasonDto>()
            .ForMember(dest => dest.TMDbSeasonId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes))
            .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.Episodes.Count));

        CreateMap<TMDbEpisodeResponse, EpisodeDto>()
            .ForMember(dest => dest.TMDbEpisodeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AirDate, opt => opt.MapFrom(src => ParseDate(src.AirDate)))
            .ForMember(dest => dest.Runtime, opt => opt.MapFrom(src => src.Runtime));
    }

    private static DateTime ParseDate(string dateString)
    {
        return DateTime.TryParse(dateString, out DateTime date) ? date : default;
    }
}