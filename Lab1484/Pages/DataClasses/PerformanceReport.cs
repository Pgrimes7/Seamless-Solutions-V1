namespace Lab1484.Pages.DataClasses
{
    public class PerformanceReport
    {
        public string PerformanceReportName { get; set; }
        public int PerformanceReportID { get; set; }
        public int ReportID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Funding { get; set; }
        public int ProjectsCompleted { get; set; }
        public int ActiveGrants { get; set; }
        public int GrantsSubmitted { get; set; }
        public int ProjectsWIP { get; set; }
        public double UnawardedFunding { get; set; }
        public string AuthorName { get; set; }
        public int PapersPublished { get; set; }


        public int PotentialGrants { get; set; }
        public int AwardedGrants { get; set; }
        public int RejectedGrants { get; set; }
        public int ArchivedGrants { get; set; }
    }

}
