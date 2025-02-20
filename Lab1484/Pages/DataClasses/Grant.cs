namespace Lab1484.Pages.DataClasses
{
    public class Grant//needs to be reworked for GrantID
    {
        public int GrantID { get; set; }
        public String GrantName { get; set; }
        public double GrantAmount { get; set; }

        public String? ProjectName { get; set; }
        public DateTime DateGranted { get; set; }
      
    }
}
