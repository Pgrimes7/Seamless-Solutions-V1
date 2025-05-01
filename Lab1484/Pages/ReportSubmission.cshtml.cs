using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ReportSubmissionModel : PageModel
    {


        [BindProperty]
        public string ReportName { get; set; }
        [BindProperty]
        public List<int> GrantIDs { get; set; }
        [BindProperty]
        public List<int> ProjectIDs { get; set; }
        [BindProperty]
        public List<string> SubjectTitle { get; set; }
        [BindProperty]
        public List<string> SubjectText { get; set; }
        [BindProperty]
        public List<Report> ReportList { get; set; }

        public List<Grant> GrantList { get; set; }
        public List<Project> ProjectList { get; set; }

        public ReportSubmissionModel()
        {
            ProjectList = new List<Project>();
            GrantList = new List<Grant>();
            ReportList = new List<Report>();
        }
       

        public IActionResult OnGet()
        {
            // Check if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            // Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }
            SqlDataReader reportReader = DBClass.AllReportReader();
            while (reportReader.Read())
            {
                ReportList.Add(new Report
                {
                    ReportName = reportReader["ReportName"].ToString(),
                    ReportDate = reportReader.GetDateTime(reportReader.GetOrdinal("ReportDate"))



                });
            }
            SqlDataReader grantReader = DBClass.GrantReader(null);
            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    facultyName = grantReader["FacultyLead"].ToString(),
                    facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                    grantStatus = grantReader["grantStatus"].ToString(),
                    grantName = grantReader["grantName"].ToString()
                });
            }
            SqlDataReader projectReader = DBClass.ProjectReader(null);
            while (projectReader.Read())
            {
                ProjectList.Add(new Project
                {
                    ProjectID = Int32.Parse(projectReader["ProjectID"].ToString()),
                    ProjectName = projectReader["ProjectName"].ToString(),
                    DateDue = projectReader.GetDateTime(projectReader.GetOrdinal("dueDate")),
                    DateCreated = projectReader.GetDateTime(projectReader.GetOrdinal("dateCreated")),
                    DateCompleted = projectReader.GetDateTime(projectReader.GetOrdinal("dateCompleted")),
                    AdminName = projectReader["AdminName"].ToString(),
                    AdminEmail = projectReader["AdminEmail"].ToString(),
                    ProjectStatus = projectReader["ProjectStatus"].ToString()
                });
            }


            return Page();
        }

      


        public async Task<IActionResult> OnPostAsync()
        {
            // Debugging: Check if GrantIDs and ProjectIDs are populated
            if (GrantIDs == null || !GrantIDs.Any())
            {
                throw new Exception("GrantIDs are not populated.");
            }
            if (ProjectIDs == null || !ProjectIDs.Any())
            {
                throw new Exception("ProjectIDs are not populated.");
            }

            var report = new Report
            {
                ReportDate = DateTime.Now,
                ReportName = ReportName
            };

            var subjects = new List<ReportSubject>();
            for (int i = 0; i < SubjectTitle.Count; i++)
            {
                subjects.Add(new ReportSubject
                {
                    SubjectTitle = SubjectTitle[i],
                    SubjectText = SubjectText[i]
                });
            }

            DBClass.InsertReport(report, GrantIDs, ProjectIDs, subjects);

            return RedirectToPage("/ReportSubmission");
        }

    }
}
