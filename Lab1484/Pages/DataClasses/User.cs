using System.ComponentModel.DataAnnotations;

namespace Lab1484.Pages.DataClasses
{
    public class User
    {
        //set up user class
        public int userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string? phone { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }

        [Required(ErrorMessage = "User Type is required.")]
        public int? UserType { get; set; }//in db these are seen as ints 0-3, 0 = admin, 1 = faculty, 2 = staff, 3 = student
    }




}

