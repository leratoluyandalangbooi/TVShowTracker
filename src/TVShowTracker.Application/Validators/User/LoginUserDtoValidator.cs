using TVShowTracker.Application.DTOs.Request;

namespace TVShowTracker.Application.Validators.User;

public class LoginUserDtoValidator : AbstractValidator<LoginDto>
{
    public LoginUserDtoValidator() 
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(5);
    }

    
}
