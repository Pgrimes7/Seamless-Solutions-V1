using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class UpdateProjectModel : PageModel
    {
        [BindProperty]
        public int ProjectID { get; set; }

        public Project Proj { get; set; } = new Project();

        [BindProperty]
        public Project UpdateProj { get; set; } = new Project();

        [TempData]
        public string? CreateOrEditProjectSuccess { get; set; }

        [TempData]
        public string? CreateOrEditProjectFailure { get; set; }

        public IActionResult OnPost()
        {
            //UPDATE THESE VALUES
            //UpdateProj.ProjectID = ProjectID;
            //UpdateProj.ProjectName = NewProjectName;
            //UpdateProj.DateDue = Proj.DateDue;
            //UpdateProj.ProjectStatus = "";

            TempData["ProjectID"] = ProjectID;
            TempData.Keep("ProjectID");
            return RedirectToPage("/UpdateProject");
        }

        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            if (TempData.ContainsKey("ProjectID"))
            {
                ProjectID = (int)TempData["ProjectID"];
            }

            SqlDataReader projectReader = DBClass.SingleProjectReader(ProjectID);
            while (projectReader.Read())
            {
                UpdateProj.ProjectID = ProjectID;
                UpdateProj.ProjectName = projectReader["ProjectName"].ToString();
                UpdateProj.DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate"));
                UpdateProj.ProjectStatus = projectReader["ProjectStatus"].ToString();
            }

            return Page();
        }

        public IActionResult OnPostUpdateProjectPost()
        {
            bool success = DBClass.UpdateProject(UpdateProj);
           

            if (success)
            {
                CreateOrEditProjectSuccess = "Project was successfully updated.";
            }
            else
            {
                CreateOrEditProjectFailure = "Error: Project could not be updated.";
                return RedirectToPage("/UpdateProject");
            }

            return RedirectToPage("/GrantsAndProjects");
        }
    }
}
