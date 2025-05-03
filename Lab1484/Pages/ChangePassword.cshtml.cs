using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Lab1484.Pages
{
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public string ConfirmedUsername { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool UsernameConfirmed { get; set; } = false;

        [BindProperty]
        public string UsernameError { get; set; }

        [BindProperty]
        public string PasswordError { get; set; }

        [TempData]
        public string PassChangeSuccess { get; set; }

        [TempData]
        public string PassChangeFailure { get; set; }





        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public IActionResult OnPost(string action)
        {
            var currentUsername = User.Identity?.Name;

            if (action == "confirm")
            {
                if (ConfirmedUsername?.Trim().ToLower() != currentUsername?.ToLower())
                {
                    UsernameError = "Username does not match the logged-in user.";
                    UsernameConfirmed = false;
                }
                else
                {
                    UsernameConfirmed = true;
                }

                return Page(); 
            }

            return Page();
        }


        public IActionResult OnPostConfirmUser()
        {
            string actualUsername = HttpContext.Session.GetString("username");
            if (ConfirmedUsername?.Trim().ToLower() != actualUsername?.ToLower())
            {
                UsernameError = "Username does not match the logged-in user.";
                return Page();
            }

            UsernameConfirmed = true; 
            return Page();
        }

        public IActionResult OnPostUpdatePassword()
        {
            UsernameConfirmed = true;

            ConfirmedUsername = HttpContext.Session.GetString("username");

            if (NewPassword != ConfirmPassword)
            {
                PasswordError = "Passwords do not match.";
                return Page();
            }

            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                PasswordError = "Password fields cannot be empty.";
                return Page();
            }

            string userIdString = HttpContext.Session.GetString("userID");

            bool success = DBClass.UpdateHashedPassword(userIdString, NewPassword);
            DBClass.Lab3DBConnection.Close();

            if (success) 
            {
                PassChangeSuccess = "Password successfully changed.";
                return RedirectToPage("/Settings");
            }

            else
            {
                PassChangeFailure = "Error: Password could not be changed.";
            }

            return Page();
        }
    }
}
