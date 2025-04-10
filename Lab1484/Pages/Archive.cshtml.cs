using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Lab1484.Pages
{
    public class GrantArchiveModel : PageModel
    {
        public List<Grant> GrantList { get; set; }

        public GrantArchiveModel()
        {
            GrantList = new List<Grant>();
        }

        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            SqlDataReader grantReader = DBClass.GrantReader(null);

            while (grantReader.Read())
            {
                string status = grantReader["grantStatus"].ToString().ToLower();

                if (status == "archived" || status == "rejected")
                {
                    GrantList.Add(new Grant
                    {
                        GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                        businessName = grantReader["businessName"].ToString(),
                        amount = Double.Parse(grantReader["amount"].ToString()),
                        category = grantReader["category"].ToString(),
                        dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                        facultyName = grantReader["FacultyLead"].ToString(),
                        facultyEmail = grantReader["FacultyLeadEmail"].ToString(),
                        grantStatus = grantReader["grantStatus"].ToString(),
                        grantName = grantReader["grantName"].ToString()
                    });
                }
            }

            DBClass.Lab3DBConnection.Close();

           
            GrantList = GrantList
             .OrderByDescending(g => g.dueDate) 
             .ToList();


            return Page();
        }
    }
}
