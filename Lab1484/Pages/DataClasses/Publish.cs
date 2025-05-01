namespace Lab1484.Pages.DataClasses
{
    public class Publish
    {
        public int PublishID { get; set; }
        public DateTime? DueDate { get; set; }
        public string Requirements { get; set; }
        public string Authors { get; set; }
        public string Status { get; set; }
        public int ReferenceCount { get; set; }
        public string JournalTitle { get; set; }
        public string FileName { get; set; }
    }

}
