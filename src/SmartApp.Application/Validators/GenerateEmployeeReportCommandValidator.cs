namespace SmartApp.Application.Validators;

using FluentValidation;
using SmartApp.Application.Commands.GenerateEmployeeReport;

public sealed class GenerateEmployeeReportCommandValidator
    : AbstractValidator<GenerateEmployeeReportCommand>
{
    public GenerateEmployeeReportCommandValidator()
    {
        RuleFor(x => x.RequestedBy)
            .NotEmpty().WithMessage("RequestedBy is required.")
            .MaximumLength(100);

        RuleFor(x => x.OutputDirectory)
            .NotEmpty().WithMessage("OutputDirectory is required.")
            .MaximumLength(500);
    }
}