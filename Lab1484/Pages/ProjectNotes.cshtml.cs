using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ProjectNotesModel : PageModel
    {
        [BindProperty]
        public int ProjectID { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();

        [BindProperty]
        public string NewNoteBody { get; set; }

        [TempData]
        public string? CreateProjectNoteSuccess { get; set; }

        [TempData]
        public string? CreateProjectNoteFailure { get; set; }


        public IActionResult OnPost()
        {
            TempData["ProjectID"] = ProjectID;
            TempData.Keep("ProjectID");
            return RedirectToPage();
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

            if (TempData.ContainsKey("ProjectID"))
            {
                ProjectID = (int)TempData["ProjectID"];
                Notes = DBClass.GetProjNotes(ProjectID);
            }

            return Page();
        }

        public IActionResult OnPostAddNote()
        {
            //Add note when textbox isn't null
            if(!string.IsNullOrWhiteSpace(NewNoteBody))
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
                    return RedirectToPage("/GrantsAndProjects");
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

            Notes = DBClass.GetProjNotes(ProjectID);

            return Page();
        }
    }
}
