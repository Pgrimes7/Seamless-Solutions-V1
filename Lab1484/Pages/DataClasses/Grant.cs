using System.ComponentModel.DataAnnotations;

namespace Lab1484.Pages.DataClasses
{
    public class Grant
    {
        public int GrantID { get; set; }

        public string FacultyLeadID {  get; set; }

        public string BusinessPartnerID {  get; set; }

        public String businessName { get; set; }

        public String? category { get; set; }
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime? submissionDate { get; set; }
        public DateTime? awardDate { get; set; }
        public String? grantStatus { get; set; }
        public double amount { get; set; }

        public String? facultyName {get; set; }

        
    



    }
}
