using FluentValidation;
using RinhaBackendAPI2024Q1.Models.Requests;

namespace RinhaBackendAPI2024Q1.Validators;

public class CreateNewTransacaoRequestValidator : AbstractValidator<CreateNewTransacaoRequest>
{
    public CreateNewTransacaoRequestValidator()
    {
        RuleFor(f => f.descricao)
            .NotNull()
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(f => f.valor)
            .GreaterThan(0)
            .Must(f => f % 1 == 0);

        RuleFor(f => f.tipo)
            .Must(f => f is 'c' or 'd');
    }
}