using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;//Links to DB class and Dataclasses folders
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;

namespace Lab1484.Pages
{
    public class DashBoardModel : PageModel
    {
        [BindProperty] public int SelectedProject { get; set; }
        public String SelectMessage { get; set; }

        public List<Project> ProjectList { get; set; }//Creates project list
        public List<Grant> GrantList { get; set; }//create grant object list
        public DashBoardModel() 
        { 
        ProjectList = new List<Project>();
        GrantList = new List<Grant>();

        }
        public void OnPostProjectSelect()//Produce output after clicking on drop down project list might be able to just remove since doesn't produce msg
        {
            SelectMessage = "Selection Project was: " + SelectedProject;
        }

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader projectReader = DBClass.ProjectReader();//invokes data from project table
            while (projectReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString()),
                    ProjectName = projectReader["ProjectName"].ToString(),
                    DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate")), // Directly retrieve as DateTime
                    DateCreated = projectReader.GetDateTime(projectReader.GetOrdinal("dateCreated")), // Directly retrieve as DateTime
                    DateCompleted = projectReader.GetDateTime(projectReader.GetOrdinal("dateCompleted")),
                    AdminName = projectReader["AdminName"].ToString()
                });
            }

            SqlDataReader grantReader = DBClass.GrantReader();//instantiates class to read grant table and produce all available summary data
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    submissionDate = grantReader.GetDateTime(grantReader.GetOrdinal("submissionDate")),//getOrdinal finds the column index then GetDateTime pulls it from the list
                    awardDate = grantReader.GetDateTime(grantReader.GetOrdinal("awardDate")),
                    facultyName = grantReader["FacultyLead"].ToString()
                });
            }

            // Close your connection in DBClass
            DBClass.Lab2DBConnection.Close();

            return Page();
        }


    }
}
