namespace Lab1484.Pages.DataClasses
{
    public class grant_user
    {
        //Permission level types that reflect the user's access to the grant/project data
        //Add more public ints if necessary in both this class and the DB

        public int viewPermission { get; set; }
        public int editPermission { get; set; }
        public int sensitiveInfoPermission { get; set; }
        public int userID { get; set; }
        public int grantID { get; set; }


    }
}
