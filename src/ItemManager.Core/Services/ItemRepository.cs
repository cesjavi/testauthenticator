using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class ItemRepository
{
    private readonly List<Item> _items =
    [
        new Item(1, "Laptop", "Dell XPS 13", 5),
        new Item(2, "Monitor", "Monitor 24\" full HD", 8),
        new Item(3, "Teclado", "Teclado mecánico", 12)
    ];

    private int _nextId = 4;

    public IEnumerable<Item> GetAll() => _items.AsReadOnly();

    public Item? GetById(int id) => _items.FirstOrDefault(item => item.Id == id);

    public Item Create(ItemInput input)
    {
        var item = new Item(_nextId++, input.Name, input.Description, input.Quantity);
        _items.Add(item);
        return item;
    }

    public Item? Update(int id, ItemInput input)
    {
        var index = _items.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return null;
        }

        var updated = new Item(id, input.Name, input.Description, input.Quantity);
        _items[index] = updated;
        return updated;
    }

    public bool Delete(int id)
    {
        var item = GetById(id);
        if (item is null)
        {
            return false;
        }

        _items.Remove(item);
        return true;
    }
}
