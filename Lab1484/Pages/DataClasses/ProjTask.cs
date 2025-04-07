namespace Lab1484.Pages.DataClasses
{
    public class ProjTask
    {
        public int TaskID { get; set; }

        public int ProjectID { get; set; }

        public string? taskDescription { get; set; }

        public DateTime? dueDate { get; set; }

        public string? ProjectName { get; set; }
    }
}
