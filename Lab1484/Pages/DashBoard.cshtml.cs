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
        public void OnPostProjectSelect()
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

            SqlDataReader grantReader = DBClass.GrantReader();
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    GrantName = grantReader["GrantName"].ToString(),
                    GrantAmount = Double.Parse(grantReader["GrantAmount"].ToString()),
                    ProjectName = grantReader["AssignedEmployee"].ToString()
                });
            }

            // Close your connection in DBClass
            DBClass.OrgGrantDBConnection.Close();

        }


    }
}
