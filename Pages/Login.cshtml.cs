using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace claudeWebsite.Pages;

public class LoginModel : PageModel
{
    private readonly IConfiguration _config;

    public string? ErrorMessage { get; private set; }

    public LoginModel(IConfiguration config)
    {
        _config = config;
    }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Admin/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string password)
    {
        var correctPassword = _config["Gallery:Password"];
        if (password != correctPassword)
        {
            ErrorMessage = "Incorrect password.";
            return Page();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "owner") };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToPage("/Admin/Index");
    }
}
