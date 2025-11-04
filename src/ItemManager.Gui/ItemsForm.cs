using ItemManager.Core.Models;
using ItemManager.Core.Services;

namespace ItemManager.Gui;

public partial class ItemsForm : Form
{
    private readonly ItemRepository _itemRepository;
    private readonly User _user;

    public ItemsForm(User user, ItemRepository itemRepository)
    {
        _user = user;
        _itemRepository = itemRepository;
        InitializeComponent();
        itemsGrid.AutoGenerateColumns = false;
        itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Item.Id),
            HeaderText = "ID",
            Width = 60,
            ReadOnly = true
        });
        itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Item.Name),
            HeaderText = "Nombre",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true
        });
        itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Item.Description),
            HeaderText = "Descripción",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true
        });
        itemsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Item.Quantity),
            HeaderText = "Cantidad",
            Width = 80,
            ReadOnly = true
        });
        UpdateButtonsState();
    }

    public bool LoggedOut { get; private set; }

    private void ItemsForm_Load(object? sender, EventArgs e)
    {
        welcomeLabel.Text = $"Bienvenido, {_user.DisplayName}";
        RefreshItems();
    }

    private void RefreshItems()
    {
        var items = _itemRepository.GetAll().ToList();
        itemsGrid.DataSource = items;
        UpdateButtonsState();
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var editor = new ItemEditorForm();
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            var created = _itemRepository.Create(editor.ToInput());
            RefreshItems();
            SelectItem(created.Id);
        }
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        using var editor = new ItemEditorForm(selected);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            var updated = _itemRepository.Update(selected.Id, editor.ToInput());
            if (updated is null)
            {
                MessageBox.Show("No se pudo actualizar el item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RefreshItems();
            SelectItem(updated.Id);
        }
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        var confirm = MessageBox.Show($"¿Eliminar '{selected.Name}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
        {
            return;
        }

        var deleted = _itemRepository.Delete(selected.Id);
        if (!deleted)
        {
            MessageBox.Show("No se pudo eliminar el item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        RefreshItems();
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        RefreshItems();
    }

    private void LogoutButton_Click(object? sender, EventArgs e)
    {
        LoggedOut = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ItemsForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!LoggedOut && DialogResult == DialogResult.None)
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    private void ItemsGrid_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateButtonsState();
    }

    private void ItemsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            EditButton_Click(sender, EventArgs.Empty);
        }
    }

    private void SelectItem(int id)
    {
        foreach (DataGridViewRow row in itemsGrid.Rows)
        {
            if (row.DataBoundItem is Item item && item.Id == id)
            {
                row.Selected = true;
                itemsGrid.CurrentCell = row.Cells[0];
                break;
            }
        }
    }

    private Item? GetSelectedItem()
    {
        return itemsGrid.CurrentRow?.DataBoundItem as Item;
    }

    private void UpdateButtonsState()
    {
        var hasSelection = GetSelectedItem() is not null;
        editButton.Enabled = hasSelection;
        deleteButton.Enabled = hasSelection;
    }
}
