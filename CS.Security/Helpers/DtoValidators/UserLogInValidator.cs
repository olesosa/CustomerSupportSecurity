using CS.Security.DTO;
using FluentValidation;

namespace CS.Security.Helpers.DtoValidators;

public class UserLogInValidator : AbstractValidator<UserLogInDto>
{
    public UserLogInValidator()
    {
        RuleFor(u => u.Email)
            .NotNull()
            .MaximumLength(50)
            .EmailAddress()
            .WithMessage("Not valid email");

        RuleFor(u => u.Password)
            .NotNull()
            .WithMessage("Not valid password");
    }
}