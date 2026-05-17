using Dapper;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddScoped(_ => new NpgsqlConnection(connectionString));

var app = builder.Build();

app.MapPost("/fruit", async (NpgsqlConnection db, FruitRequest request) =>
{
    await db.ExecuteAsync("INSERT INTO Fruit(Name) VALUES (@Name)", new { request.Name });
    return Results.Created();
});

app.MapGet("/fruit", async (NpgsqlConnection db) =>
{
    var fruits = await db.QueryAsync<Fruit>("SELECT FruitId, Name FROM Fruit");
    return Results.Ok(fruits);
});

app.Run();

internal record FruitRequest(string Name);

internal record Fruit(int FruitId, string Name);

public partial class Program;