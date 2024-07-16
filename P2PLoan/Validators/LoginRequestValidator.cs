using FluentValidation;
using P2PLoan.DTOs.Requests;

namespace P2PLoan.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDTO>
{
    // Invokes set of rule for CreateorUpdateCategoryDto
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email must not be empty")
            .MaximumLength(150).EmailAddress().WithMessage("Please provide a valid email");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must not be empty").MinimumLength(6);
    }
}