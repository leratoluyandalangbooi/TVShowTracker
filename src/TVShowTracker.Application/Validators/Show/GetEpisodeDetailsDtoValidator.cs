namespace TVShowTracker.Application.Validators.Show;

public class GetEpisodeDetailsDtoValidator : AbstractValidator<GetEpisodeDetailsDto>
{
    public GetEpisodeDetailsDtoValidator()
    {
        RuleFor(x => x.ShowId).NotEmpty();
        RuleFor(x => x.SeasonNumber).NotEmpty();
        RuleFor(x => x.EpisodeNumber).NotEmpty();
    }
}


