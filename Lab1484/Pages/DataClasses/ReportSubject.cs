namespace Lab1484.Pages.DataClasses
{
    public class ReportSubject
    {

        
            public int SubjectID { get; set; } // Primary Key
            public int ReportID { get; set; } // Foreign Key
        public int? GrantID { get; set; } // Foreign Key
        public int? ProjectID { get; set; } // Foreign Key
       
        public string SubjectTitle { get; set; }
            public string SubjectText { get; set; }
        

    }
}
