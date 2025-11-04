using ItemManager.Core.Services;
using ItemManager.Gui.Properties;

namespace ItemManager.Gui;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var userStore = new UserStore();
        var sessionService = new SessionService();
        var totpService = new TotpService();
        var authService = new AuthService(userStore, sessionService, totpService);
        var itemRepository = new ItemRepository();

        using var loginForm = new LoginForm(authService, itemRepository, totpService, userStore);
        Application.Run(loginForm);
    }
}
