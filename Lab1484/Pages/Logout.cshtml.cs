using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            //Clear the session data
            HttpContext.Session.Clear();

           
            ViewData["LogoutMessage"] = "Successfully Logged Out!";


            return Page();
        }
    }
}


  