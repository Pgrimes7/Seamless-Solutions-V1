using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DB;

namespace Lab1484.Pages
{
    public class HashedLoginModel : PageModel
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
            if (DBClass.HashedParameterLogin(Username, Password))
            {
                HttpContext.Session.SetString("username", Username);
                ViewData["LoginMessage"] = "Login Successful!";
                DBClass.Lab3DBConnection.Close();
                return Page();
            }
            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                DBClass.Lab3DBConnection.Close();
                return Page();
            }

        }
    }
}
