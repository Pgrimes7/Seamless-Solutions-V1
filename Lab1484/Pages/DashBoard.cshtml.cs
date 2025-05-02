using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lab1484.Pages
{
    public class DashBoardModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }
        [BindProperty] public int SelectedProject { get; set; }
        public string SelectMessage { get; set; }

        public List<Project> ProjectList { get; set; }
        public List<Grant> GrantList { get; set; }
        public List<ProjTask> tasks { get; set; } = new List<ProjTask>();

        public List<GrantTask> gtasks { get; set; } = new List<GrantTask>();

        [BindProperty]
        public Grant newGrant { get; set; } = new Grant();

        public List<User> AdminList { get; set; } = new List<User>();

        public List<User> PartnerList { get; set; } = new List<User>();


        public DashBoardModel()
        {
            ProjectList = new List<Project>();
            GrantList = new List<Grant>();
        }

        public void OnPostProjectSelect()
        {
            SelectMessage = "Selected Project was: " + SelectedProject;
        }

        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            string currentUserIDString = HttpContext.Session.GetString("userID");


            int currentUserID = int.Parse(currentUserIDString);
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            if (DBClass.checkUserType(HttpContext) == 0)
            {
                //Read DB Tasks into tasks list
                SqlDataReader taskReader = DBClass.TaskReader();
                while (taskReader.Read())
                {
                    tasks.Add(new ProjTask
                    {
                        TaskID = Int32.Parse(taskReader["TaskID"].ToString()),
                        ProjectID = Int32.Parse(taskReader["ProjectID"].ToString()),
                        UserID = Int32.Parse(taskReader["UserID"].ToString()),
                        UserName = taskReader["UserName"].ToString(),
                        taskDescription = taskReader["taskDescription"].ToString(),
                        ProjectName = taskReader["ProjectName"].ToString(),
                        dueDate = taskReader.GetDateTime(taskReader.GetOrdinal("dueDate")),
                        PTStatus = taskReader["PTStatus"].ToString()
                    });
                }
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader userTaskReader = DBClass.UserTaskReader(CurrentUserID);
                while (userTaskReader.Read())
                {
                    tasks.Add(new ProjTask
                    {
                        TaskID = Int32.Parse(userTaskReader["TaskID"].ToString()),
                        ProjectID = Int32.Parse(userTaskReader["ProjectID"].ToString()),
                        UserID = Int32.Parse(userTaskReader["UserID"].ToString()),
                        UserName = userTaskReader["UserName"].ToString(),
                        taskDescription = userTaskReader["taskDescription"].ToString(),
                        ProjectName = userTaskReader["ProjectName"].ToString(),
                        dueDate = userTaskReader.GetDateTime(userTaskReader.GetOrdinal("dueDate")),
                        PTStatus = userTaskReader["PTStatus"].ToString()
                    });
                }
            }



            if (DBClass.checkUserType(HttpContext) == 0)
            {
                //Read DB Tasks into tasks list
                SqlDataReader grantTaskReader = DBClass.GrantTaskReader();
                while (grantTaskReader.Read())
                {
                    gtasks.Add(new GrantTask
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
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader grantTaskReader = DBClass.UserGrantTaskReader(CurrentUserID);
                while (grantTaskReader.Read())
                {
                    gtasks.Add(new GrantTask
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
            }

            SqlDataReader adminReader = DBClass.AdminReader();
            while (adminReader.Read())
            {
                AdminList.Add(new User
                {
                    userID = Int32.Parse(adminReader["UserID"].ToString()),
                    firstName = adminReader["firstName"].ToString(),
                    lastName = adminReader["lastName"].ToString()
                });
            }

            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),
                    firstName = partnerReader["firstName"].ToString(),
                    lastName = partnerReader["lastName"].ToString()
                });
            }

            SqlDataReader projectReader = DBClass.ProjectReader(null);
            while (projectReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString()),
                    ProjectName = projectReader["ProjectName"].ToString(),
                    DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate")),
                    DateCreated = projectReader.GetDateTime(projectReader.GetOrdinal("dateCreated")),
                    DateCompleted = projectReader.GetDateTime(projectReader.GetOrdinal("dateCompleted")),
                    AdminName = projectReader["AdminName"].ToString(),
                    ProjectStatus = projectReader["ProjectStatus"].ToString()
                });
            }
            if (DBClass.checkUserType(HttpContext) != 0)
            {
                SqlDataReader grantReader = DBClass.User_GrantReader(currentUserID);
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                        grantName = grantReader["grantName"].ToString(),
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString()
                    });
                }

            }
            else
            {
                SqlDataReader grantReader = DBClass.GrantReader(SearchQuery);
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString(),
                        grantName = grantReader["grantName"].ToString()
                    });
                }
                grantReader.Close();
            }

            
            DBClass.Lab3DBConnection.Close();

            var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Active", 1 },
                { "Funded", 2 },
                { "Potential", 3 },
                { "Rejected", 4 },
                { "Archived", 5 }
            };

            GrantList = GrantList
                .OrderBy(g => statusOrder.ContainsKey(g.grantStatus) ? statusOrder[g.grantStatus] : int.MaxValue)
                .ThenBy(g => g.businessName)
                .ToList();

            return Page();
        }

        public IActionResult OnPostInsertGrant()
        {
            DBClass.InsertGrant(newGrant);

            SqlDataReader adminReader = DBClass.AdminReader();
            while (adminReader.Read())
            {
                AdminList.Add(new User
                {
                    userID = Int32.Parse(adminReader["UserID"].ToString()),
                    firstName = adminReader["firstName"].ToString(),
                    lastName = adminReader["lastName"].ToString()
                });
            }

            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),
                    firstName = partnerReader["firstName"].ToString(),
                    lastName = partnerReader["lastName"].ToString()
                });
            }

            SqlDataReader grantReader = DBClass.GrantReader(null);
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    facultyName = grantReader["FacultyLead"].ToString(),
                    facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                    grantStatus = grantReader["grantStatus"].ToString(),
                    grantName = grantReader["grantName"].ToString()
                });
            }

            DBClass.Lab3DBConnection.Close();

            var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Active", 1 },
                { "Funded", 2 },
                { "Potential", 3 },
                { "Rejected", 4 },
                { "Archived", 5 }
            };

            GrantList = GrantList
                .OrderBy(g => statusOrder.ContainsKey(g.grantStatus) ? statusOrder[g.grantStatus] : int.MaxValue)
                .ThenBy(g => g.businessName)
                .ToList();

            return RedirectToPage("/UpdatePermission");
        }

        public JsonResult OnGetGrantStatusData()
        {
            int[] grantCounts = DBClass.GetGrantStatusCounts();
            return new JsonResult(grantCounts);
        }

        public IActionResult OnPostUpdateGrant()
        {
            //Updates existing grant
            DBClass.UpdateGrant(newGrant);
            DBClass.Lab3DBConnection.Close();

            string currentUser = HttpContext.Session.GetString("username");
            string currentUserIDString = HttpContext.Session.GetString("userID");


            int currentUserID = int.Parse(currentUserIDString);
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            if (DBClass.checkUserType(HttpContext) == 0)
            {
                //Read DB Tasks into tasks list
                SqlDataReader taskReader = DBClass.TaskReader();
                while (taskReader.Read())
                {
                    tasks.Add(new ProjTask
                    {
                        TaskID = Int32.Parse(taskReader["TaskID"].ToString()),
                        ProjectID = Int32.Parse(taskReader["ProjectID"].ToString()),
                        UserID = Int32.Parse(taskReader["UserID"].ToString()),
                        UserName = taskReader["UserName"].ToString(),
                        taskDescription = taskReader["taskDescription"].ToString(),
                        ProjectName = taskReader["ProjectName"].ToString(),
                        dueDate = taskReader.GetDateTime(taskReader.GetOrdinal("dueDate")),
                        PTStatus = taskReader["PTStatus"].ToString()
                    });
                }
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader userTaskReader = DBClass.UserTaskReader(CurrentUserID);
                while (userTaskReader.Read())
                {
                    tasks.Add(new ProjTask
                    {
                        TaskID = Int32.Parse(userTaskReader["TaskID"].ToString()),
                        ProjectID = Int32.Parse(userTaskReader["ProjectID"].ToString()),
                        UserID = Int32.Parse(userTaskReader["UserID"].ToString()),
                        UserName = userTaskReader["UserName"].ToString(),
                        taskDescription = userTaskReader["taskDescription"].ToString(),
                        ProjectName = userTaskReader["ProjectName"].ToString(),
                        dueDate = userTaskReader.GetDateTime(userTaskReader.GetOrdinal("dueDate")),
                        PTStatus = userTaskReader["PTStatus"].ToString()
                    });
                }
            }



            if (DBClass.checkUserType(HttpContext) == 0)
            {
                //Read DB Tasks into tasks list
                SqlDataReader grantTaskReader = DBClass.GrantTaskReader();
                while (grantTaskReader.Read())
                {
                    gtasks.Add(new GrantTask
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
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader grantTaskReader = DBClass.UserGrantTaskReader(CurrentUserID);
                while (grantTaskReader.Read())
                {
                    gtasks.Add(new GrantTask
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
            }

            SqlDataReader adminReader = DBClass.AdminReader();
            while (adminReader.Read())
            {
                AdminList.Add(new User
                {
                    userID = Int32.Parse(adminReader["UserID"].ToString()),
                    firstName = adminReader["firstName"].ToString(),
                    lastName = adminReader["lastName"].ToString()
                });
            }

            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),
                    firstName = partnerReader["firstName"].ToString(),
                    lastName = partnerReader["lastName"].ToString()
                });
            }

            SqlDataReader projectReader = DBClass.ProjectReader(null);
            while (projectReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString()),
                    ProjectName = projectReader["ProjectName"].ToString(),
                    DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate")),
                    DateCreated = projectReader.GetDateTime(projectReader.GetOrdinal("dateCreated")),
                    DateCompleted = projectReader.GetDateTime(projectReader.GetOrdinal("dateCompleted")),
                    AdminName = projectReader["AdminName"].ToString(),
                    ProjectStatus = projectReader["ProjectStatus"].ToString()
                });
            }
            if (DBClass.checkUserType(HttpContext) != 0)
            {
                SqlDataReader grantReader = DBClass.User_GrantReader(currentUserID);
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                        grantName = grantReader["grantName"].ToString(),
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString()
                    });
                }

            }
            else
            {
                SqlDataReader grantReader = DBClass.GrantReader(SearchQuery);
                while (grantReader.Read())
                {
                    GrantList.Add(new Grant
                    {
                        GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString(),
                        grantName = grantReader["grantName"].ToString()
                    });
                }
                grantReader.Close();
            }


            DBClass.Lab3DBConnection.Close();

            var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "Active", 1 },
                { "Funded", 2 },
                { "Potential", 3 },
                { "Rejected", 4 },
                { "Archived", 5 }
            };

            GrantList = GrantList
                .OrderBy(g => statusOrder.ContainsKey(g.grantStatus) ? statusOrder[g.grantStatus] : int.MaxValue)
                .ThenBy(g => g.businessName)
                .ToList();

            return Page();
        }
    }
}
