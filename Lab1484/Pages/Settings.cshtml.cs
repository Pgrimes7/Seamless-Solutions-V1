using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Lab1484.Pages
{
    public class SettingsModel : PageModel
    {
        public string? ProfileImagePath { get; set; } = "/images/default.png";
        public User? CurrentUser { get; set; }

        public void OnGet()
        {
            string? currentUserIdStr = HttpContext.Session.GetString("userId");
            if (int.TryParse(currentUserIdStr, out int currentUserId))
            {
                var user = DBClass.GetProfilePictureById(currentUserId);
                if (!string.IsNullOrEmpty(CurrentUser?.ProfileImageFileName))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", CurrentUser.ProfileImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        ProfileImagePath = $"/images/{CurrentUser.ProfileImageFileName}";
                    }
                }
            }
        }

        

    }
}
