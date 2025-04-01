using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class TasksModel : PageModel
    {
        public List<ProjTask> tasks { get; set; } = new List<ProjTask>();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader taskReader = DBClass.TaskReader();
            while (taskReader.Read())
            {
                tasks.Add(new ProjTask
                {
                    TaskID = Int32.Parse(taskReader["TaskID"].ToString()),
                    ProjectID = Int32.Parse(taskReader["ProjectID"].ToString()),
                    taskDescription = taskReader["taskDescription"].ToString(),
                    ProjectName = taskReader["ProjectName"].ToString()
                });
            }
            return Page();
        }
    }
}
