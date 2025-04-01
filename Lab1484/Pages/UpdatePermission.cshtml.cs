using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class UpdatePermissionModel : PageModel
    {
        // Lists to store grants and users
        public List<Grant> GrantList { get; set; }
        public List<User> UserList { get; set; }

        [BindProperty]
        public bool viewPermission { get; set; }

        [BindProperty]
        public bool editPermission { get; set; }

        [BindProperty]
        public bool sensitiveInfoPermission { get; set; }

        [BindProperty]
        public int userId { get; set; }

        [BindProperty]
        public int grantId { get; set; }

        public int SelectedGrantId { get; set; }

        public UpdatePermissionModel()
        {
            // Initialize the lists
            GrantList = new List<Grant>();
            UserList = new List<User>();
        }

        public void OnGet()
        {
            SqlDataReader grantReader = DBClass.GrantReader();
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    grantName = grantReader["category"].ToString(),
                    GrantID = (int)grantReader["GrantID"],
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    facultyName = grantReader["FacultyLead"].ToString(),
                    grantStatus = grantReader["grantStatus"].ToString()
                });
            }
        }

        public void OnPostSelectGrant(int grantId)
        {
            SelectedGrantId = grantId;
            SqlDataReader userReader = DBClass.Grant_UserReader(grantId);
            UserList.Clear();
            GrantList.Clear();
            while (userReader.Read())
            {
                UserList.Add(new User
                {
                    userID = Int32.Parse(userReader["UserID"].ToString()),
                    firstName = userReader["FirstName"].ToString(),
                    lastName = userReader["LastName"].ToString(),
                    email = userReader["Email"].ToString(),
                    phone = userReader["PhoneNumber"].ToString(),
                });
            }
        }

        public IActionResult OnPostUpdatePermission()
        {
            // Create a grant_user object
            var grantUser = new grant_user
            {
                userID = userId,
                grantID = grantId,
                viewPermission = viewPermission ? 1 : 0,
                editPermission = editPermission ? 1 : 0,
                sensitiveInfoPermission = sensitiveInfoPermission ? 1 : 0
            };

            // Update the database with the new permission values
            DBClass.updatePermission(grantUser);

            return RedirectToPage();
        }
    }
}