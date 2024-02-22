using FluentValidation;

namespace AppServices.Validations
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .NotNull()
                .Must(x => x?.Split(" ").Length >= 1)
                .WithMessage("Lastname deve conter ao menos um sobrenome");

            RuleFor(x => x.Age)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.NickName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(3);
        }
    }
}
