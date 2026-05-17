using System.Net;
using System.Net.Http.Json;
using Shouldly;

namespace Api.IntegrationTests;

public class FruitApiTests2(ApiFixture api) : IClassFixture<ApiFixture>
{
    [Fact]
    public async Task InsertFruit_ReturnsCreated()
    {
        var response =
            await api.Client.PostAsJsonAsync("/fruit", new { Name = "Apple" }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetFruit_ReturnsInsertedFruits()
    {
        var ct = TestContext.Current.CancellationToken;

        await api.Client.PostAsJsonAsync("/fruit", new { Name = "Mango" }, ct);
        await api.Client.PostAsJsonAsync("/fruit", new { Name = "Papaya" }, ct);

        var response = await api.Client.GetAsync("/fruit", ct);
        var fruits = await response.Content.ReadFromJsonAsync<Fruit[]>(ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fruits.ShouldNotBeNull();
        fruits.Select(f => f.Name).ShouldContain("Mango");
        fruits.Select(f => f.Name).ShouldContain("Papaya");
    }
}