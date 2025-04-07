using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Lab1484.Pages
{
    public class AllUsersModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; } = new User();
        [BindProperty]
        public int? UserType { get; set; }
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

            if (UserType.HasValue)
            {
                cmd.Connection = DBClass.Lab3DBConnection;
                cmd.Connection.ConnectionString = "Server=LocalHost;Database=Lab3;Trusted_Connection=True";
                cmd.CommandText = "SELECT userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, email, phoneNumber FROM Users WHERE UserType = @UserType";
                cmd.Parameters.AddWithValue("@UserType", UserType.Value);
                cmd.Connection.Open();
            }
            else
            {
                cmd.Connection = DBClass.Lab3DBConnection;
                cmd.Connection.ConnectionString = "Server=LocalHost;Database=Lab3;Trusted_Connection=True";
                cmd.CommandText = "SELECT userType, Concat(Users.firstName, ' ', Users.lastName) AS UsersName, email, phoneNumber FROM Users";
                cmd.Connection.Open();
            }

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int type = reader.GetInt32(0);
                string role = "";

                if (type == 0) role = "Admin";
                else if (type == 1) role = "Faculty";
                else if (type == 2) role = "Staff";
                else if (type == 3) role = "Student";
                else role = "Unknown";

                Users.Add(new UserDisplay
                {
                    UserTypeName = role,
                    UsersName = reader.GetString(1),

                    /*LastName = reader.GetString(2),*/

                    Email = reader.GetString(2),
                    Phone = reader.IsDBNull(3) ? "" : reader.GetString(3)
                });
            }

            reader.Close();
            DBClass.Lab3DBConnection.Close();

            return Page();
        }




        public class UserDisplay
        {

            public string UserTypeName { get; set; }
            public string UsersName { get; set; }

            /*public string LastName { get; set; }*/
            public string Email { get; set; }
            public string Phone { get; set; }
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
    }
}