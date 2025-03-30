namespace Lab1484.Pages.DataClasses
{
    public class grant_user
    {
        //Permission level types that reflect the user's access to the grant/project data
        //Add more public ints if necessary in both this class and the DB

        public int view { get; set; }
        public int edit { get; set; }
        public int sensitiveInfo { get; set; }



    }
}
