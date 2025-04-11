using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;

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
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader taskReader = DBClass.TaskReader();
            while (taskReader.Read())
            {
                TaskList.Add(new ProjTask
                {
                    TaskID = Convert.ToInt32(taskReader["TaskID"]),
                    ProjectID = Convert.ToInt32(taskReader["ProjectID"]),
                    taskDescription = taskReader["taskDescription"].ToString(),
                    dueDate = taskReader["duedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(taskReader["duedate"]),
                    ProjectName = taskReader["ProjectName"].ToString()
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

            SqlDataReader projectReader = DBClass.ProjectReader();
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

            SqlDataReader grantReader = DBClass.GrantReader(SearchQuery);
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
            
            DBClass.Lab3DBConnection.Close();

            var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "In Progress", 1 },
                { "Opportunity", 2 },
                { "Pending", 3 },
                { "Approved", 4 },
                { "Rejected", 5 }
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
                { "In Progress", 1 },
                { "Opportunity", 2 },
                { "Pending", 3 },
                { "Approved", 4 },
                { "Rejected", 5 }
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
    }
}
