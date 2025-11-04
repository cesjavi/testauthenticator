using ItemManager.Core.Models;

namespace ItemManager.Gui;

public partial class ItemEditorForm : Form
{
    public ItemEditorForm()
    {
        InitializeComponent();
    }

    public ItemEditorForm(Item item)
        : this()
    {
        nameTextBox.Text = item.Name;
        descriptionTextBox.Text = item.Description ?? string.Empty;
        quantityNumeric.Value = Math.Max(quantityNumeric.Minimum, Math.Min(quantityNumeric.Maximum, item.Quantity));
    }

    public ItemInput ToInput()
    {
        return new ItemInput(
            nameTextBox.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim(),
            (int)quantityNumeric.Value);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Ingresá un nombre válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nameTextBox.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
