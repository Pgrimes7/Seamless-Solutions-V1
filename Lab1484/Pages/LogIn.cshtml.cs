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

        public IActionResult OnGet()
        {
            

            return Page();
        }

        public IActionResult OnPost()
        {
            if (DBClass.HashedParameterLogin(Username, Password, HttpContext))
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login Successful!";
                
                // Redirect user to dashboard
                return RedirectToPage("/Dashboard");
              
            }
            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";

            }

            DBClass.Lab3DBConnection.Close();

            return Page();

        }
    }
}
