using ProductsCdk.Models;
using ProductsCdk.Stacks;

namespace ProductsCdk;

sealed class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        
        // Sandbox
        var sandboxContext = CdkContext.Create(app.Node.TryGetContext("sandbox"));
        var sandboxStack = new ProductServiceStack(app, "products", new ProductServiceStackProps(sandboxContext));
        
        app.Synth();
    }
}