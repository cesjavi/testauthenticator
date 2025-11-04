namespace ItemManager.Gui;

partial class ItemEditorForm
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
        nameLabel = new Label();
        nameTextBox = new TextBox();
        descriptionLabel = new Label();
        descriptionTextBox = new TextBox();
        quantityLabel = new Label();
        quantityNumeric = new NumericUpDown();
        cancelButton = new Button();
        saveButton = new Button();
        ((System.ComponentModel.ISupportInitialize)quantityNumeric).BeginInit();
        SuspendLayout();
        // 
        // nameLabel
        // 
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(24, 20);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(51, 15);
        nameLabel.TabIndex = 0;
        nameLabel.Text = "Nombre";
        // 
        // nameTextBox
        // 
        nameTextBox.Location = new Point(24, 38);
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Size = new Size(304, 23);
        nameTextBox.TabIndex = 1;
        // 
        // descriptionLabel
        // 
        descriptionLabel.AutoSize = true;
        descriptionLabel.Location = new Point(24, 72);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Size = new Size(69, 15);
        descriptionLabel.TabIndex = 2;
        descriptionLabel.Text = "Descripción";
        // 
        // descriptionTextBox
        // 
        descriptionTextBox.Location = new Point(24, 90);
        descriptionTextBox.Multiline = true;
        descriptionTextBox.Name = "descriptionTextBox";
        descriptionTextBox.Size = new Size(304, 82);
        descriptionTextBox.TabIndex = 3;
        // 
        // quantityLabel
        // 
        quantityLabel.AutoSize = true;
        quantityLabel.Location = new Point(24, 183);
        quantityLabel.Name = "quantityLabel";
        quantityLabel.Size = new Size(55, 15);
        quantityLabel.TabIndex = 4;
        quantityLabel.Text = "Cantidad";
        // 
        // quantityNumeric
        // 
        quantityNumeric.Location = new Point(24, 201);
        quantityNumeric.Maximum = new decimal(new int[] {100000, 0, 0, 0});
        quantityNumeric.Name = "quantityNumeric";
        quantityNumeric.Size = new Size(120, 23);
        quantityNumeric.TabIndex = 5;
        // 
        // cancelButton
        // 
        cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(172, 242);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 27);
        cancelButton.TabIndex = 7;
        cancelButton.Text = "Cancelar";
        cancelButton.UseVisualStyleBackColor = true;
        // 
        // saveButton
        // 
        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        saveButton.Location = new Point(253, 242);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 27);
        saveButton.TabIndex = 6;
        saveButton.Text = "Guardar";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += SaveButton_Click;
        // 
        // ItemEditorForm
        // 
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(352, 286);
        Controls.Add(saveButton);
        Controls.Add(cancelButton);
        Controls.Add(quantityNumeric);
        Controls.Add(quantityLabel);
        Controls.Add(descriptionTextBox);
        Controls.Add(descriptionLabel);
        Controls.Add(nameTextBox);
        Controls.Add(nameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Item";
        ((System.ComponentModel.ISupportInitialize)quantityNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label nameLabel;
    private TextBox nameTextBox;
    private Label descriptionLabel;
    private TextBox descriptionTextBox;
    private Label quantityLabel;
    private NumericUpDown quantityNumeric;
    private Button cancelButton;
    private Button saveButton;
    private System.ComponentModel.IContainer? components;
}
