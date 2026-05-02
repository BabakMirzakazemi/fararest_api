using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Auth;

public class LogoutModel(IAdminPanelAuthService authService) : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await authService.SignOutAsync();
        return RedirectToPage("/Auth/Login");
    }
}
