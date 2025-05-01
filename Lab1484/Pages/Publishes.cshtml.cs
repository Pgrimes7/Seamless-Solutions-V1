using System.Data.SqlClient;
using System.IO;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class PublishesModel : PageModel
    {
        public List<Publish> PublishList { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public string[] StatusOptions = { "Not Submitted", "Submitted", "Accepted", "Accepted with Revisions", "Rejected" };

        public void OnGet()
        {
            PublishList = DBClass.GetAllPublishes();
            using (SqlDataReader reader = DBClass.AllUsersReader())
            {
                while (reader.Read())
                {
                    Users.Add(new User
                    {
                        userID = (int)reader["userID"],
                        firstName = reader["firstName"].ToString(),
                        lastName = reader["lastName"].ToString()
                    });
                }
            }

        }

        public IActionResult OnPostCreate(Publish publish)
        {
            // Set ReferenceCount based on Status
            switch (publish.Status?.ToLower())
            {
                case "accepted":
                    publish.ReferenceCount = 2;
                    break;
                case "accepted with revisions":
                    publish.ReferenceCount = 1;
                    break;
                case "submitted":
                case "not submitted":
                case "rejected":
                default:
                    publish.ReferenceCount = 0;
                    break;
            }

            DBClass.InsertPublish(publish);
            return RedirectToPage();
        }

    }
}
