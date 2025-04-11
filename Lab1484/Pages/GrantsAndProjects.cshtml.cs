using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class GrantsAndProjectsModel : PageModel
    {

        [BindProperty] public int SelectedProject { get; set; }
        public string SelectMessage { get; set; }

        public List<Project> ProjectList { get; set; }
        public List<Grant> GrantList { get; set; }

        public List<User> AdminList { get; set; } = new List<User>();

        public List<User> PartnerList { get; set; } = new List<User>();

        public List<User> EmployeeList { get; set; } = new List<User>();

        [BindProperty]
        public List<int> EmployeeIDs { get; set; } = new List<int>();

        [BindProperty]
        public Grant newGrant { get; set; } = new Grant();

        [BindProperty]
        public Project newProject { get; set; } = new Project();

        public GrantsAndProjectsModel()
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


            if (DBClass.checkUserType(HttpContext) != 0)
            {

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
                        AdminEmail = projectReader["AdminEmail"].ToString(),
                        ProjectStatus = projectReader["ProjectStatus"].ToString()
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

                SqlDataReader employeeReader = DBClass.EmployeeReader();//instntiates class to read grant table and produce all available summary data
                while (employeeReader.Read())
                {
                    EmployeeList.Add(new User
                    {
                        userID = Int32.Parse(employeeReader["userID"].ToString()),
                        firstName = (string)employeeReader["firstName"],
                        lastName = (string)employeeReader["lastName"]
                    });
                }

                SqlDataReader grantReader = DBClass.User_GrantReader(currentUserID);
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
            }

            else
            {
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
                        AdminEmail = projectReader["AdminEmail"].ToString(),
                        ProjectStatus = projectReader["ProjectStatus"].ToString()
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

                SqlDataReader employeeReader = DBClass.EmployeeReader();//instntiates class to read grant table and produce all available summary data
                while (employeeReader.Read())
                {
                    EmployeeList.Add(new User
                    {
                        userID = Int32.Parse(employeeReader["userID"].ToString()),
                        firstName = (string)employeeReader["firstName"],
                        lastName = (string)employeeReader["lastName"]
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
            }
            return Page();
        }

        public IActionResult OnPostInsertGrant()
        {
            DBClass.InsertGrant(newGrant);

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
                    AdminEmail = projectReader["AdminEmail"].ToString(),
                    ProjectStatus = projectReader["ProjectStatus"].ToString()
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

            SqlDataReader employeeReader = DBClass.EmployeeReader();//instntiates class to read grant table and produce all available summary data
            while (employeeReader.Read())
            {
                EmployeeList.Add(new User
                {
                    userID = Int32.Parse(employeeReader["userID"].ToString()),
                    firstName = (string)employeeReader["firstName"],
                    lastName = (string)employeeReader["lastName"]
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

            return Page();
        }

        public IActionResult OnPostInsertProject()
        {
            //Inserts new project into DB and returns its ProjectID
            int newProjectID = DBClass.InsertProject(newProject);
            DBClass.Lab3DBConnection.Close();

            //Uses the returned ProjectID to insert entries for each employee in EmployeeProject
            foreach (int employeeID in EmployeeIDs)
            {
                DBClass.InsertEmployeeProject(newProjectID, employeeID);
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
                    AdminEmail = projectReader["AdminEmail"].ToString(),
                    ProjectStatus = projectReader["ProjectStatus"].ToString()
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

            SqlDataReader employeeReader = DBClass.EmployeeReader();//instntiates class to read grant table and produce all available summary data
            while (employeeReader.Read())
            {
                EmployeeList.Add(new User
                {
                    userID = Int32.Parse(employeeReader["userID"].ToString()),
                    firstName = (string)employeeReader["firstName"],
                    lastName = (string)employeeReader["lastName"]
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

            DBClass.Lab3DBConnection.Close();

            return Page();
        }
    }
}
