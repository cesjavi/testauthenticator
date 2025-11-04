namespace ItemManager.Gui;

partial class LoginForm
{
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        usernameLabel = new Label();
        usernameTextBox = new TextBox();
        passwordLabel = new Label();
        passwordTextBox = new TextBox();
        otpLabel = new Label();
        otpTextBox = new TextBox();
        loginButton = new Button();
        messageLabel = new Label();
        otpInfoLink = new LinkLabel();
        SuspendLayout();
        // 
        // usernameLabel
        // 
        usernameLabel.AutoSize = true;
        usernameLabel.Location = new Point(24, 24);
        usernameLabel.Name = "usernameLabel";
        usernameLabel.Size = new Size(60, 15);
        usernameLabel.TabIndex = 0;
        usernameLabel.Text = "Usuario";
        // 
        // usernameTextBox
        // 
        usernameTextBox.Location = new Point(24, 42);
        usernameTextBox.Name = "usernameTextBox";
        usernameTextBox.Size = new Size(312, 23);
        usernameTextBox.TabIndex = 1;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(24, 78);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(67, 15);
        passwordLabel.TabIndex = 2;
        passwordLabel.Text = "Contraseña";
        // 
        // passwordTextBox
        // 
        passwordTextBox.Location = new Point(24, 96);
        passwordTextBox.Name = "passwordTextBox";
        passwordTextBox.PasswordChar = '\u25cf';
        passwordTextBox.Size = new Size(312, 23);
        passwordTextBox.TabIndex = 3;
        // 
        // otpLabel
        // 
        otpLabel.AutoSize = true;
        otpLabel.Location = new Point(24, 132);
        otpLabel.Name = "otpLabel";
        otpLabel.Size = new Size(168, 15);
        otpLabel.TabIndex = 4;
        otpLabel.Text = "Código Google Authenticator";
        // 
        // otpTextBox
        // 
        otpTextBox.Location = new Point(24, 150);
        otpTextBox.MaxLength = 6;
        otpTextBox.Name = "otpTextBox";
        otpTextBox.Size = new Size(184, 23);
        otpTextBox.TabIndex = 5;
        // 
        // loginButton
        // 
        loginButton.Location = new Point(241, 188);
        loginButton.Name = "loginButton";
        loginButton.Size = new Size(95, 27);
        loginButton.TabIndex = 7;
        loginButton.Text = "Ingresar";
        loginButton.UseVisualStyleBackColor = true;
        loginButton.Click += LoginButton_Click;
        // 
        // messageLabel
        // 
        messageLabel.AutoSize = true;
        messageLabel.ForeColor = Color.Firebrick;
        messageLabel.Location = new Point(24, 188);
        messageLabel.Name = "messageLabel";
        messageLabel.Size = new Size(0, 15);
        messageLabel.TabIndex = 8;
        // 
        // otpInfoLink
        // 
        otpInfoLink.AutoSize = true;
        otpInfoLink.Location = new Point(216, 153);
        otpInfoLink.Name = "otpInfoLink";
        otpInfoLink.Size = new Size(120, 15);
        otpInfoLink.TabIndex = 6;
        otpInfoLink.TabStop = true;
        otpInfoLink.Text = "Ver secretos TOTP";
        otpInfoLink.LinkClicked += OtpInfoLink_LinkClicked;
        // 
        // LoginForm
        // 
        AcceptButton = loginButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(360, 235);
        Controls.Add(otpInfoLink);
        Controls.Add(messageLabel);
        Controls.Add(loginButton);
        Controls.Add(otpTextBox);
        Controls.Add(otpLabel);
        Controls.Add(passwordTextBox);
        Controls.Add(passwordLabel);
        Controls.Add(usernameTextBox);
        Controls.Add(usernameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Item Manager - Inicio de sesión";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label usernameLabel;
    private TextBox usernameTextBox;
    private Label passwordLabel;
    private TextBox passwordTextBox;
    private Label otpLabel;
    private TextBox otpTextBox;
    private Button loginButton;
    private Label messageLabel;
    private LinkLabel otpInfoLink;
    private System.ComponentModel.IContainer? components;
}
