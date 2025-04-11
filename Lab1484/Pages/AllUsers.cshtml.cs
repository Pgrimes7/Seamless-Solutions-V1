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

            SqlCommand cmd = new SqlCommand();
            if (DBClass.Lab3DBConnection.State == System.Data.ConnectionState.Open)
            {
                DBClass.Lab3DBConnection.Close();
            }

            cmd.Connection = DBClass.Lab3DBConnection;
            cmd.Connection.ConnectionString = "Server=LocalHost;Database=Lab3;Trusted_Connection=True";
            cmd.Connection.Open();

            if (UserType.HasValue)
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber FROM Users WHERE Users.UserType = @UserType ORDER BY Users.UserID;" ;
                cmd.Parameters.AddWithValue("@UserType", UserType.Value);
            }
            

            else if (!string.IsNullOrEmpty(SearchQuery))
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber FROM Users WHERE Users.firstName LIKE @SearchQuery OR Users.lastName LIKE @SearchQuery OR Concat(Users.firstName, ' ', Users.lastName) LIKE @SearchQuery OR email LIKE @SearchQuery OR Users.phoneNumber LIKE @SearchQuery ORDER BY Users.UserID;";
                cmd.Parameters.AddWithValue("@SearchQuery", "%" + SearchQuery + "%");
            }
            else
            {
                cmd.CommandText = "SELECT Users.UserID, Users.userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, Users.email, Users.phoneNumber FROM Users ORDER BY Users.UserID;";
            }

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int type = reader.GetInt32(1);
                string role = "";

                if (type == 0) role = "Admin";
                else if (type == 1) role = "Faculty";
                else if (type == 2) role = "Staff";
                else if (type == 3) role = "Student";
                else role = "Unknown";

                Users.Add(new UserDisplay
                {
                    UserID = reader.GetInt32(0),
                    UserTypeName = role,
                    UsersName = reader.GetString(2),

                    /*LastName = reader.GetString(2),*/

                    Email = reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? "" : reader.GetString(4)
                    

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
        }


        public IActionResult OnPostCreateUser()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/AllUsers");
            }

            DBClass.CreateHashedUser(NewUser);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/AllUsers");
            
        }

        public IActionResult OnPostUpdateUser()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/AllUsers");
            }

            DBClass.UpdateHashedUser(UpdateUser);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/AllUsers");
        }
    }
}