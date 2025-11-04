using ItemManager.Models;

namespace ItemManager.Services;

public class UserStore
{
    private readonly List<User> _users =
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

    public User? FindByUsername(string username) =>
        _users.FirstOrDefault(user => string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<User> GetAll() => _users.AsReadOnly();
}
