using DbUp;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;

namespace Api.IntegrationTests;

public class ApiFixture : IAsyncLifetime
{
    private readonly PostgresAssemblyFixture _postgres;
    private WebApplicationFactory<Program>? _factory;

    public ApiFixture(PostgresAssemblyFixture postgres)
    {
        _postgres = postgres;
    }

    public HttpClient Client { get; private set; } = null!;

    public ValueTask InitializeAsync()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(_postgres.GetConnectionString())
        {
            Database = $"api_test_{Guid.NewGuid():N}"
        }.ConnectionString;

        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        DeployChanges.To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(ApiFixture).Assembly)
            .Build()
            .PerformUpgrade();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("ConnectionStrings:DefaultConnection", connectionString));

        Client = _factory.CreateClient();

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_factory != null)
            await _factory.DisposeAsync();
    }
}