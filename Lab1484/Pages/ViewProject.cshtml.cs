using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ViewProjectModel : PageModel
    {
        [BindProperty]
        public int ProjectID { get; set; }

        public Project SpecProject { get; set; } = new Project();

        public List<Note> Notes { get; set; } = new List<Note>();

        [BindProperty]
        public string NewNoteBody { get; set; }

        [TempData]
        public string? CreateProjectNoteSuccess { get; set; }

        [TempData]
        public string? CreateProjectNoteFailure { get; set; }

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            //Get selected ProjectID
            int? ProjectIDFromSession = HttpContext.Session.GetInt32("ProjectID");
            if (ProjectIDFromSession == null)
            {
                // handle error or redirect safely
                return RedirectToPage("/GrantsAndProjects");
            }

            //Get data from selected Project
            ProjectID = ProjectIDFromSession.Value;

            SqlDataReader projectReader = DBClass.SpecProjectReader(ProjectID);
            if (projectReader.Read())
            {
                SpecProject = new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString()),
                    ProjectName = projectReader["ProjectName"].ToString(),
                    DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate")),
                    ProjectStatus = projectReader["ProjectStatus"].ToString(),
                    AdminName = projectReader["AdminName"].ToString(),
                    AdminEmail = projectReader["AdminEmail"].ToString()
                };

            }
            else
            {
                SpecProject = new Project();
            }

            projectReader.Close();

            Notes = DBClass.GetProjNotes(SpecProject.ProjectID);


            return Page();
        }

        public IActionResult OnPostAddNote()
        {
            //Add note when textbox isn't null
            if (!string.IsNullOrWhiteSpace(NewNoteBody))
            {
                Note newNote = new Note()
                {
                    ProjectID = ProjectID,
                    NoteBody = NewNoteBody
                };
                bool success = DBClass.InsertNote(newNote);

                if (success)
                {
                    CreateProjectNoteSuccess = "Note was successfully added.";
                    return RedirectToPage();
                }

                else
                {
                    CreateProjectNoteFailure = "Error: Note could not be added.";
                }
            }
            else
            {
                CreateProjectNoteFailure = "Error: Note could not be added.";
            }

            Notes = DBClass.GetProjNotes(SpecProject.ProjectID);

            return RedirectToPage();
        }
    }
}
