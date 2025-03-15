using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ProjectCreationModel : PageModel
    {
        [BindProperty]
        public String ProjectName { get; set; }

        [BindProperty]
        public int AdminID { get; set; }

        [BindProperty]
        public List<int> EmployeeIDs { get; set; } = new List<int>();

        public List<Project> ProjectList { get; set; } = new List<Project>();

        [BindProperty]
        public float ProjectCost { get; set; }

        [BindProperty]
        public DateTime DueDate { get; set; }

        public String? CompletionDate { get; set; } = "2099-01-01";

        public string DateCreated { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [BindProperty]
        public String ProjectStatus { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

        [BindProperty]
        public List<User> EmployeeList { get; set; } = new List<User>();

        [BindProperty]
        public List<User> AdminList { get; set; } = new List<User>();

        [BindProperty]
        public Project NewProject { get; set; } = new Project();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader adminReader = DBClass.AdminReader();//instntiates class to read grant table and produce all available summary data
            while (adminReader.Read())
            {
                AdminList.Add(new User
                {
                    userID = Int32.Parse(adminReader["userID"].ToString()),
                    firstName = (string)adminReader["firstName"],
                    lastName = (string)adminReader["lastName"]
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
            return Page();
        }

        public IActionResult OnPost()
        {
            //Inserts new project into DB and returns its ProjectID
            int newProjectID = DBClass.InsertProject(NewProject);
            DBClass.Lab3DBConnection.Close();

            //Uses the returned ProjectID to insert entries for each employee in EmployeeProject
            foreach (int employeeID in EmployeeIDs)
            {
                DBClass.InsertEmployeeProject(newProjectID, employeeID);
            }

            return RedirectToPage("/ProjectCreation");
        }
        

        //Method with sample data for the populate button
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            ProjectName = "Sample Project";

            //Admin to use as sample data
            SqlDataReader adminReader = DBClass.AdminReader();//instntiates class to read grant table and produce all available summary data
            while (adminReader.Read())
            {
                AdminList.Add(new User
                {
                    userID = Int32.Parse(adminReader["userID"].ToString()),
                    firstName = (string)adminReader["firstName"],
                    lastName = (string)adminReader["lastName"]
                });
                if (AdminList != null && AdminList.Count > 0)
                {
                    // Select the first Admin as a sample
                    var selectedPartner = AdminList.FirstOrDefault();
                    if (selectedPartner != null)
                    {
                        AdminID = selectedPartner.userID;
                    }
                }
            }

            ProjectCost = 50000;
            DueDate = DateTime.Now.AddMonths(6);

            //Employee to use as sample data
            SqlDataReader employeeReader = DBClass.EmployeeReader();//instntiates class to read grant table and produce all available summary data
            while (employeeReader.Read())
            {
                EmployeeList.Add(new User
                {
                    userID = Int32.Parse(employeeReader["userID"].ToString()),
                    firstName = (string)employeeReader["firstName"],
                    lastName = (string)employeeReader["lastName"]
                });
                if (EmployeeList != null && EmployeeList.Count > 0)
                {
                    // Select the first Admin as a sample
                    var selectedPartner = EmployeeList.FirstOrDefault();
                    if (selectedPartner != null)
                    {
                        EmployeeID = selectedPartner.userID;
                    }
                }
            }

            ProjectStatus = "Incomplete";

            return Page();
        }


    }
    }
