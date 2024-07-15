using FluentValidation;

namespace TVShowTracker.Application.Validators;

//public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
//{
//    public RegisterUserDtoValidator()
//    {
//        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
//        RuleFor(x => x.Email).NotEmpty().EmailAddress();
//        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
//        RuleFor(x => x.PreferredName).MaximumLength(100);
//    }
//}
