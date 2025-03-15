using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;


namespace Lab1484.Pages
{
    public class UpdateGrantModel : PageModel
    {
        public List<Grant> GrantList { get; set; } = new();


        [BindProperty]

        public int SelectedGrantId { get; set; }


        [BindProperty]

        public string NewGrantStatus { get; set; }

        public void OnGet()
        {
            // Use DBClass to read grants from the database

            SqlDataReader grantReader = DBClass.GrantReader();


            while (grantReader.Read())

            {

                GrantList.Add(new Grant

                {

                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),

                    businessName = grantReader["businessName"].ToString(),

                    amount = Double.Parse(grantReader["amount"].ToString()),

                    category = grantReader["category"].ToString(),

                    submissionDate = grantReader.GetDateTime(grantReader.GetOrdinal("submissionDate")),

                    awardDate = grantReader.IsDBNull(grantReader.GetOrdinal("awardDate")) ? (DateTime?)null : grantReader.GetDateTime(grantReader.GetOrdinal("awardDate")),

                    facultyName = grantReader["FacultyLead"].ToString(),

                    grantStatus = grantReader["grantStatus"].ToString()

                });

            }


            // Close the DB connection

            DBClass.Lab3DBConnection.Close();
        }

        public IActionResult OnPost()

        {

            if (SelectedGrantId == 0 || string.IsNullOrEmpty(NewGrantStatus))

            {

                ModelState.AddModelError(string.Empty, "Please select a grant and a valid status.");

                return Page();

            }


            using (SqlConnection conn = DBClass.Lab3DBConnection)

            {

                conn.Open();

                string updateQuery = "UPDATE Grants SET grantStatus = @GrantStatus WHERE GrantID = @GrantID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))

                {

                    cmd.Parameters.AddWithValue("@GrantStatus", NewGrantStatus);

                    cmd.Parameters.AddWithValue("@GrantID", SelectedGrantId);

                    cmd.ExecuteNonQuery();

                }

            }


            return RedirectToPage();

        }
    }
}
