using Testcontainers.PostgreSql;

namespace Api.IntegrationTests;

public class PostgresAssemblyFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
    }

    public string GetConnectionString()
    {
        return _container.GetConnectionString();
    }
}