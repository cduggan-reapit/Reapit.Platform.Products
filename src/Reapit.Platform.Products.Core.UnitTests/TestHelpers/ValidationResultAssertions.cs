using System.Text;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentValidation.Results;

namespace Reapit.Platform.Products.Core.UnitTests.TestHelpers;


public class ValidationResultAssertions(ValidationResult instance)
    : ReferenceTypeAssertions<ValidationResult, ValidationResultAssertions>(instance)
{
    protected override string Identifier => "ValidationResult";

    [CustomAssertion]
    public AndConstraint<ValidationResultAssertions> Pass(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsValid)
            .FailWith(GetFailedAssertionMessage($"{Identifier} does not indicate successful validation."));

        return new AndConstraint<ValidationResultAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<ValidationResultAssertions> Fail(
        string propertyName,
        string? errorMessage = null,
        string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            // Should indicate failure
            .ForCondition(!Subject.IsValid)
            .FailWith($"{Identifier} status does not indicate validation failure.")
            .Then
            // Should have one error
            .ForCondition(Subject.Errors.Count == 1)
            .FailWith(GetFailedAssertionMessage("More than one error detected."))
            .Then
            // That error should match the propertyName...
            .ForCondition(Subject.Errors.Single().PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            .FailWith(GetFailedAssertionMessage($"{Identifier} does not contain error for property \"{propertyName}\"."))
            .Then
            // ... and the error message (if it was provided)
            .ForCondition(errorMessage == null || Subject.Errors.Single().ErrorMessage.Equals(errorMessage, StringComparison.OrdinalIgnoreCase))
            .FailWith(GetFailedAssertionMessage($"{Identifier} does not contain error for property \"{propertyName}\" with message: \"{errorMessage}\""));

        return new AndConstraint<ValidationResultAssertions>(this);
    }

    private string GetFailedAssertionMessage(string reason)
    {
        var sb = new StringBuilder();
        sb.AppendLine(reason);
        sb.AppendLine($"Validation state: {Subject.IsValid}");
        if (Subject.IsValid)
            return sb.ToString();

        sb.AppendLine(new string('-', 50));
        foreach (var error in Subject.Errors)
            sb.AppendLine($"  {error.PropertyName}: {error.ErrorMessage}");
        sb.AppendLine(new string('-', 50));
        return sb.ToString();
    }
}