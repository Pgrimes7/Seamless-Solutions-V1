using System.Data.SqlClient;
using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class NewMessageModel : PageModel
    {
        public List<Credentials> CredentialsList { get; set; } = new List<Credentials>();

        [BindProperty]
        public Message NewMessage { get; set; } = new Message();

        public void OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            SqlDataReader credentialsReader = DBClass.CredentialsReader();//instntiates class to read grant table and produce all available summary data
            while (credentialsReader.Read())
            {
                CredentialsList.Add(new Credentials
                {
                    Username = (string)credentialsReader["Username"],
                });
            }
        }

        public IActionResult OnPost()
        {
            string currentUser = HttpContext.Session.GetString("username");
            NewMessage.Sender = (string)currentUser;
            DBClass.InsertMessage(NewMessage);
            DBClass.Lab3DBConnection.Close();

            return RedirectToPage("/SentMessage");
        }
    }
}