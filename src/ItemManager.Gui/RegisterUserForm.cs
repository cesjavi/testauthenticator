using System.Linq;
using System.Windows.Forms;
using ItemManager.Core.Models;
using ItemManager.Core.Services;

namespace ItemManager.Gui;

public partial class RegisterUserForm : Form
{
    private readonly UserStore _userStore;
    private readonly TotpService _totpService;
    private readonly string _issuer;

    public RegisterUserForm(UserStore userStore, TotpService totpService, string issuer)
    {
        _userStore = userStore;
        _totpService = totpService;
        _issuer = issuer;
        InitializeComponent();
    }

    private void RegisterUserForm_Load(object? sender, EventArgs e)
    {
        RefreshUsers();
    }

    private void RefreshUsers(string? usernameToSelect = null)
    {
        var users = _userStore.GetAll().ToList();
        var selectedUsername = usernameToSelect;
        if (string.IsNullOrEmpty(selectedUsername) && userComboBox.SelectedItem is User selectedUser)
        {
            selectedUsername = selectedUser.Username;
        }

        userComboBox.DataSource = null;
        userComboBox.DataSource = users;
        userComboBox.DisplayMember = nameof(User.DisplayName);
        userComboBox.ValueMember = nameof(User.Username);
        if (!string.IsNullOrEmpty(selectedUsername))
        {
            var index = users.FindIndex(u => string.Equals(u.Username, selectedUsername, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                userComboBox.SelectedIndex = index;
            }
            else if (users.Count > 0)
            {
                userComboBox.SelectedIndex = 0;
            }
        }
        else if (users.Count > 0)
        {
            userComboBox.SelectedIndex = 0;
        }
        UpdateSelectedUser();
    }

    private void UpdateSelectedUser()
    {
        if (userComboBox.SelectedItem is not User selectedUser)
        {
            secretTextBox.Text = string.Empty;
            uriTextBox.Text = string.Empty;
            return;
        }

        secretTextBox.Text = selectedUser.SecretKey;
        uriTextBox.Text = _totpService.BuildOtpAuthUri(selectedUser, _issuer);
    }

    private void UserComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateSelectedUser();
    }

    private void CopySecretButton_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(secretTextBox.Text))
        {
            Clipboard.SetText(secretTextBox.Text);
        }
    }

    private void CopyUriButton_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(uriTextBox.Text))
        {
            Clipboard.SetText(uriTextBox.Text);
        }
    }

    private void GenerateButton_Click(object? sender, EventArgs e)
    {
        messageLabel.Text = string.Empty;

        var username = newUsernameTextBox.Text.Trim();
        var displayName = displayNameTextBox.Text.Trim();
        var password = passwordTextBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(password))
        {
            messageLabel.Text = "Completá todos los campos";
            return;
        }

        if (_userStore.FindByUsername(username) is not null)
        {
            messageLabel.Text = "El usuario ya existe";
            return;
        }

        var secret = _totpService.GenerateSecretKey();
        var newUser = new User
        {
            Username = username,
            DisplayName = displayName,
            Password = password,
            SecretKey = secret
        };

        _userStore.Add(newUser);
        RefreshUsers(newUser.Username);
        messageLabel.Text = $"Usuario {username} creado";
        newUsernameTextBox.Clear();
        displayNameTextBox.Clear();
        passwordTextBox.Clear();
    }
}
