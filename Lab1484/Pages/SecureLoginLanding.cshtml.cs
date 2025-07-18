using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class SecureLoginLandingModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");

                return Page();
            }
        }
    }
}
