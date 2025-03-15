using System.ComponentModel.DataAnnotations;

namespace Lab1484.Pages.DataClasses
{
    public class Project
    {
        public int ProjectID { get; set; }
        public String ProjectName { get; set; }
        public double ProjectCost { get; set; }
        public int ProjectAdminID { get; set; }

        public String? AssignedEmployee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Award Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime? DateCreated { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Award Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime? DateCompleted { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1753", "12/31/9999", ErrorMessage = "Award Date must be between 01/01/1753 and 12/31/9999")]
        public DateTime? DateDue { get; set; }

        public String AdminName { get; set; }

        public String ProjectStatus { get; set; }

        public String noteBody { get; set; }
    }
}
