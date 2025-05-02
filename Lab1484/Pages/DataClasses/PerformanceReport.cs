namespace Lab1484.Pages.DataClasses
{
    public class PerformanceReport
    {
        public String PerformanceReportName { get; set; } // Name of the performance report
        public int PerformanceReportID { get; set; } // Primary Key
        public int ReportID { get; set; } // Foreign Key referencing Reports table
        public string Description { get; set; } // Description of the performance report
        public DateTime StartDate { get; set; } // Start date of the performance period
        public DateTime EndDate { get; set; } // End date of the performance period
        public double Funding { get; set; } // Total funding amount
        public int ProjectsCompleted { get; set; } // Number of projects completed
        public int GrantsArchived { get; set; } // Number of grants archived
        public int ActiveGrants { get; set; } // Number of active grants
        public int GrantsInProgress { get; set; } // Number of grants in progress
        public int GrantsSubmitted { get; set; } // Number of grants submitted
        public int ProjectsWIP { get; set; } // Number of projects in work-in-progress
        public double BudgetUsed { get; set; } // Total budget used
        public string AuthorName { get; set; } // Name of the author of the report
        public int PapersPublished { get; set; } // Number of papers published
    }
}
