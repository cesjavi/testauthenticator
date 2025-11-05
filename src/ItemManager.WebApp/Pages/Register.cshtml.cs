using System.ComponentModel.DataAnnotations;
using ItemManager.ApiClient;
using ItemManager.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItemManager.WebApp.Pages;

public class RegisterModel : PageModel
{
    private readonly ItemManagerApiClient _apiClient;
    private readonly QrCodeService _qrCodeService;

    public RegisterModel(ItemManagerApiClient apiClient, QrCodeService qrCodeService)
    {
        _apiClient = apiClient;
        _qrCodeService = qrCodeService;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public RegistrationResult? Registration { get; private set; }

    public string? QrCodeDataUri { get; private set; }

    public string? SuccessMessage { get; private set; }

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Registration = null;
        QrCodeDataUri = null;
        SuccessMessage = null;
        ErrorMessage = null;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var username = Input.Username.Trim();
        var displayName = Input.DisplayName.Trim();
        var password = Input.Password.Trim();

        var result = await _apiClient.RegisterUserAsync(username, displayName, password);
        if (!result.Success)
        {
            var message = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "No fue posible registrar el usuario."
                : result.ErrorMessage;

            ModelState.AddModelError(string.Empty, message);
            return Page();
        }

        Registration = result.Payload!;

        try
        {
            QrCodeDataUri = _qrCodeService.GeneratePngDataUri(Registration.OtpAuthUri);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"El usuario se creó pero no se pudo generar el código QR: {ex.Message}";
        }

        SuccessMessage = $"Usuario '{Registration.Username}' creado correctamente.";

        ModelState.Clear();
        Input = new RegisterInput();

        return Page();
    }

    public sealed class RegisterInput
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Display(Name = "Usuario")]
        [RegularExpression(@"^[a-zA-Z0-9._-]{3,32}$", ErrorMessage = "Usá entre 3 y 32 caracteres alfanuméricos, puntos, guiones o guiones bajos.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre a mostrar es obligatorio.")]
        [Display(Name = "Nombre a mostrar")]
        [StringLength(64, ErrorMessage = "El nombre a mostrar debe tener hasta 64 caracteres.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 64 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
