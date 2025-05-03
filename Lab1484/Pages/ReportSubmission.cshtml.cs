using System;
using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.JSInterop;


namespace Lab1484.Pages
{
    public class ReportSubmissionModel : PageModel
    {
        private readonly IJSRuntime _jsRuntime;//uses AI to learn how to do this
        

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
        [BindProperty]
        public string SelectedReportType { get; set; } // "Progress" or "Performance"
        [BindProperty]
        public PerformanceReport SelectedPerformanceReport { get; set; }

        [TempData]
        public string ProgReportSuccess { get; set; }

        [TempData]
        public string ProgReportFailure { get; set; }


        [TempData]
        public string PerformReportSuccess { get; set; }

        [TempData]
        public string PerformReportFailure { get; set; }

        

        public static bool IsPageLoaded { get; private set; }
        [JSInvokable]
        public static void PageLoaded()
        {
            IsPageLoaded = true;
            Console.WriteLine("Page has fully loaded in the browser.");
        }

        public ReportSubmissionModel(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;

            // Initialize properties
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
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }
            SelectedPerformanceReport = new PerformanceReport
            {
                Funding = 15000,
                UnawardedFunding = 10000
            };
            // Initialize Progress Reports and Performance Reports
            SqlDataReader reportReader = DBClass.AllReportReader();
            var addedReportIDs = new HashSet<int>(); // List to Track added ReportIDs since I am burger and didn't make progress report table flexible
            //Hashset list cause it guarantees uniqueness 
            while (reportReader.Read())
            {
                int reportID = reportReader.GetInt32(reportReader.GetOrdinal("ReportID"));
                if (!addedReportIDs.Contains(reportID)) // Add the report only if it hasn't been added yet
                {
                    if (reportReader["SubjectID"] != DBNull.Value) // Progress Reports (have SubjectID)
                {
                    ReportList.Add(new Report
                    {
                        ReportID = reportReader.GetInt32(reportReader.GetOrdinal("ReportID")),
                        ReportName = reportReader["ReportName"].ToString(),
                        ReportDate = reportReader.GetDateTime(reportReader.GetOrdinal("ReportDate")),
                        AuthorName = reportReader["AuthorName"].ToString()
                    });
                        addedReportIDs.Add(reportID); // Mark this ReportID as added
                    }
                }
                else // Performance Reports (no SubjectID)
                {
                    PerformanceReportList.Add(new PerformanceReport
                    {
                        ReportID = reportReader.GetInt32(reportReader.GetOrdinal("ReportID")), // Use the correct column for ReportID
                        PerformanceReportName = reportReader["ReportName"].ToString(),
                        AuthorName = reportReader["AuthorName"].ToString(),
                        CreatedDate = reportReader.GetDateTime(reportReader.GetOrdinal("ReportDate"))


                    });

                }
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

            return Page();
        }





        public async Task<IActionResult> OnPostCreatePerformanceReportAsync()
        {
            // Retrieve the current user ID from the session
            int userID = Convert.ToInt32(HttpContext.Session.GetString("userID"));
            CurrentUserID = DBClass.GetUserInfoById(userID);

            // Set the author name for the performance report
            PerformanceReport.AuthorName = $"{CurrentUserID.firstName} {CurrentUserID.lastName}";

            // Ensure the start and end dates are valid
            if (PerformanceReport.StartDate == default || PerformanceReport.EndDate == default)
            {
                throw new Exception("Start Date and End Date must be provided.");
            }

            // Get the aggregated data for the performance report
            var reportData = DBClass.GetPerformanceReportData(PerformanceReport.StartDate, PerformanceReport.EndDate);

            // Populate the PerformanceReport object with the aggregated data
            PerformanceReport.Funding = reportData.Funding;
            PerformanceReport.ProjectsCompleted = reportData.ProjectsCompleted;
            PerformanceReport.GrantsSubmitted = reportData.GrantsSubmitted;
            PerformanceReport.ProjectsWIP = reportData.ProjectsWIP;
            PerformanceReport.PapersPublished = reportData.PapersPublished;
            PerformanceReport.UnawardedFunding = reportData.UnawardedFunding;
            PerformanceReport.PotentialGrants = reportData.PotentialGrants;
            PerformanceReport.AwardedGrants = reportData.AwardedGrants;
            PerformanceReport.ActiveGrants = reportData.ActiveGrants;
            PerformanceReport.RejectedGrants = reportData.RejectedGrants;
            PerformanceReport.ArchivedGrants = reportData.ArchivedGrants;

            // Insert the performance report into the database
            bool success = DBClass.InsertPerformanceReport(PerformanceReport);

            if (success)
            {
                PerformReportSuccess = "Performance report successfully added.";
            }

            else
            {
                PerformReportFailure = "Error: Performance report could not be added.";
            }

            // Redirect back to the ReportSubmission page
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



            bool success = DBClass.InsertReport(report, new List<int>(), new List<int>(), subjects);

            if (success)
            {
                ProgReportSuccess = "Progress report successfully added.";

            }

            else
            {
                ProgReportFailure = "Error: Report could not be added.";
            }

            return RedirectToPage("/ReportSubmission");
        }

