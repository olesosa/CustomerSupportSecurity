using CS.Security.DTO;
using FluentValidation;

namespace CS.Security.Helpers.DtoValidators;

public class AdminCreateDtoValidator: AbstractValidator<AdminCreateDto>
{
    public AdminCreateDtoValidator()
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