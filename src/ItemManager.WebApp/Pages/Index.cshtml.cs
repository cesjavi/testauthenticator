using System.ComponentModel.DataAnnotations;
using ItemManager.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItemManager.WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ItemManagerApiClient _apiClient;

    public IndexModel(ItemManagerApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? StatusMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString(SessionKeys.Token) is not null)
        {
            return RedirectToPage("/Items/Index");
        }

        StatusMessage = TempData[nameof(StatusMessage)] as string;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var loginResult = await _apiClient.LoginAsync(Input.Username, Input.Password, Input.OtpCode);
        if (!loginResult.Success)
        {
            ModelState.AddModelError(string.Empty, loginResult.ErrorMessage ?? "No fue posible iniciar sesión.");
            return Page();
        }

        var session = loginResult.Payload!;
        HttpContext.Session.SetString(SessionKeys.Token, session.Token);
        HttpContext.Session.SetString(SessionKeys.Username, session.Username);
        HttpContext.Session.SetString(SessionKeys.DisplayName, session.DisplayName);

        return RedirectToPage("/Items/Index");
    }

    public sealed class LoginInput
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Display(Name = "Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código TOTP es obligatorio.")]
        [Display(Name = "Código TOTP (6 dígitos)")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "El código debe tener 6 dígitos.")]
        public string OtpCode { get; set; } = string.Empty;
    }
}
