using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace Lab1484.Pages
{
    public class UpdateGrantModel : PageModel
    {
        public List<Grant> GrantList { get; set; } = new();


        [BindProperty]

        public int? SelectedGrantId { get; set; }


        [BindProperty]

        public string? NewGrantStatus { get; set; }

        [BindProperty]
        public string? EditBusinessName { get; set; }

        [BindProperty]
        public string? EditCategory { get; set; }

        [BindProperty]
        public double? EditAmount { get; set; }

        [BindProperty]
        public DateTime? EditDueDate { get; set; }

        [BindProperty]
        public string? EditFacultyName { get; set; }


        [BindProperty]
        public string? EditGrantName { get; set; }

        [BindProperty]
        public string? EditFacultyLeadID { get; set; }

        public List<User> AllUsersList { get; set; } = new();





        public void OnGet()
        {
            SqlDataReader grantReader = DBClass.GrantReader();

            while (grantReader.Read())
            {
                GrantList.Add(new Grant
                {
                    GrantID = Int32.Parse(grantReader["GrantID"].ToString()),
                    grantName = grantReader["grantName"].ToString(),
                    businessName = grantReader["businessName"].ToString(),
                    category = grantReader["category"].ToString(),
                    amount = Double.Parse(grantReader["amount"].ToString()),
                    dueDate = grantReader.GetDateTime(grantReader.GetOrdinal("dueDate")),
                    grantStatus = grantReader["grantStatus"].ToString(),
                    facultyName = grantReader["FacultyLead"].ToString()
                });
            }

            DBClass.Lab3DBConnection.Close();


            SqlDataReader userReader = DBClass.AllUsersReader();

            while (userReader.Read())
            {
                AllUsersList.Add(new User
                {
                    userID = int.Parse(userReader["userID"].ToString()),
                    firstName = userReader["firstName"].ToString(),
                    lastName = userReader["lastName"].ToString()
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
            if (SelectedGrantId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a grant.");
                return Page();
            }

            using SqlConnection conn = DBClass.Lab3DBConnection;
            conn.Open();

            List<string> setClauses = new();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            if (!string.IsNullOrWhiteSpace(EditFacultyLeadID))
            {
                setClauses.Add("FacultyLeadID = @FacultyLeadID");
                cmd.Parameters.AddWithValue("@FacultyLeadID", EditFacultyLeadID);
            }


            if (!string.IsNullOrWhiteSpace(EditGrantName))
            {
                setClauses.Add("grantName = @GrantName");
                cmd.Parameters.AddWithValue("@GrantName", EditGrantName);
            }

            if (!string.IsNullOrWhiteSpace(EditBusinessName))
            {
                setClauses.Add("businessName = @BusinessName");
                cmd.Parameters.AddWithValue("@BusinessName", EditBusinessName);
            }

            if (!string.IsNullOrWhiteSpace(EditCategory))
            {
                setClauses.Add("category = @Category");
                cmd.Parameters.AddWithValue("@Category", EditCategory);
            }

            if (EditAmount.HasValue)
            {
                setClauses.Add("amount = @Amount");
                cmd.Parameters.AddWithValue("@Amount", EditAmount.Value);
            }

            if (EditDueDate.HasValue)
            {
                setClauses.Add("dueDate = @DueDate");
                cmd.Parameters.AddWithValue("@DueDate", EditDueDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(NewGrantStatus))
            {
                setClauses.Add("grantStatus = @GrantStatus");
                cmd.Parameters.AddWithValue("@GrantStatus", NewGrantStatus);
            }

            if (setClauses.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No fields were provided to update.");
                return Page();
            }

            string updateQuery = $"UPDATE Grants SET {string.Join(", ", setClauses)} WHERE GrantID = @GrantID";
            cmd.Parameters.AddWithValue("@GrantID", SelectedGrantId);
            cmd.CommandText = updateQuery;

            cmd.ExecuteNonQuery();

            return RedirectToPage();
        }

    }
}
