using System.Text;
using Reapit.Platform.Cloud.Services.SecretsManager;
using Reapit.Platform.Cloud.Services.SimpleSystemsManagement;
using Reapit.Platform.Products.Core.Extensions;

namespace Reapit.Platform.Products.Api.Infrastructure.Configuration;


/// <summary>Extension class for injecting remote configuration data.</summary>
public static class ConfigurationExtensions
{
    /// <summary>Adds configuration objects from AWS to the builders configuration provider. </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>A reference to the web application builder after the operation.</returns>
    /// <exception cref="Exception">failed to retrieve/parse configuration object from SSM.</exception>
    /// <remarks>Does not alter the configuration object unless the application is running in a production environment.</remarks>
    public static IHostApplicationBuilder InjectRemoteConfiguration(this IHostApplicationBuilder builder)
    {
        using var serviceProvider = builder.Services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        if (builder.Environment.IsDevelopment())
        {
            logger.LogWarning("Application running in {environment} environment - using local configuration.", 
                builder.Environment.EnvironmentName);
            return builder;
        }
        
        // Inject the SSM parameter
        var parameterName = builder.Configuration.GetValue<string>("RemoteConfigurationParameter")
                            ?? throw new Exception("RemoteConfigurationParameter value not found in local configuration.");

        var ssmClient = scope.ServiceProvider.GetRequiredService<IParameterStoreService>();
        var parameterValue = ssmClient.GetParameterAsync(parameterName).Result
                             ?? throw new Exception($"Failed to retrieve parameter: \"{parameterName}\"");

        using var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes($"{{ \"Service\": {parameterValue} }}"));
        builder.Configuration.AddJsonStream(jsonStream);
        
        // Inject the database user credentials
        var databaseConfiguration = builder.Configuration.ReadDatabaseConfiguration();
        var secretClient = scope.ServiceProvider.GetRequiredService<ISecretsService>();
        
        var userSecretString = secretClient.GetAsync(databaseConfiguration.UserSecretPath).Result
            ?? throw new Exception("Failed to retrieve database user credentials.");
        var userSecret = userSecretString.DeserializeTo<DatabaseUserSecretModel>();
        using var secretStream = new MemoryStream(Encoding.UTF8.GetBytes(
                $"{{ \"ConnectionStrings\": {{ \"Writer\": \"{userSecret.GetConnectionString(databaseConfiguration.Name)}\" }} }}"));
        builder.Configuration.AddJsonStream(secretStream);
        
        return builder;
    }

    private static DatabaseConfigurationModel ReadDatabaseConfiguration(this IConfiguration configuration)
    {
        var name = configuration.GetValue<string?>("Service:database:name");
        var user = configuration.GetValue<string?>("Service:database:user");
        
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(user))
            throw new Exception("Malformed database configuration.");
        
        return new DatabaseConfigurationModel(name, user);
    }
}