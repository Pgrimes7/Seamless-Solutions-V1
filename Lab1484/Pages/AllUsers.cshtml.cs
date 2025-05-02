using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;

namespace Lab1484.Pages
{
    public class AllUsersModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; } = new User();

        [BindProperty]
        public UserUpdate UpdateUser { get; set; } = new UserUpdate();

        [BindProperty(SupportsGet = true)]
        public int? UserType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public List<UserDisplay> Users { get; set; } = new();

        [BindProperty]
        public string? ProfileImagePath { get; set; } = "/images/default.png";

        [TempData]
        public string? CreateOrEditUserSuccess { get; set; }


        [TempData]
        public string? CreateOrEditUserFailure { get; set; }


        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            int userType = DBClass.checkUserType(HttpContext);
            if (userType != 0) 
            {
                return RedirectToPage("/Dashboard");
            }

            string? currentUserIdStr = HttpContext.Session.GetString("userID");
            if (int.TryParse(currentUserIdStr, out int currentUserId))
            {
                NewUser = DBClass.GetUserInfoById(currentUserId);

                if (!string.IsNullOrEmpty(NewUser?.ProfileImageFileName))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", NewUser.ProfileImageFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        ProfileImagePath = $"/images/{NewUser.ProfileImageFileName}";
                    }
                }
            }

            SqlCommand cmd = new SqlCommand();
            if (DBClass.Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                DBClass.Lab3DBConnection.Close();
            }

            cmd.Connection = DBClass.Lab3DBConnection;
            //Uncomment if working locally
            cmd.Connection.ConnectionString = "Server=LocalHost;Database=Lab3;Trusted_Connection=True";
            /*cmd.Connection.ConnectionString = "Server=seamless-solutions-server.database.windows.net,1433;" +
            "Database=Lab3;" +
            "User Id=capstoneadmin;" +
            "Password=Seamless123!@#;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";*/
            cmd.Connection.Open();

            if (UserType.HasValue)
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber, Users.ProfileImageFileName FROM Lab3.dbo.Users WHERE Users.UserType = @UserType ORDER BY Users.UserID;" ;
                cmd.Parameters.AddWithValue("@UserType", UserType.Value);
            }
            

            else if (!string.IsNullOrEmpty(SearchQuery))
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber, Users.ProfileImageFileName FROM Lab3.dbo.Users WHERE Users.firstName LIKE @SearchQuery OR Users.lastName LIKE @SearchQuery OR Concat(Users.firstName, ' ', Users.lastName) LIKE @SearchQuery OR email LIKE @SearchQuery OR Users.phoneNumber LIKE @SearchQuery ORDER BY Users.UserID;";
                cmd.Parameters.AddWithValue("@SearchQuery", "%" + SearchQuery + "%");
            }
            else
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber, Users.ProfileImageFileName FROM Lab3.dbo.Users ORDER BY Users.UserID;";
            }

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int type = reader.GetInt32(1);
                string role = "";

                if (type == 0) role = "Executive Director";
                else if (type == 1) role = "Associate Director";
                else if (type == 2) role = "Faculty Affiliate";
                else if (type == 3) role = "Student";
                else if (type == 4) role = "Administrative Assistant";
                else if (type == 5) role = "External Partner";
                else if (type == 6) role = "Principal Investigator";
                else if (type == 7) role = "Co-Principal Investigator";
                else role = "Unknown";

                Users.Add(new UserDisplay
                {
                    UserID = reader.GetInt32(0),
                    UserTypeName = role,
                    UsersName = reader.GetString(2),

                    /*LastName = reader.GetString(2),*/

                    Email = reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ProfileImageFileName = reader.IsDBNull(5) ? "" : reader.GetString(5)

                });
            }

            reader.Close();
            DBClass.Lab3DBConnection.Close();

            return Page();
        }




        public class UserDisplay
        {
            [BindProperty]
            public int UserID { get; set; }
            [BindProperty]
            public string UserTypeName { get; set; }
            [BindProperty]
            public string UsersName { get; set; }

            /*public string LastName { get; set; }*/
            [BindProperty]
            public string Email { get; set; }
            [BindProperty]
            public string Phone { get; set; }
            [BindProperty]
            public string Username { get; set;}
            [BindProperty]
            public string Password { get; set; }
            [BindProperty]
            public string ProfileImageFileName { get; set; }
        }


        public IActionResult OnPostCreateUser()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/AllUsers");
            }

            bool success = DBClass.CreateHashedUser(NewUser);
            DBClass.Lab3DBConnection.Close();

            if (success)
            {
                CreateOrEditUserSuccess = "User was successfully created.";
            }
            else
            {
                CreateOrEditUserFailure = "Error: User could not be created.";
            }

            return RedirectToPage("/AllUsers");
        }

        public IActionResult OnPostUpdateUser()
        {


            bool success = DBClass.UpdateHashedUser(UpdateUser);
            DBClass.Lab3DBConnection.Close();

            if (success)
            {
                CreateOrEditUserSuccess = "User was successfully updated.";
            }
            else
            {
                CreateOrEditUserFailure = "Error: User could not be updated.";
            }

            return RedirectToPage("/AllUsers");
        }
    }
}