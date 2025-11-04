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
        brandPanel = new Panel();
        brandFooterLabel = new Label();
        brandSubtitleLabel = new Label();
        brandTitleLabel = new Label();
        contentPanel = new Panel();
        registerButton = new Button();
        messageLabel = new Label();
        loginButton = new Button();
        otpInfoLink = new LinkLabel();
        otpTextBox = new TextBox();
        otpLabel = new Label();
        passwordTextBox = new TextBox();
        passwordLabel = new Label();
        usernameTextBox = new TextBox();
        usernameLabel = new Label();
        welcomeSubtitleLabel = new Label();
        welcomeLabel = new Label();
        brandPanel.SuspendLayout();
        contentPanel.SuspendLayout();
        SuspendLayout();
        //
        // brandPanel
        //
        brandPanel.Controls.Add(brandFooterLabel);
        brandPanel.Controls.Add(brandSubtitleLabel);
        brandPanel.Controls.Add(brandTitleLabel);
        brandPanel.Dock = DockStyle.Left;
        brandPanel.Location = new Point(0, 0);
        brandPanel.Name = "brandPanel";
        brandPanel.Padding = new Padding(32, 40, 32, 40);
        brandPanel.Size = new Size(260, 441);
        brandPanel.TabIndex = 0;
        brandPanel.Paint += BrandPanel_Paint;
        //
        // brandFooterLabel
        //
        brandFooterLabel.AutoSize = false;
        brandFooterLabel.Dock = DockStyle.Bottom;
        brandFooterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        brandFooterLabel.ForeColor = Color.White;
        brandFooterLabel.Location = new Point(32, 329);
        brandFooterLabel.Name = "brandFooterLabel";
        brandFooterLabel.Size = new Size(196, 72);
        brandFooterLabel.TabIndex = 2;
        brandFooterLabel.Text = "Protegido con doble factor de autenticación para que solo vos accedas a tus datos.";
        brandFooterLabel.TextAlign = ContentAlignment.BottomLeft;
        //
        // brandSubtitleLabel
        //
        brandSubtitleLabel.AutoSize = true;
        brandSubtitleLabel.Dock = DockStyle.Top;
        brandSubtitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        brandSubtitleLabel.ForeColor = Color.White;
        brandSubtitleLabel.Location = new Point(32, 90);
        brandSubtitleLabel.MaximumSize = new Size(196, 0);
        brandSubtitleLabel.Name = "brandSubtitleLabel";
        brandSubtitleLabel.Padding = new Padding(0, 12, 0, 0);
        brandSubtitleLabel.Size = new Size(192, 50);
        brandSubtitleLabel.TabIndex = 1;
        brandSubtitleLabel.Text = "Gestioná tus items como si fuera tu billetera digital, con seguridad Mercado Pago.";
        //
        // brandTitleLabel
        //
        brandTitleLabel.AutoSize = true;
        brandTitleLabel.Dock = DockStyle.Top;
        brandTitleLabel.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
        brandTitleLabel.ForeColor = Color.White;
        brandTitleLabel.Location = new Point(32, 40);
        brandTitleLabel.Name = "brandTitleLabel";
        brandTitleLabel.Size = new Size(177, 37);
        brandTitleLabel.TabIndex = 0;
        brandTitleLabel.Text = "ItemManager";
        //
        // contentPanel
        //
        contentPanel.BackColor = Color.White;
        contentPanel.Controls.Add(registerButton);
        contentPanel.Controls.Add(messageLabel);
        contentPanel.Controls.Add(loginButton);
        contentPanel.Controls.Add(otpInfoLink);
        contentPanel.Controls.Add(otpTextBox);
        contentPanel.Controls.Add(otpLabel);
        contentPanel.Controls.Add(passwordTextBox);
        contentPanel.Controls.Add(passwordLabel);
        contentPanel.Controls.Add(usernameTextBox);
        contentPanel.Controls.Add(usernameLabel);
        contentPanel.Controls.Add(welcomeSubtitleLabel);
        contentPanel.Controls.Add(welcomeLabel);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(260, 0);
        contentPanel.Name = "contentPanel";
        contentPanel.Padding = new Padding(0);
        contentPanel.Size = new Size(472, 441);
        contentPanel.TabIndex = 1;
        //
        // registerButton
        //
        registerButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        registerButton.BackColor = Color.White;
        registerButton.FlatAppearance.BorderSize = 0;
        registerButton.FlatStyle = FlatStyle.Flat;
        registerButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        registerButton.ForeColor = Color.FromArgb(0, 102, 204);
        registerButton.Location = new Point(48, 328);
        registerButton.Name = "registerButton";
        registerButton.Size = new Size(376, 34);
        registerButton.TabIndex = 9;
        registerButton.Text = "Agregar cuenta en Authenticator";
        registerButton.UseVisualStyleBackColor = false;
        registerButton.Click += RegisterButton_Click;
        //
        // messageLabel
        //
        messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        messageLabel.ForeColor = Color.Firebrick;
        messageLabel.Location = new Point(48, 264);
        messageLabel.Name = "messageLabel";
        messageLabel.Size = new Size(376, 24);
        messageLabel.TabIndex = 8;
        messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // loginButton
        //
        loginButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        loginButton.BackColor = Color.FromArgb(0, 132, 255);
        loginButton.FlatAppearance.BorderSize = 0;
        loginButton.FlatStyle = FlatStyle.Flat;
        loginButton.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
        loginButton.ForeColor = Color.White;
        loginButton.Location = new Point(48, 296);
        loginButton.Name = "loginButton";
        loginButton.Size = new Size(376, 44);
        loginButton.TabIndex = 7;
        loginButton.Text = "Ingresar";
        loginButton.UseVisualStyleBackColor = false;
        loginButton.Click += LoginButton_Click;
        //
        // otpInfoLink
        //
        otpInfoLink.ActiveLinkColor = Color.FromArgb(0, 102, 204);
        otpInfoLink.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        otpInfoLink.AutoSize = true;
        otpInfoLink.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        otpInfoLink.LinkColor = Color.FromArgb(0, 132, 255);
        otpInfoLink.Location = new Point(271, 224);
        otpInfoLink.Name = "otpInfoLink";
        otpInfoLink.Size = new Size(153, 15);
        otpInfoLink.TabIndex = 6;
        otpInfoLink.TabStop = true;
        otpInfoLink.Text = "Ver códigos y secretos TOTP";
        otpInfoLink.VisitedLinkColor = Color.FromArgb(0, 72, 204);
        otpInfoLink.LinkClicked += OtpInfoLink_LinkClicked;
        //
        // otpTextBox
        //
        otpTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        otpTextBox.BorderStyle = BorderStyle.FixedSingle;
        otpTextBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        otpTextBox.Location = new Point(48, 216);
        otpTextBox.MaxLength = 6;
        otpTextBox.Name = "otpTextBox";
        otpTextBox.Size = new Size(176, 27);
        otpTextBox.TabIndex = 5;
        otpTextBox.TextAlign = HorizontalAlignment.Center;
        //
        // otpLabel
        //
        otpLabel.AutoSize = true;
        otpLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        otpLabel.ForeColor = Color.FromArgb(64, 64, 64);
        otpLabel.Location = new Point(48, 196);
        otpLabel.Name = "otpLabel";
        otpLabel.Size = new Size(166, 15);
        otpLabel.TabIndex = 4;
        otpLabel.Text = "Código de seguridad (TOTP)";
        //
        // passwordTextBox
        //
        passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
        passwordTextBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        passwordTextBox.Location = new Point(48, 160);
        passwordTextBox.Name = "passwordTextBox";
        passwordTextBox.PasswordChar = '\u25cf';
        passwordTextBox.Size = new Size(376, 27);
        passwordTextBox.TabIndex = 3;
        //
        // passwordLabel
        //
        passwordLabel.AutoSize = true;
        passwordLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        passwordLabel.ForeColor = Color.FromArgb(64, 64, 64);
        passwordLabel.Location = new Point(48, 140);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(69, 15);
        passwordLabel.TabIndex = 2;
        passwordLabel.Text = "Contraseña";
        //
        // usernameTextBox
        //
        usernameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        usernameTextBox.BorderStyle = BorderStyle.FixedSingle;
        usernameTextBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        usernameTextBox.Location = new Point(48, 104);
        usernameTextBox.Name = "usernameTextBox";
        usernameTextBox.Size = new Size(376, 27);
        usernameTextBox.TabIndex = 1;
        //
        // usernameLabel
        //
        usernameLabel.AutoSize = true;
        usernameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        usernameLabel.ForeColor = Color.FromArgb(64, 64, 64);
        usernameLabel.Location = new Point(48, 84);
        usernameLabel.Name = "usernameLabel";
        usernameLabel.Size = new Size(54, 15);
        usernameLabel.TabIndex = 0;
        usernameLabel.Text = "Usuario";
        //
        // welcomeSubtitleLabel
        //
        welcomeSubtitleLabel.AutoSize = true;
        welcomeSubtitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        welcomeSubtitleLabel.ForeColor = Color.FromArgb(96, 96, 96);
        welcomeSubtitleLabel.Location = new Point(48, 52);
        welcomeSubtitleLabel.Name = "welcomeSubtitleLabel";
        welcomeSubtitleLabel.Size = new Size(244, 19);
        welcomeSubtitleLabel.TabIndex = 11;
        welcomeSubtitleLabel.Text = "Ingresá tus datos para continuar";
        //
        // welcomeLabel
        //
        welcomeLabel.AutoSize = true;
        welcomeLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
        welcomeLabel.ForeColor = Color.FromArgb(33, 37, 41);
        welcomeLabel.Location = new Point(48, 16);
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Size = new Size(166, 32);
        welcomeLabel.TabIndex = 10;
        welcomeLabel.Text = "¡Hola de nuevo!";
        //
        // LoginForm
        //
        AcceptButton = loginButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(732, 441);
        Controls.Add(contentPanel);
        Controls.Add(brandPanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Item Manager - Inicio de sesión";
        brandPanel.ResumeLayout(false);
        brandPanel.PerformLayout();
        contentPanel.ResumeLayout(false);
        contentPanel.PerformLayout();
        ResumeLayout(false);
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
    private Button registerButton;
    private Panel brandPanel;
    private Label brandTitleLabel;
    private Label brandSubtitleLabel;
    private Label brandFooterLabel;
    private Panel contentPanel;
    private Label welcomeLabel;
    private Label welcomeSubtitleLabel;
    private System.ComponentModel.IContainer? components;
}
