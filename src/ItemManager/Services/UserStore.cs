using System.Text.Json;
using ItemManager.Models;
using Microsoft.Extensions.Hosting;

namespace ItemManager.Services;

public class UserStore
{
    private readonly string _filePath;
    private readonly object _syncRoot = new();
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private List<User> _users;

    public UserStore(IHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
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

    public void AddOrUpdate(User user)
    {
        lock (_syncRoot)
        {
            var existing = _users.FindIndex(u =>
                string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase));

            if (existing >= 0)
            {
                _users[existing] = user;
            }
            else
            {
                _users.Add(user);
            }

            SaveUsers();
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
            // ignored - we will recreate the file with default users below
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
