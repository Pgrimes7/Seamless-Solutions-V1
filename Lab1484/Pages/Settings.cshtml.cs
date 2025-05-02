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

        [TempData]
        public string SuccessfulChange { get; set; }

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
                CurrentUser = DBClass.GetUserInfoById(currentUserId); 

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
                // generate unique filename based on the user ID and the original file name
                string fileExtension = Path.GetExtension(ProfileImage.FileName);
                string fileName = $"profile_{HttpContext.Session.GetString("userID")}{fileExtension}";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                // save the file to wwwroot/images
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                //update the user's profile image in the database
                var userId = int.Parse(HttpContext.Session.GetString("userID"));
                User user = DBClass.GetUserInfoById(userId);
                if (user != null)
                {
                    user.ProfileImageFileName = fileName;
                    DBClass.AddProfileImage(user);
                }

                return RedirectToPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteProfileImageAsync()
        {
            
            string? currentUserIdStr = HttpContext.Session.GetString("userID");
            if (int.TryParse(currentUserIdStr, out int currentUserId))
            {
                
                var user = DBClass.GetUserInfoById(currentUserId);

                
                if (!string.IsNullOrEmpty(user?.ProfileImageFileName))
                {
                    
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", user.ProfileImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);  //delete the file
                    }
                }

                //update the user's profile picture in the database to the default picture
                user.ProfileImageFileName = "default.png";  //set to default image
                DBClass.AddProfileImage(user);  //update the database

                
                return RedirectToPage();
            }

            //return error if userID is not found or something goes wrong
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
