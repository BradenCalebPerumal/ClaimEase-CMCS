using FluentValidation;
using TaskOneDraft.Models;

public class ClaimsValidator : AbstractValidator<Claims>
{
    public ClaimsValidator()
    {


        // Rule 1: Maximum working hours
        RuleFor(c => c.HoursWorked)
            .LessThanOrEqualTo(45)
            .WithMessage("Maximum working hours exceeded. The claim will be rejected.");

        // Rule 2: Maximum overtime hours
        RuleFor(c => c.OvertimeHours)
            .LessThanOrEqualTo(10)
            .When(c => c.OvertimeHours.HasValue)
            .WithMessage("Maximum overtime hours exceeded. The claim will be rejected.");

        // Rule 3: Minimum hourly rate
        RuleFor(c => c.RateHour)
            .GreaterThanOrEqualTo(50)
            .WithMessage("Hourly rate below the minimum allowed. The claim will be rejected.");

    }
}
