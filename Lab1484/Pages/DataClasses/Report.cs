namespace Lab1484.Pages.DataClasses
{
    public class Report
    {
        public int ReportID { get; set; }
        public string? ReportName { get; set; }

        //make lists of IDs for the grants and projects cause many to many relationship
        public List<int> GrantIDs { get; set; } = new List<int>();
        public List<int> ProjectIDs { get; set; } = new List<int>();
        //make a list of report subjects cause multiple subjects for one report
        public List<ReportSubject> Subjects { get; set; } = new List<ReportSubject>();

        public DateTime ReportDate { get; set; }


        //Could be used to store the file path of the report later
        public string? ReportFilePath { get; set; }
        public string? ReportFileType { get; set; }
    }
}
