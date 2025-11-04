using ItemManager.Core.Models;

namespace ItemManager.Gui;

partial class ItemsForm
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
        welcomeLabel = new Label();
        itemsGrid = new DataGridView();
        addButton = new Button();
        editButton = new Button();
        deleteButton = new Button();
        refreshButton = new Button();
        logoutButton = new Button();
        ((System.ComponentModel.ISupportInitialize)itemsGrid).BeginInit();
        SuspendLayout();
        // 
        // welcomeLabel
        // 
        welcomeLabel.AutoSize = true;
        welcomeLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        welcomeLabel.Location = new Point(12, 9);
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Size = new Size(132, 20);
        welcomeLabel.TabIndex = 0;
        welcomeLabel.Text = "Bienvenido usuario";
        // 
        // itemsGrid
        // 
        itemsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        itemsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        itemsGrid.Location = new Point(12, 41);
        itemsGrid.MultiSelect = false;
        itemsGrid.Name = "itemsGrid";
        itemsGrid.RowTemplate.Height = 25;
        itemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        itemsGrid.Size = new Size(560, 265);
        itemsGrid.TabIndex = 1;
        itemsGrid.SelectionChanged += ItemsGrid_SelectionChanged;
        itemsGrid.CellDoubleClick += ItemsGrid_CellDoubleClick;
        // 
        // addButton
        // 
        addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        addButton.Location = new Point(12, 320);
        addButton.Name = "addButton";
        addButton.Size = new Size(88, 27);
        addButton.TabIndex = 2;
        addButton.Text = "Agregar";
        addButton.UseVisualStyleBackColor = true;
        addButton.Click += AddButton_Click;
        // 
        // editButton
        // 
        editButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        editButton.Location = new Point(106, 320);
        editButton.Name = "editButton";
        editButton.Size = new Size(88, 27);
        editButton.TabIndex = 3;
        editButton.Text = "Editar";
        editButton.UseVisualStyleBackColor = true;
        editButton.Click += EditButton_Click;
        // 
        // deleteButton
        // 
        deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        deleteButton.Location = new Point(200, 320);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(88, 27);
        deleteButton.TabIndex = 4;
        deleteButton.Text = "Eliminar";
        deleteButton.UseVisualStyleBackColor = true;
        deleteButton.Click += DeleteButton_Click;
        // 
        // refreshButton
        // 
        refreshButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        refreshButton.Location = new Point(294, 320);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(88, 27);
        refreshButton.TabIndex = 5;
        refreshButton.Text = "Actualizar";
        refreshButton.UseVisualStyleBackColor = true;
        refreshButton.Click += RefreshButton_Click;
        // 
        // logoutButton
        // 
        logoutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        logoutButton.Location = new Point(484, 320);
        logoutButton.Name = "logoutButton";
        logoutButton.Size = new Size(88, 27);
        logoutButton.TabIndex = 6;
        logoutButton.Text = "Cerrar sesión";
        logoutButton.UseVisualStyleBackColor = true;
        logoutButton.Click += LogoutButton_Click;
        // 
        // ItemsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(584, 361);
        Controls.Add(logoutButton);
        Controls.Add(refreshButton);
        Controls.Add(deleteButton);
        Controls.Add(editButton);
        Controls.Add(addButton);
        Controls.Add(itemsGrid);
        Controls.Add(welcomeLabel);
        MinimumSize = new Size(600, 400);
        Name = "ItemsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de Items";
        FormClosing += ItemsForm_FormClosing;
        Load += ItemsForm_Load;
        ((System.ComponentModel.ISupportInitialize)itemsGrid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label welcomeLabel;
    private DataGridView itemsGrid;
    private Button addButton;
    private Button editButton;
    private Button deleteButton;
    private Button refreshButton;
    private Button logoutButton;
    private System.ComponentModel.IContainer? components;
}
