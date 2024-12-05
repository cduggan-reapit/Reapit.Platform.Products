using ProductsCdk.Models;

namespace ProductsCdk.Stacks;

public class ProductServiceStackProps : StackProps
{
    public ProductServiceStackProps(CdkContext context)
    {
        Context = context;
        Env = new Environment
        {
            Account = Context.AccountId,
            Region = Context.Region
        };
        Description = $"Reapit.Platform.Products service stack ({Context.Environment.FullName})";
        StackName = $"platform-products-stack-{Context.Environment.ShortName}";
    }
    
    public CdkContext Context { get; }
}