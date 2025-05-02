using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ViewGrantModel : PageModel
    {
        [BindProperty]
        public int GrantID { get; set; }

        [BindProperty]
        public int userId { get; set; }

        public List<Grant> GrantList { get; set; } = new List<Grant>();

        public List<User> UserList { get; set; } = new List<User>();

        public List<User> NonGrantUserList { get; set; } = new List<User>();

        public List<GrantTask> GrantTaskList { get; set; } = new List<GrantTask>();

        public Grant SpecGrant { get; set; }

        [BindProperty]
        public int Permission { get; set; }

        [BindProperty]
        public bool viewPermission { get; set; }

        [BindProperty]
        public bool editPermission { get; set; }

        [BindProperty]
        public bool sensitiveInfoPermission { get; set; }

        [BindProperty]
        public GrantTask NewGrantTask { get; set; } = new GrantTask();

        public List<Grant> grants { get; set; } = new List<Grant>();

        public List<User> users { get; set; } = new List<User>();


        public void OnGet()
        {
            //Get selected GrantID
            int? grantIDFromSession = HttpContext.Session.GetInt32("GrantID");
            if (grantIDFromSession == null)
            {
                // handle error or redirect safely
                RedirectToPage("/GrantsAndProjects");
                return;
            }
            //Get data from selected Grant
            GrantID = grantIDFromSession.Value;

            SqlDataReader grantReader = DBClass.SpecGrantReader(GrantID);
            if (grantReader.Read())
            {
                SpecGrant = new Grant
                {
                    grantName = grantReader["grantName"].ToString(),
                    businessName = grantReader["businessName"].ToString(),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    grantStatus = grantReader["grantStatus"].ToString(),
                    amount = double.Parse(grantReader["amount"].ToString()),
                    facultyName = grantReader["UserName"].ToString()
                };

            } else
            {
                SpecGrant = new Grant();
            }

            grantReader.Close();

            //Read associated GrantUsers, steal code to add/update permissions for GrantUsers
            SqlDataReader userReader = DBClass.Grant_UserReader(GrantID);
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

            using (SqlDataReader grantNonUserList = DBClass.readNonGrant_User(GrantID))
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


            //Read grant task data, steal code to add grant tasks
            SqlDataReader grantTaskReader = DBClass.SpecGrantTaskReader(GrantID);
            while (grantTaskReader.Read())
            {
                GrantTaskList.Add(new GrantTask
                {
                    GTaskID = Int32.Parse(grantTaskReader["GTaskID"].ToString()),
                    GrantID = Int32.Parse(grantTaskReader["GrantID"].ToString()),
                    UserID = Int32.Parse(grantTaskReader["UserID"].ToString()),
                    UserName = grantTaskReader["UserName"].ToString(),
                    taskDescription = grantTaskReader["taskDescription"].ToString(),
                    GrantName = grantTaskReader["grantName"].ToString(),
                    dueDate = grantTaskReader.GetDateTime(grantTaskReader.GetOrdinal("dueDate")),
                    GTStatus = grantTaskReader["GTStatus"].ToString()
                });
            }

            grantTaskReader.Close();

            //Read DB Grants into projects list
            SqlDataReader grantAllReader = DBClass.GrantReader(null);
            while (grantAllReader.Read())
            {
                grants.Add(new Grant
                {
                    GrantID = Int32.Parse(grantAllReader["GrantID"].ToString()),
                    grantName = grantAllReader["grantName"].ToString()
                });
            }

            //Read DB Users into users list
            SqlDataReader userNewReader = DBClass.AllUsersReader();
            while (userNewReader.Read())
            {
                users.Add(new User
                {
                    userID = Int32.Parse(userNewReader["UserID"].ToString()),
                    firstName = userNewReader["firstName"].ToString(),
                    lastName = userNewReader["lastName"].ToString()
                });
            }

            //Set it so that only admins can see everything, regular users can't see create/update buttons

            DBClass.Lab3DBConnection.Close();
        }

        public IActionResult OnPostCompleteGrantTask(int GTaskID)
        {
            DBClass.GrantTaskComplete(GTaskID);

            return RedirectToPage();
        }

        public IActionResult OnPostUpdatePermission()
        {
            // Create a grant_user object
            var grantUser = new grant_user
            {
                userID = userId,
                grantID = GrantID,
                viewPermission = (Permission == 1) ? 1 : 0,
                editPermission = (Permission == 2) ? 1 : 0,
                sensitiveInfoPermission = sensitiveInfoPermission ? 1 : 0
            };

            // Update the database with the new permission values
            DBClass.updatePermission(grantUser);
            HttpContext.Session.SetInt32("GrantID", GrantID);
            return RedirectToPage();
        }

        public IActionResult OnPostAddUser(int UserID, int GrantID)
        {
            UserID = userId;
            int UserGrantID = GrantID;
            DBClass.addGrantUser(UserGrantID, UserID);

            HttpContext.Session.SetInt32("GrantID", GrantID);
            return RedirectToPage();//pass the grantId to the httpcontext so it can be used in the OnGet method
            //this means page reloads without losing the grantId and grant previously selected after add user!
        }

        public IActionResult OnPostAddGrantTask()
        {
            if (!string.IsNullOrWhiteSpace(NewGrantTask.taskDescription))
            {
                DBClass.InsertGrantTask(NewGrantTask);
            }
            return RedirectToPage();
        }
    }
}
