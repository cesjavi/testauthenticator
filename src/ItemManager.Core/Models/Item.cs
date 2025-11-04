namespace ItemManager.Core.Models;

public record Item(int Id, string Name, string? Description, int Quantity);

public record ItemInput(string Name, string? Description, int Quantity);
