using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Lab1484.Pages
{
    public class UpdatePermissionModel : PageModel
    {
        // Lists to store grants and users
        public List<Grant> GrantList { get; set; }
        public List<User> UserList { get; set; }
        public List<User> NonGrantUserList { get; set; }

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

        [BindProperty]
        public int Permission { get; set; }
        
        [BindProperty]
        public int SelectedGrantId { get; set; }
        [BindProperty]
        public string SelectedGrantName { get; set; }

        [TempData]
        public string? CreateOrEditGrantSuccess { get; set; }

        [TempData]
        public string? CreateOrEditGrantFailure { get; set; }


        public UpdatePermissionModel()
        {
            // Initialize the lists
            GrantList = new List<Grant>();
            UserList = new List<User>();
            NonGrantUserList = new List<User>();

        }

        //Adding OnPost to capture the tempdata value after creating a new grant
        //public IActionResult OnPost()
        //{
        //    //Check to see if the user is logged in
        //    string currentUser = HttpContext.Session.GetString("username");
        //    //Redirect them if they aren't
        //    if (string.IsNullOrEmpty(currentUser))
        //    {
        //        return RedirectToPage("/Login");
        //    }

        //    HttpContext.Session.SetInt32("GrantID", grantId);

        //    return RedirectToPage("/UpdatePermission");
        //}

        public IActionResult OnGet()
        {

            int? grantId = HttpContext.Session.GetInt32("GrantID");

            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

           

            if (grantId.HasValue)
            {
                SelectedGrantId = grantId.Value;
                // Load data based on SelectedGrantId
                SqlDataReader userReader = DBClass.Grant_UserReader(SelectedGrantId);
                UserList.Clear();
                NonGrantUserList.Clear();
                HttpContext.Session.Remove("GrantID");
                using (SqlDataReader grantReader = DBClass.SingleGrantReader(SelectedGrantId))
                {
                    while (grantReader.Read())
                    {
                        GrantList.Add(new Grant
                        {
                            grantName = grantReader["grantName"].ToString(),
                            GrantID = (int)grantReader["GrantID"],
                            businessName = grantReader["businessName"].ToString(),
                            amount = Double.Parse(grantReader["amount"].ToString()),
                            category = grantReader["category"].ToString(),
                            dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                            facultyName = grantReader["FacultyLead"].ToString(),
                            grantStatus = grantReader["grantStatus"].ToString()
                        });
                        SelectedGrantName = grantReader["grantName"].ToString();
                    }
                }

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

                using (SqlDataReader grantNonUserList = DBClass.readNonGrant_User(SelectedGrantId))
                {
                    while (grantNonUserList.Read())
                    {
                        NonGrantUserList.Add(new User
                        {
                            userID = Int32.Parse(grantNonUserList["UserID"].ToString()),
                            firstName = grantNonUserList["FirstName"].ToString(),
                            lastName = grantNonUserList["LastName"].ToString(),
                            email = grantNonUserList["Email"].ToString(),
                            phone = grantNonUserList["PhoneNumber"].ToString(),
                        });
                    }
                }
            }
            else
            {
                // Load default data if no grant is selected
                SqlDataReader grantReader = DBClass.GrantReader(null);
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        grantName = grantReader["grantName"].ToString(),
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
        

            return Page();
        }

        public void OnPostSelectGrant(int grantId)
        {

            SelectedGrantId = grantId;
            SqlDataReader userReader = DBClass.Grant_UserReader(grantId);
            UserList.Clear();
            //GrantList.Clear();
            NonGrantUserList.Clear();

            using (SqlDataReader grantReader = DBClass.SingleGrantReader(grantId))
            {
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        grantName = grantReader["grantName"].ToString(),
                        GrantID = (int)grantReader["GrantID"],
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString()
                    });
                    SelectedGrantName = grantReader["grantName"].ToString();
                }
                
            }

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

            using (SqlDataReader grantNonUserList = DBClass.readNonGrant_User(grantId))
            {
                while (grantNonUserList.Read())
                {
                    NonGrantUserList.Add(new User
                    {
                        userID = Int32.Parse(grantNonUserList["UserID"].ToString()),
                        firstName = grantNonUserList["FirstName"].ToString(),
                        lastName = grantNonUserList["LastName"].ToString(),
                        email = grantNonUserList["Email"].ToString(),
                        phone = grantNonUserList["PhoneNumber"].ToString(),
                    });
                }
            }

           
        }

        public IActionResult OnPostUpdatePermission()
        {
            // Create a grant_user object
            var grantUser = new grant_user
            {
                userID = userId,
                grantID = grantId,
                viewPermission = (Permission == 1) ? 1 : 0,
                editPermission = (Permission == 2) ? 1 : 0,
                sensitiveInfoPermission = sensitiveInfoPermission ? 1 : 0
            };

            // Update the database with the new permission values
            DBClass.updatePermission(grantUser);
            HttpContext.Session.SetInt32("GrantID", grantId);
            return RedirectToPage();
        }

        public IActionResult OnPostAddUser(int UserID, int GrantID)
        {
            
     
            UserID = userId;
            GrantID = grantId;
            DBClass.addGrantUser(GrantID, UserID);



            HttpContext.Session.SetInt32("GrantID", GrantID);
            return RedirectToPage();//pass the grantId to the httpcontext so it can be used in the OnGet method
            //this means page reloads without losing the grantId and grant previously selected after add user!







        }



    }
}