namespace Lab1484.Pages.DataClasses
{
    public class Project
    {
        public int ProjectID { get; set; }
        public String? ProjectName { get; set; }
        public double ProjectCost { get; set; }
        public int? ProjectAdminID { get; set; }

        public String? AssignedEmployee { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateCompleted { get; set; }

        public DateTime DateDue { get; set; }
    }
}
