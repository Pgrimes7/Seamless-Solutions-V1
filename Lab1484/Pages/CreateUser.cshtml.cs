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

        //Method with sample data for the populate button
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            NewUser.UserType = 3;
            NewUser.firstName = "Rob";
            NewUser.lastName = "Johnson";
            NewUser.email = "robjohnson@example.com";
            NewUser.phone = "123-246-2864";
            NewUser.username = "johnsron";
            NewUser.password = "67478";

            return Page();
        }
    }
}
