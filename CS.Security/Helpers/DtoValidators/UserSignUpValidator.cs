using CS.Security.DTO;
using FluentValidation;

namespace CS.Security.Helpers.DtoValidators;

public class UserSignUpValidator : AbstractValidator<UserSignUpDto>
{
    public UserSignUpValidator()
    {
        RuleFor(u => u.UserName)
            .NotNull()
            .MaximumLength(50)
            .Length(4)
            .WithMessage("Not valid name");

        RuleFor(u => u.Email)
            .NotNull()
            .EmailAddress()
            .WithMessage("Invalid email");

        RuleFor(u => u.Password)
            .NotNull()
            .WithMessage("Invalid password");
    }
}