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

        public List<GrantTask> gtasks { get; set; } = new List<GrantTask>();

        public List<Grant> grants { get; set; } = new List<Grant>();

        public GrantTask NewGrantTask { get; set; } = new GrantTask();

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
                    UserID = Int32.Parse(taskReader["UserID"].ToString()),
                    UserName = taskReader["UserName"].ToString(),
                    taskDescription = taskReader["taskDescription"].ToString(),
                    ProjectName = taskReader["ProjectName"].ToString(),
                    dueDate = taskReader.GetDateTime(taskReader.GetOrdinal("dueDate")),
                    PTStatus = taskReader["PTStatus"].ToString()
                });
            }

            //Read DB Projects into projects list
            SqlDataReader projReader = DBClass.ProjectReader(null);
            while (projReader.Read())
            {
                projects.Add(new Project
                {
                    ProjectID = Int32.Parse(projReader["ProjectID"].ToString()),
                    ProjectName = projReader["ProjectName"].ToString()
                });
            }

            SqlDataReader grantTaskReader = DBClass.GrantTaskReader();
            while (grantTaskReader.Read())
            {
                gtasks.Add(new GrantTask
                {
                    GTaskID = Int32.Parse(grantTaskReader["GTaskID"].ToString()),
                    GrantID = Int32.Parse(grantTaskReader["GrantID"].ToString()),
                    UserID = Int32.Parse(grantTaskReader["UserID"].ToString()),
                    UserName = grantTaskReader["UserName"].ToString(),
                    taskDescription = grantTaskReader["taskDescription"].ToString(),
                    GrantName = grantTaskReader["grantName"].ToString(),
                    dueDate = grantTaskReader.GetDateTime(grantTaskReader.GetOrdinal("dueDate")),
                    GTStatus = grantTaskReader["GTStatus"].ToString()
                });
            }

            //Read DB Projects into projects list
            SqlDataReader grantReader = DBClass.GrantReader(null);
            while (grantReader.Read())
            {
                grants.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    grantName = grantReader["grantName"].ToString()
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
