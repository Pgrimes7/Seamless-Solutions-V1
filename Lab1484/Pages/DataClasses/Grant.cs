using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lab1484.Pages.DataClasses
{
    public class Grant
    {
        public int GrantID { get; set; }

        public string FacultyLeadID { get; set; }

        public string BusinessPartnerID { get; set; }

        public string businessName { get; set; }

        public string? category { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Due Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime dueDate { get; set; }

        public string? grantStatus { get; set; }

        public double amount { get; set; }

        public string? facultyName { get; set; }
    }
}
