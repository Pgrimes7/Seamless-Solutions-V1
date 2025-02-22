using System.ComponentModel.DataAnnotations;
using Lab1484.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ProjectCreationModel : PageModel
    {

        /*Date time button to select dates associated with project? Or it auto does it when created idk
         * String input for project name
         * String input for project status? Or, radio buttons to do options liek approved, unapproved etc..
         * Need AdminID selection, access by select statement where userType = admin
         */
        [Required]
        public String ProjectName { get; set; }

        [Required]
        public int AdminID { get; set; }

        [Required]
        public float ProductCost { get; set; }

        [Required]
        public String DueDate { get; set; }

        [Required]
        public String ProjectStatus { get; set; }

        [Required]
        public List<User> EmployeeList { get; set; }

        public void OnGet()
        {

        }

        //public IActionResult OnPost()
        //{

        //}
    }
}
