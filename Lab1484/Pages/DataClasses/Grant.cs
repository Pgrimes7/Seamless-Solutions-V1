namespace Lab1484.Pages.DataClasses
{
    public class Grant
    {
        public int GrantID { get; set; }

        public string FacultyLeadID {  get; set; }

        public string BusinessPartnerID {  get; set; }

        public String businessName { get; set; }

        public String? category { get; set; }
        public DateTime submissionDate { get; set; }
        public DateTime awardDate { get; set; }
        public String? grantStatus { get; set; }
        public double amount { get; set; }

        public String? facultyName {get; set; }

        
    



    }
}
