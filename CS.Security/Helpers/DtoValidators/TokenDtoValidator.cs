using CS.Security.DTO;
using FluentValidation;

namespace CS.Security.Helpers.DtoValidators;

public class TokenDtoValidator :AbstractValidator<TokenDto>
{
    public TokenDtoValidator()
    {
        RuleFor(t => t.Token)
            .NotNull()
            .WithMessage("Access token is required");
        
        RuleFor(t => t.RefreshToken)
            .NotNull()
            .WithMessage("Refresh token is required");
    }
}