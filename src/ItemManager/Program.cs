using System.Linq;
using ItemManager.Core.Models;
using ItemManager.Core.Services;
using ItemManager.Filters;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<ItemRepository>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<TotpService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<PushDeviceStore>();
builder.Services.AddSingleton<PushChallengeService>();
builder.Services.AddSingleton<PushAuthService>();
builder.Services.AddSingleton<SessionValidationFilter>();
builder.Services.Configure<FirebasePushOptions>(builder.Configuration.GetSection("Firebase"));
builder.Services.AddHttpClient<FirebasePushNotificationService>(client =>
{
    client.BaseAddress = new Uri("https://fcm.googleapis.com/");
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => Results.Json(new
{
    name = "Item Manager Mockup",
    message = "Bienvenido al mockup de ABM de items con login mediante Google Authenticator y push de Firebase",
    endpoints = new[]
    {
        "POST /auth/login",
        "POST /auth/register",
        "POST /auth/push/register",
        "POST /auth/push/login",
        "POST /auth/push/confirm",
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

app.MapPost("/auth/push/register", async (HttpContext context, PushAuthService pushAuthService) =>
{
    var registerRequest = await context.Request.ReadFromJsonAsync<PushDeviceRegistrationRequest>();
    if (registerRequest is null)
    {
        //return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var result = pushAuthService.RegisterDevice(registerRequest);
    if (!result.Success || result.Device is null)
    {
        return Results.BadRequest(new { error = result.Error ?? "No se pudo registrar el dispositivo." });
    }

    return Results.Ok(new
    {
        device = new
        {
            result.Device.DeviceId,
            result.Device.DeviceName,
            result.Device.RegisteredAt
        }
    });
});

app.MapPost("/auth/push/login", async (HttpContext context, PushAuthService pushAuthService) =>
{
    var pushLoginRequest = await context.Request.ReadFromJsonAsync<PushLoginRequest>();
    if (pushLoginRequest is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var result = await pushAuthService.InitiateLoginAsync(pushLoginRequest, context.RequestAborted);
    if (!result.Success || result.Challenge is null || result.User is null)
    {
        return result.FailureReason switch
        {
            PushLoginFailureReason.InvalidCredentials => Results.Unauthorized(),
            PushLoginFailureReason.NoDevicesRegistered => Results.Json(new { error = result.Error ?? "No hay dispositivos registrados." }, statusCode: StatusCodes.Status409Conflict),
            PushLoginFailureReason.NotificationError => Results.Json(new { error = result.Error ?? "No se pudo enviar la notificación push." }, statusCode: StatusCodes.Status502BadGateway),
            _ => Results.BadRequest(new { error = result.Error ?? "No se pudo iniciar el desafío push." })
        };
    }

    return Results.Accepted($"/auth/push/challenges/{result.Challenge.Id}", new
    {
        challenge = new
        {
            result.Challenge.Id,
            result.Challenge.ExpiresAt,
            devices = result.Challenge.Targets.Select(t => new { t.DeviceId, t.DeviceName })
        },
        user = new
        {
            result.User.Username,
            result.User.DisplayName
        }
    });
});

app.MapPost("/auth/push/confirm", async (HttpContext context, PushAuthService pushAuthService) =>
{
    var confirmationRequest = await context.Request.ReadFromJsonAsync<PushLoginConfirmationRequest>();
    if (confirmationRequest is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var result = pushAuthService.CompleteLogin(confirmationRequest);
    if (!result.Success || result.Login is null)
    {
        if (result.Expired)
        {
            return Results.Json(new { error = result.Error ?? "El desafío expiró." }, statusCode: StatusCodes.Status410Gone);
        }

        return Results.BadRequest(new { error = result.Error ?? "No se pudo completar el desafío." });
    }

    if (result.Login.User is null || string.IsNullOrWhiteSpace(result.Login.SessionToken))
    {
        return Results.BadRequest(new { error = "No se pudo crear la sesión." });
    }

    return Results.Ok(new
    {
        token = result.Login.SessionToken,
        user = new
        {
            result.Login.User.Username,
            result.Login.User.DisplayName
        }
    });
});

app.MapPost("/auth/register", async (HttpContext context, UserStore userStore, TotpService totpService) =>
{
    var registerRequest = await context.Request.ReadFromJsonAsync<RegisterUserRequest>();
    if (registerRequest is null)
    {
        return Results.BadRequest(new { error = "Solicitud inválida" });
    }

    var username = registerRequest.Username?.Trim();
    var displayName = registerRequest.DisplayName?.Trim();
    var password = registerRequest.Password?.Trim();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(password))
    {
        return Results.BadRequest(new { error = "Todos los campos son obligatorios." });
    }

    var newUser = new User
    {
        Username = username,
        DisplayName = displayName,
        Password = password,
        SecretKey = totpService.GenerateSecretKey()
    };

    if (!userStore.Add(newUser))
    {
        return Results.Conflict(new { error = "El usuario ya existe." });
    }

    var issuer = "ItemManager";
    var otpAuthUri = totpService.BuildOtpAuthUri(newUser, issuer);

    return Results.Ok(new
    {
        user = new
        {
            newUser.Username,
            newUser.DisplayName
        },
        secretKey = newUser.SecretKey,
        otpAuthUri
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
