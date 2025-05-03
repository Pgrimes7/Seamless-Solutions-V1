using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class TasksModel : PageModel
    {
        public int CurrentUserType { get; set; }
        public List<ProjTask> tasks { get; set; } = new List<ProjTask>();

        public List<Project> projects { get; set; } = new List<Project>();

        public List<User> users { get; set; } = new List<User>();

        [BindProperty]
        public ProjTask NewTask { get; set; } = new ProjTask();

        public List<GrantTask> gtasks { get; set; } = new List<GrantTask>();

        public List<Grant> grants { get; set; } = new List<Grant>();

        [BindProperty]
        public GrantTask NewGrantTask { get; set; } = new GrantTask();

        [TempData]
        public string? CreateGrantTaskSuccess { get; set; }

        [TempData]
        public string? CreateGrantTaskFailure { get; set; }

        [TempData]
        public string? CreateProjTaskSuccess { get; set; }

        [TempData]
        public string? CreateProjTaskFailure { get; set; }

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            CurrentUserType = DBClass.checkUserType(HttpContext);

            if (DBClass.checkUserType(HttpContext) == 0)
            {
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
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader userTaskReader = DBClass.UserTaskReader(CurrentUserID);
                while (userTaskReader.Read())
                {
                    tasks.Add(new ProjTask
                    {
                        TaskID = Int32.Parse(userTaskReader["TaskID"].ToString()),
                        ProjectID = Int32.Parse(userTaskReader["ProjectID"].ToString()),
                        UserID = Int32.Parse(userTaskReader["UserID"].ToString()),
                        UserName = userTaskReader["UserName"].ToString(),
                        taskDescription = userTaskReader["taskDescription"].ToString(),
                        ProjectName = userTaskReader["ProjectName"].ToString(),
                        dueDate = userTaskReader.GetDateTime(userTaskReader.GetOrdinal("dueDate")),
                        PTStatus = userTaskReader["PTStatus"].ToString()
                    });
                }
            }

           

            if (DBClass.checkUserType(HttpContext) == 0)
            {
                //Read DB Tasks into tasks list
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
            }
            else
            {
                int CurrentUserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
                SqlDataReader grantTaskReader = DBClass.UserGrantTaskReader(CurrentUserID);
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

            //Read DB Users into users list
            SqlDataReader userReader = DBClass.AllUsersReader();
            while (userReader.Read())
            {
                users.Add(new User
                {
                    userID = Int32.Parse(userReader["UserID"].ToString()),
                    firstName = userReader["firstName"].ToString(),
                    lastName = userReader["lastName"].ToString()
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

        public IActionResult OnPostAddGrantTask()
        {
            if (!string.IsNullOrWhiteSpace(NewGrantTask.taskDescription))
            {
                bool success = DBClass.InsertGrantTask(NewGrantTask);

                if (success)
                {
                    CreateProjTaskSuccess = "Note was successfully added.";

                }

                else
                {
                    CreateProjTaskFailure = "Error: Note could not be added.";
                }
            }
            else
            {
                CreateGrantTaskFailure = "Error: Note could not be added.";
            }


            return RedirectToPage("/Tasks");
        }

        public IActionResult OnPostAddProjTask()
        {
            if (!string.IsNullOrWhiteSpace(NewTask.taskDescription))
            {
                bool success = DBClass.InsertTask(NewTask);

                if (success)
                {
                    CreateProjTaskSuccess = "Note was successfully added.";
                   
                }

                else
                {
                    CreateProjTaskFailure = "Error: Note could not be added.";
                }
            }
            


            else
            {
                CreateGrantTaskFailure = "Error: Note could not be added.";
            }
        
            return RedirectToPage("/Tasks", null, null, "profile-tab-pane");
        }


        public IActionResult OnPostCompleteGrantTask(int GTaskID)
        {
            DBClass.GrantTaskComplete(GTaskID);

            return RedirectToPage();
        }

        public IActionResult OnPostCompleteTask(int TaskID)
        {
            DBClass.TaskComplete(TaskID);

            return RedirectToPage("/Tasks", null, null, "profile-tab-pane");
        }
    }
}
