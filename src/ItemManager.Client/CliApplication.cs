using ItemManager.ApiClient;
using ItemManager.Core.Models;

namespace ItemManager.ClientApp;

internal class CliApplication
{
    private readonly ItemManagerApiClient _apiClient;
    private readonly Uri _baseUri;
    private AuthSession? _session;

    public CliApplication(ItemManagerApiClient apiClient, Uri baseUri)
    {
        _apiClient = apiClient;
        _baseUri = baseUri;
    }

    public async Task RunAsync()
    {
        PrintHeader();

        while (true)
        {
            PrintMenu();
            Console.Write("Selecciona una opción: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            switch (input.Trim())
            {
                case "0":
                    Console.WriteLine("Hasta luego 👋");
                    return;
                case "1":
                    await HandleLoginAsync();
                    break;
                case "2":
                    await RequireSessionAsync(ListItemsAsync);
                    break;
                case "3":
                    await RequireSessionAsync(CreateItemAsync);
                    break;
                case "4":
                    await RequireSessionAsync(UpdateItemAsync);
                    break;
                case "5":
                    await RequireSessionAsync(DeleteItemAsync);
                    break;
                case "6":
                    HandleLogout();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Opción no reconocida. Intenta nuevamente.");
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine();
        }
    }

    private void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================");
        Console.WriteLine(" Item Manager - Cliente de referencia CLI");
        Console.WriteLine("============================================");
        Console.ResetColor();
        Console.WriteLine($"Base URL: {_baseUri}");
        Console.WriteLine("Escribe el número de la opción y presiona Enter.\n");
    }

    private void PrintMenu()
    {
        Console.WriteLine("Menú disponible:");
        Console.WriteLine("  1) Iniciar sesión");
        Console.WriteLine("  2) Listar items");
        Console.WriteLine("  3) Crear item");
        Console.WriteLine("  4) Actualizar item");
        Console.WriteLine("  5) Eliminar item");
        Console.WriteLine("  6) Cerrar sesión");
        Console.WriteLine("  0) Salir");

        if (_session is not null)
        {
            Console.WriteLine($"\nSesión activa: {_session.DisplayName} ({_session.Username})");
        }
    }

    private async Task HandleLoginAsync()
    {
        Console.WriteLine("\n== Iniciar sesión ==");
        var username = ReadRequired("Usuario");
        var password = ReadPassword("Contraseña");
        var otpCode = ReadRequired("Código TOTP (6 dígitos)");

        var loginResult = await _apiClient.LoginAsync(username, password, otpCode);
        if (!loginResult.Success)
        {
            ShowError(loginResult.ErrorMessage ?? "No fue posible iniciar sesión.");
            return;
        }

        _session = loginResult.Payload;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Bienvenido {_session!.DisplayName}. Tu token de sesión expira según la configuración del servidor.");
        Console.ResetColor();
    }

    private async Task ListItemsAsync(AuthSession session)
    {
        Console.WriteLine("\n== Items disponibles ==");
        var result = await _apiClient.GetItemsAsync(session.Token);
        if (!result.Success)
        {
            ShowError(result.ErrorMessage ?? "No se pudieron obtener los items.");
            if (result.RequiresReauthentication)
            {
                _session = null;
            }
            return;
        }

        var items = result.Payload;
        if (items is null || items.Count == 0)
        {
            Console.WriteLine("No hay items cargados.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"- #{item.Id}: {item.Name} (Cantidad: {item.Quantity})");
            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                Console.WriteLine($"    Descripción: {item.Description}");
            }
        }
    }

    private async Task CreateItemAsync(AuthSession session)
    {
        Console.WriteLine("\n== Crear nuevo item ==");
        var name = ReadRequired("Nombre");
        Console.Write("Descripción (opcional): ");
        var description = Console.ReadLine();
        var quantity = ReadInt("Cantidad");

        var input = new ItemInput(name, string.IsNullOrWhiteSpace(description) ? null : description, quantity);
        var result = await _apiClient.CreateItemAsync(session.Token, input);
        if (!result.Success)
        {
            ShowError(result.ErrorMessage ?? "No se pudo crear el item.");
            if (result.RequiresReauthentication)
            {
                _session = null;
            }
            return;
        }

        var created = result.Payload!;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Item #{created.Id} creado correctamente.");
        Console.ResetColor();
    }

    private async Task UpdateItemAsync(AuthSession session)
    {
        Console.WriteLine("\n== Actualizar item ==");
        var id = ReadInt("ID del item");
        var name = ReadRequired("Nuevo nombre");
        Console.Write("Nueva descripción (opcional): ");
        var description = Console.ReadLine();
        var quantity = ReadInt("Nueva cantidad");

        var input = new ItemInput(name, string.IsNullOrWhiteSpace(description) ? null : description, quantity);
        var result = await _apiClient.UpdateItemAsync(session.Token, id, input);
        if (!result.Success)
        {
            ShowError(result.ErrorMessage ?? "No se pudo actualizar el item.");
            if (result.RequiresReauthentication)
            {
                _session = null;
            }
            return;
        }

        var updated = result.Payload!;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Item #{updated.Id} actualizado correctamente.");
        Console.ResetColor();
    }

    private async Task DeleteItemAsync(AuthSession session)
    {
        Console.WriteLine("\n== Eliminar item ==");
        var id = ReadInt("ID del item a eliminar");
        var result = await _apiClient.DeleteItemAsync(session.Token, id);
        if (!result.Success)
        {
            ShowError(result.ErrorMessage ?? "No se pudo eliminar el item.");
            if (result.RequiresReauthentication)
            {
                _session = null;
            }
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Item eliminado correctamente.");
        Console.ResetColor();
    }

    private void HandleLogout()
    {
        if (_session is null)
        {
            Console.WriteLine("No hay una sesión activa.");
            return;
        }

        _session = null;
        Console.WriteLine("Sesión cerrada.");
    }

    private async Task RequireSessionAsync(Func<AuthSession, Task> operation)
    {
        if (_session is null)
        {
            ShowError("Primero necesitas iniciar sesión (opción 1).");
            return;
        }

        await operation(_session);
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            ShowError($"El campo '{label}' es obligatorio.");
        }
    }

    private static string ReadPassword(string label)
    {
        Console.Write($"{label}: ");
        var buffer = new Stack<char>();
        ConsoleKeyInfo keyInfo;
        while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (buffer.Count > 0)
                {
                    buffer.Pop();
                    Console.Write("\b \b");
                }
                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                buffer.Push(keyInfo.KeyChar);
                Console.Write('*');
            }
        }

        Console.WriteLine();
        return new string(buffer.Reverse().ToArray());
    }

    private static int ReadInt(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            var value = Console.ReadLine();
            if (int.TryParse(value, out var parsed) && parsed >= 0)
            {
                return parsed;
            }

            ShowError("Ingresa un número entero válido.");
        }
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
