using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Lab1484.Pages
{
    public class SettingsModel : PageModel
    {
        [BindProperty]
        public string? ProfileImagePath { get; set; } = "/images/default.png";

        [BindProperty]
        public User? CurrentUser { get; set; } = new User();

        [BindProperty]
        public UserUpdate UpdateUser { get; set; } = new UserUpdate();

        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            string? currentUserIdStr = HttpContext.Session.GetString("userID");
            if (int.TryParse(currentUserIdStr, out int currentUserId))
            {
                CurrentUser = DBClass.GetProfilePictureById(currentUserId); 

                if (!string.IsNullOrEmpty(CurrentUser?.ProfileImageFileName))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", CurrentUser.ProfileImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        ProfileImagePath = $"/images/{CurrentUser.ProfileImageFileName}";
                    }
                }
            }

            return Page();
        }


        public async Task<IActionResult> OnPostUploadProfileImageAsync(IFormFile ProfileImage)
        {
            string? userIdString = HttpContext.Session.GetString("userID");

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                // Generate a unique filename based on the user ID and the original file name
                string fileExtension = Path.GetExtension(ProfileImage.FileName);
                string fileName = $"profile_{HttpContext.Session.GetString("userID")}{fileExtension}";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                // Save the file to wwwroot/images
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                // Update the user's profile image in the database
                var userId = int.Parse(HttpContext.Session.GetString("userID"));
                User user = DBClass.GetProfilePictureById(userId);
                if (user != null)
                {
                    user.ProfileImageFileName = fileName;
                    DBClass.AddProfileImage(user);
                }

                // Redirect to the same page to refresh the image
                return RedirectToPage();
            }

            // If no file is uploaded, just return the same page without changing anything
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteProfileImageAsync()
        {
            // Retrieve the current user ID from session
            string? currentUserIdStr = HttpContext.Session.GetString("userId");
            if (int.TryParse(currentUserIdStr, out int currentUserId))
            {
                // Get the current user
                var user = DBClass.GetProfilePictureById(currentUserId);

                // If the user has a custom profile picture
                if (!string.IsNullOrEmpty(user?.ProfileImageFileName))
                {
                    // Optional: Delete the profile picture file from wwwroot
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", user.ProfileImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);  // Delete the file
                    }
                }

                // Update the user's profile picture in the database to the default picture
                user.ProfileImageFileName = "default.png";  // Set to default image
                DBClass.AddProfileImage(user);  // Update the database

                // Redirect to the Settings page (or where you'd like after deletion)
                return RedirectToPage();
            }

            // Return error if userId is not found or something goes wrong
            return NotFound();
        }

        public IActionResult OnPostUpdateUser()
        {




            DBClass.UpdateHashedUser(UpdateUser);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/Settings");
        }

    }
}
