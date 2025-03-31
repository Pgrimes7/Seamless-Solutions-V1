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
                Proj.ProjectID = ProjectID;
                Proj.ProjectName = projectReader["ProjectName"].ToString();
                Proj.DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate"));
                Proj.DateCreated = projectReader.GetDateTime(projectReader.GetOrdinal("dateCreated"));
                Proj.DateCompleted = projectReader.GetDateTime(projectReader.GetOrdinal("dateCompleted"));
                Proj.AdminName = projectReader["AdminName"].ToString();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            TempData["ProjectID"] = ProjectID;
            TempData.Keep("ProjectID");
            return RedirectToPage();
        }
    }
}
