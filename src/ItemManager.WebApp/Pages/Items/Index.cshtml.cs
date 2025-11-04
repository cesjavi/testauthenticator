using System.ComponentModel.DataAnnotations;
using ItemManager.ApiClient;
using ItemManager.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItemManager.WebApp.Pages.Items;

public class IndexModel : PageModel
{
    private readonly ItemManagerApiClient _apiClient;

    public IndexModel(ItemManagerApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public IList<Item> Items { get; private set; } = new List<Item>();

    [BindProperty]
    public ItemForm NewItem { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public string? StatusMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        return await LoadItemsAsync();
    }

    public async Task<IActionResult> OnPostRefreshAsync()
    {
        return await LoadItemsAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            return await LoadItemsAsync();
        }

        var token = HttpContext.Session.GetString(SessionKeys.Token);
        if (string.IsNullOrEmpty(token))
        {
            TempData[nameof(IndexModel.StatusMessage)] = "La sesión expiró. Inicia sesión nuevamente.";
            return RedirectToPage("/Index");
        }

        var input = new ItemInput(NewItem.Name, string.IsNullOrWhiteSpace(NewItem.Description) ? null : NewItem.Description, NewItem.Quantity);
        var result = await _apiClient.CreateItemAsync(token, input);
        if (!result.Success)
        {
            return await HandleApiError(result.ErrorMessage, result.RequiresReauthentication);
        }

        TempData[nameof(StatusMessage)] = $"Item '{result.Payload!.Name}' creado correctamente.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var token = HttpContext.Session.GetString(SessionKeys.Token);
        if (string.IsNullOrEmpty(token))
        {
            TempData[nameof(IndexModel.StatusMessage)] = "La sesión expiró. Inicia sesión nuevamente.";
            return RedirectToPage("/Index");
        }

        var result = await _apiClient.DeleteItemAsync(token, id);
        if (!result.Success)
        {
            return await HandleApiError(result.ErrorMessage, result.RequiresReauthentication);
        }

        TempData[nameof(StatusMessage)] = "Item eliminado correctamente.";
        return RedirectToPage();
    }

    private async Task<IActionResult> LoadItemsAsync(bool tokenOnly = false)
    {
        var token = HttpContext.Session.GetString(SessionKeys.Token);
        if (string.IsNullOrEmpty(token))
        {
            TempData[nameof(IndexModel.StatusMessage)] = "La sesión expiró. Inicia sesión nuevamente.";
            return RedirectToPage("/Index");
        }

        if (tokenOnly)
        {
            StatusMessage = TempData[nameof(StatusMessage)] as string;
            return Page();
        }

        var result = await _apiClient.GetItemsAsync(token);
        if (!result.Success)
        {
            return await HandleApiError(result.ErrorMessage, result.RequiresReauthentication);
        }

        Items = result.Payload?.ToList() ?? new List<Item>();
        StatusMessage = TempData[nameof(StatusMessage)] as string;
        return Page();
    }

    private async Task<IActionResult> HandleApiError(string? errorMessage, bool requiresReauthentication)
    {
        if (requiresReauthentication)
        {
            HttpContext.Session.Clear();
            TempData[nameof(IndexModel.StatusMessage)] = "La sesión expiró o es inválida. Inicia sesión nuevamente.";
            return RedirectToPage("/Index");
        }

        await LoadItemsAsync(tokenOnly: true);
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Ocurrió un error al contactar al servidor." : errorMessage;
        return Page();
    }

    public sealed class ItemForm
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; } = 0;
    }
}
