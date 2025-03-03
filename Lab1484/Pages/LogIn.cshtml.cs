using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class LogInModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (DBClass.SecureLogin(Username, Password) > 0)
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login Successful!";

                // Redirect user to dashboard
                //string redirectTo = HttpContext.Session.GetString("RedirectTo");

                // If the user directly visited the login page (no URL was stored), redirect to the homepage
                //if (string.IsNullOrEmpty(redirectTo))
                //{
                    return RedirectToPage("/Dashboard");
                //}


                // Otherwise, redirect to the originally intended page
                //return RedirectToPage(redirectTo);
            }
            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";

            }

            DBClass.Lab2DBConnection.Close();

            return Page();

        }
    }
}
