using System.Runtime.InteropServices;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; } = new User();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            return Page();
            
        }

        public IActionResult OnPost()
        {
            

            DBClass.InsertUser(NewUser);
            DBClass.Lab2DBConnection.Close();

            return RedirectToPage("/CreateUser");
        }
    }
}
