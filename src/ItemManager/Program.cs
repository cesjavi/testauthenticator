using ItemManager.Core.Models;
using ItemManager.Core.Services;
using ItemManager.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<ItemRepository>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<TotpService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<SessionValidationFilter>();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    name = "Item Manager Mockup",
    message = "Bienvenido al mockup de ABM de items con login mediante Google Authenticator",
    endpoints = new[]
    {
        "POST /auth/login",
        "GET /auth/users",
        "GET /items",
        "GET /items/{id}",
        "POST /items",
        "PUT /items/{id}",
        "DELETE /items/{id}"
    }
}));

app.MapGet("/auth/users", (UserStore store, TotpService totp) =>
{
    var issuer = "ItemManager";
    var users = store.GetAll().Select(user => new
    {
        user.Username,
        user.DisplayName,
        SecretKey = user.SecretKey,
        QrUri = totp.BuildOtpAuthUri(user, issuer)
    });

    return Results.Json(users);
});

app.MapPost("/auth/login", async (HttpContext context, AuthService authService) =>
{
    var loginRequest = await context.Request.ReadFromJsonAsync<LoginRequest>();
    if (loginRequest is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var loginResult = authService.TryLogin(loginRequest);
    if (!loginResult.Success)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(new
    {
        token = loginResult.SessionToken,
        user = new
        {
            loginResult.User!.Username,
            loginResult.User.DisplayName
        }
    });
});

var items = app.MapGroup("/items");
items.AddEndpointFilter<SessionValidationFilter>();

items.MapGet("/", (ItemRepository repo) => Results.Json(repo.GetAll()));

items.MapGet("/{id:int}", (int id, ItemRepository repo) =>
{
    var item = repo.GetById(id);
    return item is null ? Results.NotFound() : Results.Json(item);
});

items.MapPost("/", async (HttpContext context, ItemRepository repo) =>
{
    var item = await context.Request.ReadFromJsonAsync<ItemInput>();
    if (item is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var created = repo.Create(item);
    return Results.Created($"/items/{created.Id}", created);
});

items.MapPut("/{id:int}", async (int id, HttpContext context, ItemRepository repo) =>
{
    var item = await context.Request.ReadFromJsonAsync<ItemInput>();
    if (item is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var updated = repo.Update(id, item);
    return updated is null ? Results.NotFound() : Results.Json(updated);
});

items.MapDelete("/{id:int}", (int id, ItemRepository repo) =>
{
    var deleted = repo.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
