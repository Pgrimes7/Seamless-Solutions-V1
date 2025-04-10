using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using Lab1484.Pages.DB;

public class AdminSearchModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string TypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

    public IActionResult OnGet()
    {
        //Check to see if the user is logged in
        string currentUser = HttpContext.Session.GetString("username");
        //Redirect them if they aren't
        int userType = DBClass.checkUserType(HttpContext);
        if (string.IsNullOrEmpty(currentUser))
        {
            return RedirectToPage("/Login");
        }

        if (userType != 0)
        {
            return RedirectToPage("/DashBoard");
        }

        if (string.IsNullOrEmpty(TypeFilter) || TypeFilter == "Project")
        {
            SqlDataReader reader = DBClass.ProjectReader();
            while (reader.Read())
            {
                SearchResults.Add(new SearchResult
                {
                    Type = "Project",
                    Name = reader["ProjectName"].ToString(),
                    Status = reader["ProjectStatus"].ToString(),
                    StartDate = reader["dateCreated"] as DateTime?,
                    EndDate = reader["dateCompleted"] as DateTime?
                });
            }
            reader.Close();
            DBClass.Lab3DBConnection.Close();
        }

        if (string.IsNullOrEmpty(TypeFilter) || TypeFilter == "Grant")
        {
            SqlDataReader reader = DBClass.GrantReader(null);
            while (reader.Read())
            {
                SearchResults.Add(new SearchResult
                {
                    Type = "Grant",
                    Name = reader["businessName"].ToString(),
                    Status = reader["grantStatus"].ToString(),
                    StartDate = reader["submissionDate"] as DateTime?,
                    EndDate = reader["awardDate"] as DateTime?
                });
            }
            reader.Close();
            DBClass.Lab3DBConnection.Close();
        }

        if (string.IsNullOrEmpty(TypeFilter) || TypeFilter == "Faculty")
        {
            SqlDataReader reader = DBClass.FacultyReader();
            while (reader.Read())
            {
                SearchResults.Add(new SearchResult
                {
                    Type = "Faculty",
                    Name = reader["firstName"].ToString() + " " + reader["lastName"].ToString(),
                    Status = "N/A"
                });
            }
            reader.Close();
            DBClass.Lab3DBConnection.Close();
        }

        if (string.IsNullOrEmpty(TypeFilter) || TypeFilter == "BusinessPartner")
        {
            SqlDataReader reader = DBClass.BusinessPartnerReader();
            while (reader.Read())
            {
                SearchResults.Add(new SearchResult
                {
                    Type = "Business Partner",
                    Name = reader["firstName"].ToString() + " " + reader["lastName"].ToString(),
                    Status = "N/A"
                });
            }
            reader.Close();
            DBClass.Lab3DBConnection.Close();
        }

        return Page();
    }

    public class SearchResult
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
