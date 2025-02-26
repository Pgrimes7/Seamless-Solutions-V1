using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ProjectCreationModel : PageModel
    {

        /*Date time button to select dates associated with project? Or it auto does it when created idk
         * String input for project name
         * String input for project status? Or, radio buttons to do options liek approved, unapproved etc..
         * Need AdminID selection, access by select statement where userType = admin
         */
        [BindProperty]
        public String ProjectName { get; set; }

        [BindProperty]
        public int AdminID { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

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
        public List<User> EmployeeList { get; set; } = new List<User>();

        public List<User> AdminList { get; set; } = new List<User>();

        public void OnGet()
        {
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
        }

        public IActionResult OnPost()
        {
            string query = "INSERT INTO Project " +
                "(ProjectAdminID, projectStatus, dateCreated, dateCompleted, dueDate, projectName)" +
                " VALUES (@ProjectAdminID, @ProjectStatus, @DateCreated, @CompletionDate, @DueDate, @ProjectName);";

            using (var connection = new SqlConnection("Server=LocalHost;Database=OrgGrant;Trusted_Connection=True"))
            {
                connection.Open();

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProjectAdminID", AdminID);
                    cmd.Parameters.AddWithValue("@ProjectStatus", ProjectStatus);
                    cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
                    cmd.Parameters.AddWithValue("@CompletionDate", CompletionDate);
                    cmd.Parameters.AddWithValue("@DueDate", DueDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@ProjectName", ProjectName);

                    cmd.ExecuteNonQuery();
                }
            }
            SqlDataReader projectReader = DBClass.ProjectReader();//instntiates class to read grant table and produce all available summary data
            while (projectReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString())
                });
            }
            string query2 = "INSERT INTO EmployeeProject " +
                "(ProjectID, EmployeeID) VALUES (@ProjectID, @EmployeeID)";

            using (var connection = new SqlConnection("Server=LocalHost;Database=OrgGrant;Trusted_Connection=True"))
            {
                connection.Open();

                using (var cmd = new SqlCommand(query2, connection))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", ProjectList[ProjectList.Count - 1].ProjectID);
                    cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    cmd.ExecuteNonQuery();
                }
            }

            return Page();
        }

           
        }
    }
