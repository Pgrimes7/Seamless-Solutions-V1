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

        public List<Project> projects { get; set; } = new List<Project>();

        [BindProperty]
        public ProjTask NewTask { get; set; } = new ProjTask();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            //Read DB Tasks into tasks list
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

            //Read DB Projects into projects list
            SqlDataReader projReader = DBClass.ProjectReader();
            while (projReader.Read())
            {
                projects.Add(new Project
                {
                    ProjectID = Int32.Parse(projReader["ProjectID"].ToString()),
                    ProjectName = projReader["ProjectName"].ToString()
                });
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(NewTask.taskDescription))
            {
                DBClass.InsertTask(NewTask);
            }

            return RedirectToPage("/Tasks");
        }
    }
}
