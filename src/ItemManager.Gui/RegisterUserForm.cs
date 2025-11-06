using ItemManager.Models;
using ItemManager.Services;
using QRCoder;
using ItemManager.Core.Services;

using TotpService = ItemManager.Core.Services.TotpService;
using Microsoft.AspNetCore.Identity;
//using UserStore = ItemManager.Services.UserStore;



namespace ItemManager.Gui;

public partial class RegisterUserForm : Form
{
    private readonly ItemManager.Core.Services.UserStore _userStore;
    private readonly TotpService _totpService;
    private readonly string _issuer;
    private readonly System.Windows.Forms.Timer _qrRefreshTimer;

    public RegisterUserForm(ItemManager.Core.Services.UserStore userStore, TotpService totpService, string issuer)
    {
        _userStore = userStore;
        _totpService = totpService;
        _issuer = issuer;
        InitializeComponent();

        _qrRefreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 60_000
        };
        _qrRefreshTimer.Tick += QrRefreshTimer_Tick;
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
        if (userComboBox.SelectedItem is not ItemManager.Core.Models.User selectedUser)
        {
            secretTextBox.Text = string.Empty;
            uriTextBox.Text = string.Empty;
            UpdateQrCode(null);
            _qrRefreshTimer.Stop();
            return;
        }

        secretTextBox.Text = selectedUser.SecretKey;
        RefreshQrCodeFor(selectedUser, restartTimer: true);
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
        var newUser = new ItemManager.Core.Models.User
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

    private void QrRefreshTimer_Tick(object? sender, EventArgs e)
    {
        if (userComboBox.SelectedItem is ItemManager.Core.Models.User selectedUser)
        {
            RefreshQrCodeFor(selectedUser, restartTimer: false);
        }
        else
        {
            _qrRefreshTimer.Stop();
            UpdateQrCode(null);
        }
    }

    private void RefreshQrCodeFor(ItemManager.Core.Models.User selectedUser, bool restartTimer)
    {
        var uri = _totpService.BuildOtpAuthUri(selectedUser, _issuer);
        uriTextBox.Text = uri;
        UpdateQrCode(uri);

        if (restartTimer)
        {
            RestartQrRefreshTimer();
        }
    }

    private void RestartQrRefreshTimer()
    {
        _qrRefreshTimer.Stop();
        _qrRefreshTimer.Start();
    }

    private void UpdateQrCode(string? uri)
    {
        if (qrPictureBox.Image is not null)
        {
            var previous = qrPictureBox.Image;
            qrPictureBox.Image = null;
            previous.Dispose();
        }

        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var code = new QRCode(data);
        qrPictureBox.Image = code.GetGraphic(20);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _qrRefreshTimer.Stop();
        _qrRefreshTimer.Tick -= QrRefreshTimer_Tick;
        _qrRefreshTimer.Dispose();
        base.OnFormClosed(e);
    }
}
