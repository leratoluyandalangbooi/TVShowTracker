namespace TVShowTracker.Application.Validators.Show;

public class GetShowDetailsDtoValidator : AbstractValidator<GetShowDetailsDto>
{
    public GetShowDetailsDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
