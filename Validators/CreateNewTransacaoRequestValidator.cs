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
            .NotNull()
            .NotEmpty();

        RuleFor(f => f.tipo)
            .NotNull()
            .NotEmpty()
            .Must(f => f is 'c' or 'd');
    }
}