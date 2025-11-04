using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItemManager.WebApp.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        HttpContext.Session.Clear();
        TempData[nameof(IndexModel.StatusMessage)] = "Sesión finalizada correctamente.";
        return RedirectToPage("/Index");
    }
}
