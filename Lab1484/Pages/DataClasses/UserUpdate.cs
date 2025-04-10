using Microsoft.AspNetCore.Mvc;

namespace Lab1484.Pages.DataClasses
{
    public class UserUpdate
    {
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public int? UserType { get; set; }
        [BindProperty]
        public string? FirstName { get; set; }
        [BindProperty]
        public string? LastName { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Phone { get; set; }
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
    }
}
