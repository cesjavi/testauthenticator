using System.Text;
using ItemManager.Core.Models;
using ItemManager.Core.Services;

namespace ItemManager.Gui;

public partial class LoginForm : Form
{
    private readonly AuthService _authService;
    private readonly ItemRepository _itemRepository;
    private readonly TotpService _totpService;
    private readonly UserStore _userStore;
    private const string Issuer = "ItemManager";

    public LoginForm(AuthService authService, ItemRepository itemRepository, TotpService totpService, UserStore userStore)
    {
        _authService = authService;
        _itemRepository = itemRepository;
        _totpService = totpService;
        _userStore = userStore;
        InitializeComponent();
    }

    private void LoginButton_Click(object? sender, EventArgs e)
    {
        messageLabel.Text = string.Empty;

        var username = usernameTextBox.Text.Trim();
        var password = passwordTextBox.Text;
        var otpCode = otpTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(otpCode))
        {
            messageLabel.Text = "Completá todos los campos";
            return;
        }

        var loginRequest = new LoginRequest(username, password, otpCode);
        var result = _authService.TryLogin(loginRequest);
        if (!result.Success || result.User is null)
        {
            messageLabel.Text = "Credenciales o código inválidos";
            otpTextBox.SelectAll();
            otpTextBox.Focus();
            return;
        }

        Hide();
        using var itemsForm = new ItemsForm(result.User, _itemRepository);
        var dialogResult = itemsForm.ShowDialog(this);
        if (dialogResult == DialogResult.OK)
        {
            usernameTextBox.Clear();
            passwordTextBox.Clear();
            otpTextBox.Clear();
            messageLabel.Text = string.Empty;
            Show();
            usernameTextBox.Focus();
        }
        else
        {
            Close();
        }
    }

    private void OtpInfoLink_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var builder = new StringBuilder();
        foreach (var user in _userStore.GetAll())
        {
            builder.AppendLine($"Usuario: {user.DisplayName} ({user.Username})");
            builder.AppendLine($"Secreto: {user.SecretKey}");
            builder.AppendLine($"URI: {_totpService.BuildOtpAuthUri(user, Issuer)}");
            builder.AppendLine();
        }

        MessageBox.Show(builder.ToString(), "Configurar Google Authenticator", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void RegisterButton_Click(object? sender, EventArgs e)
    {
        using var registerForm = new RegisterUserForm(_userStore, _totpService, Issuer);
        registerForm.ShowDialog(this);
    }
}