        public void OnPostSelectProgressReport(int reportID)
        {
            SelectedReportID = reportID;
            SelectedReportType = "Progress";

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
            Console.WriteLine($"Total Projects: {ProjectList.Count}");
            foreach (var project in ProjectList)
            {
                Console.WriteLine($"ProjectID: {project.ProjectID}, ProjectName: {project.ProjectName}, Status: {project.ProjectStatus}, DueDate: {project.DateDue}");
            }
        }


        public async Task<IActionResult> OnPostSelectPerformanceReport(int ReportID)
        {
            SelectedReportID = ReportID;
            SelectedReportType = "Performance";

            // Clear previous data
            PerformanceReport = new PerformanceReport();

            // Retrieve the selected performance report data
            using (SqlDataReader reader = DBClass.AllPerformanceReportReader(ReportID))
            {
                while (reader.Read())
                {
                    
                        PerformanceReport = new PerformanceReport
                        {
                            PerformanceReportID = reader.GetInt32(reader.GetOrdinal("PerformanceReportID")),
                            ReportID = reader.GetInt32(reader.GetOrdinal("ReportID")),
                            PerformanceReportName = reader["ReportName"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            StartDate = reader["StartDate"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("StartDate")) : DateTime.MinValue,
                            EndDate = reader["EndDate"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("EndDate")) : DateTime.MinValue,
                            Funding = reader["Funding"] != DBNull.Value ? reader.GetDouble(reader.GetOrdinal("Funding")) : 0,
                            ProjectsCompleted = reader["ProjectsCompleted"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("ProjectsCompleted")) : 0,
                            GrantsSubmitted = reader["GrantsSubmitted"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("GrantsSubmitted")) : 0,
                            ProjectsWIP = reader["ProjectsWIP"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("ProjectsWIP")) : 0,
                            PapersPublished = reader["PapersPublished"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("PapersPublished")) : 0,
                            UnawardedFunding = reader["UnawardedFunding"] != DBNull.Value ? reader.GetDouble(reader.GetOrdinal("UnawardedFunding")) : 0,
                            PotentialGrants = reader["PotentialGrants"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("PotentialGrants")) : 0,
                            AwardedGrants = reader["AwardedGrants"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("AwardedGrants")) : 0,
                            ActiveGrants = reader["ActiveGrants"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("ActiveGrants")) : 0,
                            RejectedGrants = reader["RejectedGrants"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("RejectedGrants")) : 0,
                            ArchivedGrants = reader["ArchivedGrants"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("ArchivedGrants")) : 0,
                            CreatedDate = reader["ReportDate"] != DBNull.Value ? reader.GetDateTime(reader.GetOrdinal("ReportDate")) : DateTime.MinValue,
                            AuthorName = reader["AuthorName"]?.ToString()
                        };

                        // Assign to SelectedPerformanceReport
                        SelectedPerformanceReport = PerformanceReport;
                         // Exit the loop once the matching report is found
                    }
                }
            // Log the updated data for debugging
            Console.WriteLine($"SelectedPerformanceReport: {SelectedPerformanceReport.PerformanceReportName}, Funding: {SelectedPerformanceReport.Funding}, UnawardedFunding: {SelectedPerformanceReport.UnawardedFunding}");



            return Page();
        }







    }
}





