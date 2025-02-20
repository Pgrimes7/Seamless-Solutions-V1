namespace Lab1484.Pages.DataClasses
{
    public class User
    {
        public int userID { get; set; }
        public String firstName {  get; set; }
        public String lastName { get; set; }
        public String email { get; set; }
        public int? phone { get; set; }
        public String? department { get; set; }

        public enum permissionType 
        { 
        Admin,
        Faculty,
        Employee
        
        
        }


       

    }
}
