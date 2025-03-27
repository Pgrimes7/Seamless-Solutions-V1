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
            SqlDataReader grantReader = DBClass.GrantReader();

            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    businessName = grantReader["businessName"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    category = grantReader["category"].ToString(),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("submissionDate")),
                    facultyName = grantReader["FacultyLead"].ToString(),
                    grantStatus = grantReader["grantStatus"].ToString()
                });
            }

            DBClass.Lab3DBConnection.Close();


            var statusOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "In Progress", 1 },
        { "Opportunity", 2 },
        { "Pending", 3 },
        { "Approved", 4 },
        { "Rejected", 5 }
    };

            GrantList = GrantList
                .OrderBy(g => statusOrder.ContainsKey(g.grantStatus) ? statusOrder[g.grantStatus] : int.MaxValue)
                .ThenBy(g => g.businessName)
                .ToList();
        }


        public IActionResult OnPost()
        {
            var allowedStatuses = new List<string>
    {
        "Pending", "Approved", "Rejected", "In Progress", "Opportunity"
    };

            if (SelectedGrantId == 0 || string.IsNullOrEmpty(NewGrantStatus))
            {
                ModelState.AddModelError(string.Empty, "Please select a grant and a valid status.");
                return Page();
            }

            if (!allowedStatuses.Contains(NewGrantStatus))
            {
                ModelState.AddModelError(string.Empty, "Invalid status selected.");
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
