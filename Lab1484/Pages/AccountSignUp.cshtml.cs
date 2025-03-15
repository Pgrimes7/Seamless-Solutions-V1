using System.Runtime.InteropServices;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class AccountSignUpModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; } = new User();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
           
            //Redirect them if they aren't
     

            return Page();

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Perform Validation First on Form
            // then...

            DBClass.CreateHashedUser(NewUser);
            DBClass.Lab3DBConnection.Close();


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
        public IActionResult OnPostClear()
        {
            // Clears the form 
            ModelState.Clear();
            NewUser = new User();
            return RedirectToPage();
        }
    }
}
