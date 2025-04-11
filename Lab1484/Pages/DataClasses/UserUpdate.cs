using Microsoft.AspNetCore.Mvc;

namespace Lab1484.Pages.DataClasses
{
    public class UserUpdate
    {
        
        public int UserID { get; set; }
        
        public int? UserType { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        
        public string? Username { get; set; }
        
        public string? Password { get; set; }
    }
}
