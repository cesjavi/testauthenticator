using System.Drawing;
using System.Windows.Forms;

namespace ItemManager.Gui;

partial class RegisterUserForm
{
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            qrPictureBox.Image?.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        existingUsersGroupBox = new GroupBox();
        copyUriButton = new Button();
        copySecretButton = new Button();
        uriTextBox = new TextBox();
        uriLabel = new Label();
        secretTextBox = new TextBox();
        secretLabel = new Label();
        userComboBox = new ComboBox();
        userLabel = new Label();
        qrLabel = new Label();
        qrPictureBox = new PictureBox();
        instructionsLabel = new Label();
        newUserGroupBox = new GroupBox();
        messageLabel = new Label();
        generateButton = new Button();
        passwordTextBox = new TextBox();
        passwordLabel = new Label();
        displayNameTextBox = new TextBox();
        displayNameLabel = new Label();
        newUsernameTextBox = new TextBox();
        newUsernameLabel = new Label();
        toolTip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)qrPictureBox).BeginInit();
        existingUsersGroupBox.SuspendLayout();
        newUserGroupBox.SuspendLayout();
        SuspendLayout();
        //
        // existingUsersGroupBox
        //
        existingUsersGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        existingUsersGroupBox.Controls.Add(qrPictureBox);
        existingUsersGroupBox.Controls.Add(qrLabel);
        existingUsersGroupBox.Controls.Add(copyUriButton);
        existingUsersGroupBox.Controls.Add(copySecretButton);
        existingUsersGroupBox.Controls.Add(uriTextBox);
        existingUsersGroupBox.Controls.Add(uriLabel);
        existingUsersGroupBox.Controls.Add(secretTextBox);
        existingUsersGroupBox.Controls.Add(secretLabel);
        existingUsersGroupBox.Controls.Add(userComboBox);
        existingUsersGroupBox.Controls.Add(userLabel);
        existingUsersGroupBox.Location = new Point(12, 12);
        existingUsersGroupBox.Name = "existingUsersGroupBox";
        existingUsersGroupBox.Size = new Size(720, 230);
        existingUsersGroupBox.TabIndex = 0;
        existingUsersGroupBox.TabStop = false;
        existingUsersGroupBox.Text = "Usuarios existentes";
        //
        // copyUriButton
        //
        copyUriButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        copyUriButton.Location = new Point(420, 182);
        copyUriButton.Name = "copyUriButton";
        copyUriButton.Size = new Size(75, 23);
        copyUriButton.TabIndex = 7;
        copyUriButton.Text = "Copiar";
        copyUriButton.UseVisualStyleBackColor = true;
        copyUriButton.Click += CopyUriButton_Click;
        toolTip.SetToolTip(copyUriButton, "Copia el enlace otpauth://");
        //
        // copySecretButton
        //
        copySecretButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        copySecretButton.Location = new Point(420, 118);
        copySecretButton.Name = "copySecretButton";
        copySecretButton.Size = new Size(75, 23);
        copySecretButton.TabIndex = 5;
        copySecretButton.Text = "Copiar";
        copySecretButton.UseVisualStyleBackColor = true;
        copySecretButton.Click += CopySecretButton_Click;
        toolTip.SetToolTip(copySecretButton, "Copia el secreto para registrarlo en Google Authenticator");
        //
        // uriTextBox
        //
        uriTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        uriTextBox.Location = new Point(18, 183);
        uriTextBox.Name = "uriTextBox";
        uriTextBox.ReadOnly = true;
        uriTextBox.Size = new Size(396, 23);
        uriTextBox.TabIndex = 6;
        //
        // uriLabel
        //
        uriLabel.AutoSize = true;
        uriLabel.Location = new Point(18, 165);
        uriLabel.Name = "uriLabel";
        uriLabel.Size = new Size(104, 15);
        uriLabel.TabIndex = 4;
        uriLabel.Text = "Enlace otpauth://";
        //
        // secretTextBox
        //
        secretTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        secretTextBox.Location = new Point(18, 119);
        secretTextBox.Name = "secretTextBox";
        secretTextBox.ReadOnly = true;
        secretTextBox.Size = new Size(396, 23);
        secretTextBox.TabIndex = 4;
        //
        // secretLabel
        //
        secretLabel.AutoSize = true;
        secretLabel.Location = new Point(18, 101);
        secretLabel.Name = "secretLabel";
        secretLabel.Size = new Size(48, 15);
        secretLabel.TabIndex = 2;
        secretLabel.Text = "Secreto";
        //
        // userComboBox
        //
        userComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        userComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        userComboBox.FormattingEnabled = true;
        userComboBox.Location = new Point(18, 40);
        userComboBox.Name = "userComboBox";
        userComboBox.Size = new Size(477, 23);
        userComboBox.TabIndex = 1;
        userComboBox.SelectedIndexChanged += UserComboBox_SelectedIndexChanged;
        //
        // userLabel
        //
        userLabel.AutoSize = true;
        userLabel.Location = new Point(18, 22);
        userLabel.Name = "userLabel";
        userLabel.Size = new Size(111, 15);
        userLabel.TabIndex = 0;
        userLabel.Text = "Seleccionar usuario";
        //
        // qrLabel
        //
        qrLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        qrLabel.AutoSize = false;
        qrLabel.Location = new Point(534, 22);
        qrLabel.MaximumSize = new Size(160, 0);
        qrLabel.Name = "qrLabel";
        qrLabel.Size = new Size(160, 30);
        qrLabel.TabIndex = 8;
        qrLabel.Text = "Escaneá este QR con Google Authenticator";
        qrLabel.TextAlign = ContentAlignment.MiddleCenter;
        //
        // qrPictureBox
        //
        qrPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        qrPictureBox.BorderStyle = BorderStyle.FixedSingle;
        qrPictureBox.Location = new Point(534, 55);
        qrPictureBox.Name = "qrPictureBox";
        qrPictureBox.Size = new Size(160, 160);
        qrPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        qrPictureBox.TabIndex = 9;
        qrPictureBox.TabStop = false;
        //
        // instructionsLabel
        //
        instructionsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        instructionsLabel.AutoSize = false;
        instructionsLabel.Location = new Point(12, 247);
        instructionsLabel.Name = "instructionsLabel";
        instructionsLabel.Size = new Size(720, 45);
        instructionsLabel.TabIndex = 1;
        instructionsLabel.Text = "Escaneá el código QR en Google Authenticator para registrar la cuenta. Si no podés escanearlo, copiá el enlace otpauth:// o el secreto para agregarlo manualmente.";
        //
        // newUserGroupBox
        //
        newUserGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        newUserGroupBox.Controls.Add(messageLabel);
        newUserGroupBox.Controls.Add(generateButton);
        newUserGroupBox.Controls.Add(passwordTextBox);
        newUserGroupBox.Controls.Add(passwordLabel);
        newUserGroupBox.Controls.Add(displayNameTextBox);
        newUserGroupBox.Controls.Add(displayNameLabel);
        newUserGroupBox.Controls.Add(newUsernameTextBox);
        newUserGroupBox.Controls.Add(newUsernameLabel);
        newUserGroupBox.Location = new Point(12, 302);
        newUserGroupBox.Name = "newUserGroupBox";
        newUserGroupBox.Size = new Size(720, 195);
        newUserGroupBox.TabIndex = 2;
        newUserGroupBox.TabStop = false;
        newUserGroupBox.Text = "Crear nuevo usuario";
        //
        // messageLabel
        //
        messageLabel.AutoSize = true;
        messageLabel.ForeColor = Color.Firebrick;
        messageLabel.Location = new Point(18, 156);
        messageLabel.Name = "messageLabel";
        messageLabel.Size = new Size(0, 15);
        messageLabel.TabIndex = 7;
        //
        // generateButton
        //
        generateButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        generateButton.Location = new Point(618, 148);
        generateButton.Name = "generateButton";
        generateButton.Size = new Size(90, 30);
        generateButton.TabIndex = 6;
        generateButton.Text = "Registrar";
        generateButton.UseVisualStyleBackColor = true;
        generateButton.Click += GenerateButton_Click;
        //
        // passwordTextBox
        //
        passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        passwordTextBox.Location = new Point(18, 110);
        passwordTextBox.Name = "passwordTextBox";
        passwordTextBox.PasswordChar = '\u25cf';
        passwordTextBox.Size = new Size(684, 23);
        passwordTextBox.TabIndex = 5;
        //
        // passwordLabel
        //
        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(18, 92);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(67, 15);
        passwordLabel.TabIndex = 4;
        passwordLabel.Text = "Contraseña";
        //
        // displayNameTextBox
        //
        displayNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        displayNameTextBox.Location = new Point(18, 64);
        displayNameTextBox.Name = "displayNameTextBox";
        displayNameTextBox.Size = new Size(684, 23);
        displayNameTextBox.TabIndex = 3;
        //
        // displayNameLabel
        //
        displayNameLabel.AutoSize = true;
        displayNameLabel.Location = new Point(18, 46);
        displayNameLabel.Name = "displayNameLabel";
        displayNameLabel.Size = new Size(110, 15);
        displayNameLabel.TabIndex = 2;
        displayNameLabel.Text = "Nombre a mostrar";
        //
        // newUsernameTextBox
        //
        newUsernameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        newUsernameTextBox.Location = new Point(18, 26);
        newUsernameTextBox.Name = "newUsernameTextBox";
        newUsernameTextBox.Size = new Size(684, 23);
        newUsernameTextBox.TabIndex = 1;
        //
        // newUsernameLabel
        //
        newUsernameLabel.AutoSize = true;
        newUsernameLabel.Location = new Point(18, 8);
        newUsernameLabel.Name = "newUsernameLabel";
        newUsernameLabel.Size = new Size(52, 15);
        newUsernameLabel.TabIndex = 0;
        newUsernameLabel.Text = "Usuario";
        //
        // RegisterUserForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(744, 525);
        Controls.Add(newUserGroupBox);
        Controls.Add(instructionsLabel);
        Controls.Add(existingUsersGroupBox);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "RegisterUserForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Registrar en Google Authenticator";
        Load += RegisterUserForm_Load;
        ((System.ComponentModel.ISupportInitialize)qrPictureBox).EndInit();
        existingUsersGroupBox.ResumeLayout(false);
        existingUsersGroupBox.PerformLayout();
        newUserGroupBox.ResumeLayout(false);
        newUserGroupBox.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox existingUsersGroupBox;
    private Button copyUriButton;
    private Button copySecretButton;
    private TextBox uriTextBox;
    private Label uriLabel;
    private TextBox secretTextBox;
    private Label secretLabel;
    private ComboBox userComboBox;
    private Label userLabel;
    private Label instructionsLabel;
    private Label qrLabel;
    private PictureBox qrPictureBox;
    private GroupBox newUserGroupBox;
    private Button generateButton;
    private TextBox passwordTextBox;
    private Label passwordLabel;
    private TextBox displayNameTextBox;
    private Label displayNameLabel;
    private TextBox newUsernameTextBox;
    private Label newUsernameLabel;
    private Label messageLabel;
    private System.ComponentModel.IContainer? components;
    private ToolTip toolTip;
}
