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

            // Get all messages involving the current user (sent or received)
            var allMessages = DBClass.GetAllMessagesForUser(currentUser);

            // Group by the conversation partner
            Messages = allMessages
                .GroupBy(m => m.Sender == currentUser ? m.Receiver : m.Sender)
                .Select(g => g.OrderByDescending(m => m.SentDate).First()) // most recent per conversation
                .OrderByDescending(m => m.SentDate) // sort all by date
                .ToList();

            UnreadMessagesCount = allMessages.Count(m => m.Receiver == currentUser && m.IsRead == 0);

            return Page();
        }


        public IActionResult OnPostMarkAsRead(int messageId)
        {
            DBClass.MarkMessageAsRead(messageId);

            currentUser = HttpContext.Session.GetString("username");
            Messages = DBClass.GetReceivedMessages(currentUser);
            UnreadMessagesCount = Messages.Count(m => m.IsRead == 0);

            return Page();
        }

    }
}
