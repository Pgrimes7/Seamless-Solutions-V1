using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class GrantCreationModel : PageModel
    {
        public List<User> FacultyList { get; set; } = new List<User>();
        public List<User> PartnerList { get; set; } = new List<User>();

        [BindProperty]
        [Required]
        public Grant NewGrant { get; set; } = new Grant();

        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            // Load faculty
            SqlDataReader facultyReader = DBClass.FacultyReader();
            while (facultyReader.Read())
            {
                FacultyList.Add(new User
                {
                    userID = Int32.Parse(facultyReader["userID"].ToString()),
                    firstName = (string)facultyReader["firstName"],
                    lastName = (string)facultyReader["lastName"]
                });
            }

            // Load business partners
            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),
                    firstName = (string)partnerReader["firstName"],
                    lastName = (string)partnerReader["lastName"]
                });
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DBClass.InsertGrant(NewGrant);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/GrantCreation");
        }

        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();

            NewGrant.businessName = "InnovativeCorp";

            // Populate Faculty
            SqlDataReader facultyReader = DBClass.FacultyReader();
            while (facultyReader.Read())
            {
                FacultyList.Add(new User
                {
                    userID = Int32.Parse(facultyReader["userID"].ToString()),
                    firstName = (string)facultyReader["firstName"],
                    lastName = (string)facultyReader["lastName"]
                });
            }

            if (FacultyList.Any())
            {
                NewGrant.FacultyLeadID = FacultyList.First().userID.ToString();
            }

            // Populate Business Partner
            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),
                    firstName = (string)partnerReader["firstName"],
                    lastName = (string)partnerReader["lastName"]
                });
            }

            if (PartnerList.Any())
            {
                NewGrant.BusinessPartnerID = PartnerList.First().userID.ToString();
            }

            NewGrant.amount = 50000;
            NewGrant.category = "Research";
            NewGrant.grantStatus = "Incomplete";
            NewGrant.dueDate = DateTime.Now.AddMonths(6); // ? Replaces submission/award dates

            return Page();
        }
    }
}
