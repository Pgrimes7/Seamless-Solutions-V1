using System.IO;
using Lab1484.Pages.DataClasses;
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
        public int IsRead { get; set; }
        public int UnreadMessagesCount { get; set; }

        public string currentUser { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? AttachmentFilePath { get; set; }


        public List<MessagesModel> Messages { get; set; } = new List<MessagesModel>();

        public IActionResult OnGet()
        {
            currentUser = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToPage("/Login");
            }

            // Retrieve only received messages
            Messages = DBClass.GetReceivedMessages(currentUser);

            // Calculate unread messages count
            UnreadMessagesCount = Messages.Count(m => m.IsRead == 0);

            return Page();
        }

        public IActionResult OnPostMarkAsRead(int messageId)
        {
            DBClass.MarkMessageAsRead(messageId);

            // Refresh counts after marking as read
            currentUser = HttpContext.Session.GetString("username");
            UnreadMessagesCount = DBClass.GetUnreadMessagesCount(currentUser);
            Messages = DBClass.GetReceivedMessages(currentUser);

            return Page();
        }

    }
}
