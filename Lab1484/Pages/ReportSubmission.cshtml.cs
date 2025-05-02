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
        [BindProperty]
        public List<Grant> GrantList { get; set; }
        [BindProperty]
        public List<Project> ProjectList { get; set; }
        [BindProperty]
        public List<string> SelectedGrantOrProjectID { get; set; }
        [BindProperty]
        public List<PerformanceReport> PerformanceReportList { get; set; }
        public User CurrentUserID { get; set; }
        [BindProperty]
        public PerformanceReport PerformanceReport { get; set; }
        [BindProperty]
        public int SelectedReportID { get; set; }
        [BindProperty]
        public string SelectedReportName { get; set; }
        [BindProperty]
        public Report SelectedReport { get; set; }


        public ReportSubmissionModel()
        {
            ProjectList = new List<Project>();
            GrantList = new List<Grant>();
            ReportList = new List<Report>();
            PerformanceReportList = new List<PerformanceReport>();
            SelectedGrantOrProjectID = new List<string>();
            CurrentUserID = new User();
            PerformanceReport = new PerformanceReport();
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

            // If a report is already selected, skip reinitializing the lists
            



                // Initialize ReportList
                SqlDataReader reportReader = DBClass.AllReportReader();
                while (reportReader.Read())
                {
                    ReportList.Add(new Report
                    {
                        ReportID = reportReader.GetInt32(reportReader.GetOrdinal("ReportID")),
                        ReportName = reportReader["ReportName"].ToString(),
                        ReportDate = reportReader.GetDateTime(reportReader.GetOrdinal("ReportDate")),
                        AuthorName = reportReader["AuthorName"].ToString()
                    });
                }

                // Initialize GrantList
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

                // Initialize ProjectList
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

                // Initialize PerformanceReportList
                SqlDataReader performanceReportReader = DBClass.AllPerformanceReportReader();
                while (performanceReportReader.Read())
                {
                    PerformanceReportList.Add(new PerformanceReport
                    {
                        PerformanceReportName = performanceReportReader["ReportName"].ToString(),
                        PerformanceReportID = performanceReportReader.GetInt32(performanceReportReader.GetOrdinal("PerformanceReportID")),
                        ReportID = performanceReportReader.GetInt32(performanceReportReader.GetOrdinal("ReportID")),
                        Description = performanceReportReader["Description"]?.ToString(),
                        AuthorName = performanceReportReader["AuthorName"]?.ToString(),
                        StartDate = performanceReportReader.GetDateTime(performanceReportReader.GetOrdinal("StartDate")),
                        EndDate = performanceReportReader.GetDateTime(performanceReportReader.GetOrdinal("EndDate")),
                    });
                }

                return Page();
            }
            
        


        public async Task<IActionResult> OnPostCreatePerformanceReportAsync()
        {
            int UserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
            CurrentUserID = DBClass.GetUserInfoById(UserID);

            PerformanceReport.AuthorName = $"{CurrentUserID.firstName} {CurrentUserID.lastName}";
            PerformanceReport.StartDate = PerformanceReport.StartDate;
            PerformanceReport.EndDate = PerformanceReport.EndDate;

            // Get the aggregated data
            var reportData = DBClass.GetPerformanceReportData(PerformanceReport.StartDate, PerformanceReport.EndDate);

            // Populate the PerformanceReport object
            PerformanceReport.Funding = reportData.Funding;
            PerformanceReport.ProjectsCompleted = reportData.ProjectsCompleted;
            PerformanceReport.GrantsArchived = reportData.GrantsArchived;
            PerformanceReport.ActiveGrants = reportData.ActiveGrants;
            PerformanceReport.GrantsInProgress = reportData.GrantsInProgress;
            PerformanceReport.GrantsSubmitted = reportData.GrantsSubmitted;
            PerformanceReport.ProjectsWIP = reportData.ProjectsWIP;
            PerformanceReport.PapersPublished = reportData.PapersPublished;

            // Insert the performance report into the database
            DBClass.InsertPerformanceReport(PerformanceReport);

            return RedirectToPage("/ReportSubmission");
        }




        public async Task<IActionResult> OnPostProgressReportAsync()
        {
            if (SelectedGrantOrProjectID == null || !SelectedGrantOrProjectID.Any())
            {
                throw new Exception("No grants or projects were selected.");
            }

            if (CurrentUserID == null)
            {
                throw new Exception("CurrentUserID is null. User information could not be retrieved.");
            }

            int UserID = Convert.ToInt32(HttpContext.Session.GetString("userID"));

            CurrentUserID = DBClass.GetUserInfoById(UserID);

            string authorName = string.Concat(CurrentUserID.firstName, " ", CurrentUserID.lastName);

            var report = new Report
            {
                AuthorName = authorName,
                ReportDate = DateTime.Now,
                ReportName = ReportName
            };

            var subjects = new List<ReportSubject>();

            for (int i = 0; i < SubjectTitle.Count; i++)
            {
                int? grantID = null;
                int? projectID = null;

                if (i < SelectedGrantOrProjectID.Count)
                {
                    var selected = SelectedGrantOrProjectID[i];
                    if (selected.StartsWith("grant-"))
                    {
                        grantID = int.Parse(selected.Replace("grant-", ""));
                    }
                    else if (selected.StartsWith("project-"))
                    {
                        projectID = int.Parse(selected.Replace("project-", ""));
                    }
                    else
                    {
                        throw new Exception($"Invalid selection: {selected}");
                    }
                }


                subjects.Add(new ReportSubject
                {
                    SubjectTitle = SubjectTitle[i],
                    SubjectText = SubjectText[i],
                    GrantID = grantID,
                    ProjectID = projectID
                });
            }



            DBClass.InsertReport(report, new List<int>(), new List<int>(), subjects);

            return RedirectToPage("/ReportSubmission");
        }

        public void OnPostSelectProgressReport(int reportID)
        {
            SelectedReportID = reportID;

            // Clear previous data
            SubjectTitle.Clear();
            SubjectText.Clear();
            GrantList.Clear();
            ProjectList.Clear();

            // Retrieve the selected report data
            using (SqlDataReader reader = DBClass.SingleReportReader(reportID))
            {
                while (reader.Read())
                {
                    // Log the retrieved data for debugging
                    Console.WriteLine($"DB ReportName: {reader["ReportName"]}");
                    Console.WriteLine($"DB AuthorName: {reader["AuthorName"]}");
                    Console.WriteLine($"DB ReportDate: {reader["ReportDate"]}");

                    // Always assign the SelectedReport property
                    SelectedReport = new Report
                    {
                        ReportID = reportID,
                        ReportName = reader["ReportName"].ToString(),
                        AuthorName = reader["AuthorName"].ToString(),
                        ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate"))
                    };

                    // Populate SubjectTitle and SubjectText
                    if (reader["SubjectTitle"] != DBNull.Value && reader["SubjectText"] != DBNull.Value)
                    {
                        SubjectTitle.Add(reader["SubjectTitle"].ToString());
                        SubjectText.Add(reader["SubjectText"].ToString());
                    }

                    // Populate GrantList
                    if (reader["GrantID"] != DBNull.Value)
                    {
                        GrantList.Add(new Grant
                        {
                            GrantID = (int)reader["GrantID"],
                            grantName = reader["GrantName"].ToString(),
                            businessName = reader["GrantBusinessName"].ToString(),
                            category = reader["GrantCategory"].ToString(),
                            dueDate = reader["GrantDueDate"] != DBNull.Value ? (DateTime)reader["GrantDueDate"] : DateTime.MinValue,
                            grantStatus = reader["GrantStatus"].ToString(),
                            amount = reader["GrantAmount"] != DBNull.Value ? Convert.ToDouble(reader["GrantAmount"]) : 0
                        });
                    }

                    // Populate ProjectList
                    if (reader["ProjectID"] != DBNull.Value)
                    {
                        ProjectList.Add(new Project
                        {
                            ProjectID = (int)reader["ProjectID"],
                            ProjectName = reader["ProjectName"].ToString(),
                            ProjectStatus = reader["ProjectStatus"].ToString(),
                            DateDue = reader["ProjectDueDate"] != DBNull.Value ? (DateTime)reader["ProjectDueDate"] : DateTime.MinValue,
                            DateCreated = reader["ProjectCreatedDate"] != DBNull.Value ? (DateTime)reader["ProjectCreatedDate"] : DateTime.MinValue,
                            DateCompleted = reader["ProjectCompletedDate"] != DBNull.Value ? (DateTime)reader["ProjectCompletedDate"] : DateTime.MinValue
                        });
                    }
                }
            }

            // Log for debugging
            Console.WriteLine($"SelectedReport: {SelectedReport?.ReportName}, Author: {SelectedReport?.AuthorName}, Date: {SelectedReport?.ReportDate}");
        }






    }
}





