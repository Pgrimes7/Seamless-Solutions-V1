using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;

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
        public List<ProjTask> TaskList { get; set; } = new List<ProjTask>();

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

            SqlDataReader taskReader = DBClass.TaskReader();
            while (taskReader.Read())
            {
                TaskList.Add(new ProjTask
                {
                    TaskID = Int32.Parse(taskReader["TaskID"].ToString()),
                    //GrantID = Int32.Parse(taskReader["GrantID"].ToString()),
                    ProjectID = Int32.Parse(taskReader["ProjectID"].ToString()),
                    //UserID = Int32.Parse(taskReader["UserID"].ToString()),
                    taskDescription = taskReader["taskDescription"].ToString(),
                    //EmployeeName = taskReader["EmployeeName"].ToString(),
                    ProjectName = taskReader["ProjectName"].ToString(),
                    //grantName = taskReader["grantName"].ToString(),
                    dueDate = taskReader.GetDateTime(taskReader.GetOrdinal("dueDate"))
                });
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

            SqlDataReader taskReader = DBClass.TaskReader();
            while (taskReader.Read())
            {
                TaskList.Add(new ProjTask
                {
                    TaskID = Int32.Parse(taskReader["TaskID"].ToString()),
                    //GrantID = Int32.Parse(taskReader["GrantID"].ToString()),
                    ProjectID = Int32.Parse(taskReader["ProjectID"].ToString()),
                    //UserID = Int32.Parse(taskReader["UserID"].ToString()),
                    taskDescription = taskReader["taskDescription"].ToString(),
                    //EmployeeName = taskReader["EmployeeName"].ToString(),
                    ProjectName = taskReader["ProjectName"].ToString(),
                    //grantName = taskReader["grantName"].ToString(),
                    dueDate = taskReader.GetDateTime(taskReader.GetOrdinal("dueDate"))
                });
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
