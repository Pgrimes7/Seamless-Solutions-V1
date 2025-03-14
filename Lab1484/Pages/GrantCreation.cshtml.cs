using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Reflection;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class GrantCreationModel : PageModel
    {

        [BindProperty]
        [Required]
        public List<User> FacultyList { get; set; } = new List<User>();
        public List<User> PartnerList { get; set; } = new List<User>();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader facultyReader = DBClass.FacultyReader();//instntiates class to read grant table and produce all available summary data
            while (facultyReader.Read())
            {
                FacultyList.Add(new User
                {
                    userID = Int32.Parse(facultyReader["userID"].ToString()),
                    firstName = (string)facultyReader["firstName"],
                    lastName = (string)facultyReader["lastName"]
                });
            }
            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();//instntiates class to read grant table and produce all available summary data
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),//needs to be changed to businessPartner Class
                    firstName = (string)partnerReader["firstName"],
                    lastName = (string)partnerReader["lastName"]
                });
            }

            return Page();

        }

        [BindProperty]

        public Grant NewGrant { get; set; } = new Grant();

        public IActionResult OnPost() {

            DBClass.InsertGrant(NewGrant);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/GrantCreation");

        }
        

        //Method with sample data for the populate button, curretnly sets page to population data and is stuck, will be patched lab3
        public IActionResult OnPostPopulateHandler()
        {
            ModelState.Clear();
            NewGrant.businessName = "InnovativeCorp";

            //Faculty member to use as sample data
            SqlDataReader facultyReader = DBClass.FacultyReader();//instntiates class to read grant table and produce all available summary data
            while (facultyReader.Read())
            {
                FacultyList.Add(new User
                {
                    userID = Int32.Parse(facultyReader["userID"].ToString()),
                    firstName = (string)facultyReader["firstName"],
                    lastName = (string)facultyReader["lastName"]
                });
                if (FacultyList != null && FacultyList.Count > 0)
                {
                    // Select the first Faculty Member as a sample
                    var selectedPartner = FacultyList.FirstOrDefault();
                    if (selectedPartner != null)
                    {
                        NewGrant.FacultyLeadID = selectedPartner.userID.ToString();
                    }
                }
            }
            //Business partner to use as sample data
            SqlDataReader partnerReader = DBClass.BusinessPartnerReader();//instntiates class to read grant table and produce all available summary data
            while (partnerReader.Read())
            {
                PartnerList.Add(new User
                {
                    userID = Int32.Parse(partnerReader["BusinessPartnerID"].ToString()),//needs to be changed to businessPartner Class
                    firstName = (string)partnerReader["firstName"],
                    lastName = (string)partnerReader["lastName"]
                });
                if (PartnerList != null && PartnerList.Count > 0)
                {
                    // Select the first Business Partner as a sample
                    var selectedPartner = PartnerList.FirstOrDefault();
                    if (selectedPartner != null)
                    {
                        NewGrant.BusinessPartnerID = selectedPartner.userID.ToString();
                    }
                }
            }
            //NewGrant.FacultyLeadID = "1";
            //NewGrant.BusinessPartnerID = "4";
            NewGrant.amount = 50000;
            NewGrant.category = "Research";
            NewGrant.grantStatus = "Incomplete";
            NewGrant.submissionDate = DateTime.Now;
            NewGrant.awardDate = DateTime.Now.AddMonths(6);

            return Page();
        }
       



    }
}
