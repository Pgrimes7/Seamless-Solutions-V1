using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;//Links to DB class and Dataclasses folders
using System.Data.SqlClient;

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
        public void OnPostProjectSelect()//Produce output after clicking on drop down project list
        {
            SelectMessage = "Selection Project was: " + SelectedProject;
        }

        public void OnGet()
        {

            SqlDataReader productReader = DBClass.ProjectReader();//invokes data from project table
            while (productReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(productReader["ProjectID"].ToString()),
                    ProjectName = productReader["ProjectName"].ToString(),
                    DateDue = productReader.GetDateTime(productReader.GetOrdinal("dueDate")), // Directly retrieve as DateTime
                    DateCreated = productReader.GetDateTime(productReader.GetOrdinal("dateCreated")), // Directly retrieve as DateTime
                    DateCompleted = productReader.GetDateTime(productReader.GetOrdinal("dateCompleted")),
                    AdminName = productReader["AdminName"].ToString()
                });
            }

            SqlDataReader grantReader = DBClass.GrantReader();//instntiates class to read grant table and produce all available summary data
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    submissionDate = grantReader.GetDateTime(grantReader.GetOrdinal("submissionDate")),
                    awardDate = grantReader.GetDateTime(grantReader.GetOrdinal("awardDate")),
                    facultyName = grantReader["FacultyLead"].ToString()
                });
            }

            // Close your connection in DBClass
            DBClass.OrgGrantDBConnection.Close();

        }


    }
}
