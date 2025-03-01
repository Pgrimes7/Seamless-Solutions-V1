using System.IO;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class MessagesModel : PageModel
    {
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public int UnreadMessagesCount { get; set; }
        public List<MessagesModel> Messages { get; set; } = new List<MessagesModel>();

        public IActionResult OnGet()
        {
            //Check to see if the user is logged in
            string currentUser = HttpContext.Session.GetString("username");
            //Redirect them if they aren't
            if (string.IsNullOrEmpty(currentUser))
            {
                //If not logged in, store the current URL and redirect to the Login page
                string currentPath = Request.GetEncodedUrl();
                HttpContext.Session.SetString("RedirectTo", "/Messages");
                return RedirectToPage("/SecureLoginLanding");
            }

            //Get and display unread messages
            UnreadMessagesCount = DBClass.GetUnreadMessagesCount(currentUser);

            // Load messages for the user
            Messages = DBClass.GetUserMessages(currentUser);

            return Page();
        }
    }
}
