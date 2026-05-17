using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit3;
using Shouldly;

namespace Api.IntegrationTests;

public class FruitApiTests4(ApiFixture api) : IClassFixture<ApiFixture>
{
    [Theory]
    [AutoData]
    public async Task InsertFruit_ReturnsCreated(Fruit request)
    {
        var response = await api.Client.PostAsJsonAsync("/fruit", request, TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Theory]
    [AutoData]
    public async Task InsertFruit_SecondCall_ReturnsConflict(Fruit request)
    {
        await api.Client.PostAsJsonAsync("/fruit", request, TestContext.Current.CancellationToken);
        var response = await api.Client.PostAsJsonAsync("/fruit", request, TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Theory]
    [AutoData]
    public async Task GetFruit_ReturnsInsertedFruits(Fruit firstFruit, Fruit secondFruit)
    {
        var ct = TestContext.Current.CancellationToken;

        await api.Client.PostAsJsonAsync("/fruit", firstFruit, ct);
        await api.Client.PostAsJsonAsync("/fruit", secondFruit, ct);

        var response = await api.Client.GetAsync("/fruit", ct);
        var fruits = await response.Content.ReadFromJsonAsync<Fruit[]>(ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fruits.ShouldNotBeNull();
        var fruitNames = fruits.Select(f => f.Name).ToHashSet();
        fruitNames.ShouldContain(firstFruit.Name);
        fruitNames.ShouldContain(secondFruit.Name);
    }
}