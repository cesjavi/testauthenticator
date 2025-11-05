using System.Text.Json;
using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class UserStore
{
    private readonly object _syncRoot = new();
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly List<User> _users;

    public UserStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dataDirectory = Path.Combine(appData, "ItemManager");
        Directory.CreateDirectory(dataDirectory);

        _filePath = Path.Combine(dataDirectory, "users.json");
        _users = LoadUsers();
    }

    public User? FindByUsername(string username)
    {
        lock (_syncRoot)
        {
            return _users.FirstOrDefault(user =>
                string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
        }
    }

    public IEnumerable<User> GetAll()
    {
        lock (_syncRoot)
        {
            return _users.Select(user => user).ToList();
        }
    }

    public bool Add(User user)
    {
        lock (_syncRoot)
        {
            if (_users.Any(existing =>
                    string.Equals(existing.Username, user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            _users.Add(user);
            SaveUsers();
            return true;
        }
    }

    private List<User> LoadUsers()
    {
        if (!File.Exists(_filePath))
        {
            var defaults = GetDefaultUsers();
            SaveUsers(defaults);
            return defaults;
        }

        try
        {
            using var stream = File.OpenRead(_filePath);
            var users = JsonSerializer.Deserialize<List<User>>(stream);
            if (users is { Count: > 0 })
            {
                return users;
            }
        }
        catch
        {
            // Si falla la lectura, recreamos el archivo con los usuarios por defecto.
        }

        var fallback = GetDefaultUsers();
        SaveUsers(fallback);
        return fallback;
    }

    private void SaveUsers()
    {
        SaveUsers(_users);
    }

    private void SaveUsers(List<User> users)
    {
        using var stream = File.Create(_filePath);
        JsonSerializer.Serialize(stream, users, _serializerOptions);
        stream.Flush();
    }

    private static List<User> GetDefaultUsers() =>
    [
        new User
        {
            Username = "admin",
            DisplayName = "Administrador",
            Password = "admin123",
            SecretKey = "JBSWY3DPEHPK3PXP"
        },
        new User
        {
            Username = "invitado",
            DisplayName = "Invitado",
            Password = "guest123",
            SecretKey = "NB2W45DFOIZA===="
        }
    ];
}
