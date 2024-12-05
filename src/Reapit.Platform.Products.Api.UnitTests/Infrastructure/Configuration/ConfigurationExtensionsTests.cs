using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Reapit.Platform.Cloud.Services.SecretsManager;
using Reapit.Platform.Cloud.Services.SimpleSystemsManagement;
using Reapit.Platform.Products.Api.Infrastructure.Configuration;

namespace Reapit.Platform.Products.Api.UnitTests.Infrastructure.Configuration;

public class ConfigurationExtensionsTests
{
    private readonly FakeLogger<Program> _logger = new();
    private readonly IParameterStoreService _parameterStoreService = Substitute.For<IParameterStoreService>();
    private readonly ISecretsService _secretsService = Substitute.For<ISecretsService>();
    private readonly IConfigurationManager _configuration = new ConfigurationManager();
    
    private const string ParameterName = "/Platform/Test/Parameter";
    private const string DatabaseSecretName = "/Platform/Database/User";
    
    /*
     * InjectRemoteConfiguration
     */
    
    [Fact]
    public void InjectRemoteConfiguration_ReturnsWithoutChanges_WhenEnvironmentIsDevelopment()
    {
        var sut = CreateSut("Development");
        _ = sut.InjectRemoteConfiguration();
        _logger.LatestRecord.Message.Should().Be("Application running in Development environment - using local configuration.");
    }

    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenRemoteConfigurationParameterNameNotSet()
    {
        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage("RemoteConfigurationParameter value not found in local configuration.");
    }

    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenRemoteConfigurationNotFound()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new ("RemoteConfigurationParameter", ParameterName),
        });
        
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(null));
        
        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage($"Failed to retrieve parameter: \"{ParameterName}\"");
    }
    
    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenRemoteConfigurationDoesNotIncludeIdpParameterName()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new ("RemoteConfigurationParameter", ParameterName),
        });
        
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(new { Database = new { User = "user", Name = "" } }));
        
        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage("Identity provider client credentials parameter name not configured.");
    }
    
    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenRemoteConfigurationDoesNotIncludeIdpParameterNotFound()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new ("RemoteConfigurationParameter", ParameterName),
        });

        const string idpParameterName = "/Test/IdPCredentials";
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(new { IdentityProviderClientDetails = idpParameterName, Database = new { User = "user", Name = "" } }));

        _parameterStoreService.GetParameterAsync(idpParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(null));
        
        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage("Identity provider client credentials parameter not configured.");
    }

    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenDatabaseConfigurationNotInjected()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new ("RemoteConfigurationParameter", ParameterName),
        });
        
        const string idpParameterName = "/Test/IdPCredentials";
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(new { IdentityProviderClientDetails = idpParameterName, Database = new { User = "user", Name = "" } }));
        
        _parameterStoreService.GetParameterAsync(idpParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(IdpCredentials));
        
        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage("Malformed database configuration.");
    }

    [Fact]
    public void InjectRemoteConfiguration_ThrowsException_WhenUserSecretNotFound()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new ("RemoteConfigurationParameter", ParameterName),
        });
        
        const string idpParameterName = "/Test/IdPCredentials";
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(new { IdentityProviderClientDetails = idpParameterName, Database = new { User = DatabaseSecretName, Name = "database" } }));
        
        _parameterStoreService.GetParameterAsync(idpParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(IdpCredentials));

        _secretsService.GetAsync(DatabaseSecretName, null, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(null));

        var sut = CreateSut();
        var action = () => sut.InjectRemoteConfiguration();
        action.Should().Throw<Exception>()
            .WithMessage("Failed to retrieve database user credentials.");
    }

    [Fact]
    public void InjectRemoteConfiguration_InjectsConfiguration_FromRemoteParameters_AndUserSecret()
    {
        _configuration.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
         {
             new ("RemoteConfigurationParameter", ParameterName),
         });
        
        const string idpParameterName = "/Test/IdPCredentials";
        _parameterStoreService.GetParameterAsync(ParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(new { IdentityProviderClientDetails = idpParameterName, Database = new { User = DatabaseSecretName, Name = "database" } }));

        _parameterStoreService.GetParameterAsync(idpParameterName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(IdpCredentials));
        
        var userSecret = new DatabaseUserSecretModel("uid", "pwd", "engine", "host", 3306, "cluster");
        _secretsService.GetAsync(DatabaseSecretName, null, Arg.Any<CancellationToken>())
            .Returns(Serialize(userSecret));

        var sut = CreateSut();
        _ = sut.InjectRemoteConfiguration();
        sut.Configuration.GetConnectionString("Writer").Should().Be(userSecret.GetConnectionString("database"));
        sut.Configuration.GetValue<string>("IdP:ClientSecret").Should().Be("secret-string");
    }
    
    /*
     * Private methods
     */
    
    private static string Serialize(object o)
        => JsonSerializer.Serialize(o);

    private IHostApplicationBuilder CreateSut(string environmentName = "Production")
    {
        var sut = Substitute.For<IHostApplicationBuilder>();

        // Set environment
        sut.Environment.Returns(new HostingEnvironment { EnvironmentName = environmentName });

        // Set configuration
        sut.Configuration.Returns(_configuration);

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<Program>>(_logger)
            .AddSingleton(_parameterStoreService)
            .AddSingleton(_secretsService);

        sut.Services.Returns(services);
        
        return sut;
    }

    private static object IdpCredentials => new
    {
        Domain = "domain.net",
        ClientId = "client-id",
        ClientSecret = "secret-string",
        DefaultConnection = "users",
        MyAccountClientId = "my-account-client-id",
        ResetPasswordTimeToLive = 86400,
        TokenCacheSeconds = 3600
    };
}