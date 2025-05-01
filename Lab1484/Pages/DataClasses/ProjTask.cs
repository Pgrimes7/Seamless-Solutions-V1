using System.ComponentModel.DataAnnotations;

namespace Lab1484.Pages.DataClasses
{
    public class ProjTask
    {
        public int TaskID { get; set; }

        //public int? GrantID { get; set; }

        public int? ProjectID { get; set; }

        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string? taskDescription { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Award Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime? dueDate { get; set; }

        public string? ProjectName { get; set; }

        public string PTStatus { get; set; }

        //public string? grantName { get; set; }

        //public string? EmployeeName { get; set; }
    }
}
