namespace Reapit.Platform.Products.Core.UseCases.Applications;

public static class ApplicationValidationMessages
{
    public const string NameTooLong = "Exceeds maximum length of 100 characters";
    
    public const string DescriptionTooLong = "Exceeds maximum length of 1000 characters";
    
    public const string ClientsPreventingDelete = "Cannot delete an application with active clients";
}