using Lab1484.Pages.DataClasses;
using Lab1484.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class ViewConversationModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string WithUser { get; set; }

        [BindProperty]
        public string ReplyMessage { get; set; }

        public string CurrentUser { get; set; }

        [BindProperty]
        public IFormFile? Attachment { get; set; }


        public List<MessagesModel> Conversation { get; set; }

        public IActionResult OnGet()
        {
            CurrentUser = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(CurrentUser))
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrEmpty(WithUser))
            {
                return RedirectToPage("/Messages");
            }

            // Retrieve messages between current user and the specified user
            Conversation = DBClass.GetConversationBetween(CurrentUser, WithUser);

            // Mark unread messages as read
            var unreadMessages = Conversation
                .Where(m => m.Receiver == CurrentUser && m.IsRead == 0)
                .ToList();

            foreach (var message in unreadMessages)
            {
                DBClass.MarkMessageAsRead(message.MessageId);
            }

            return Page();
        }


        public IActionResult OnPost()
        {
            CurrentUser = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(CurrentUser) || string.IsNullOrEmpty(WithUser) || string.IsNullOrEmpty(ReplyMessage))
            {
                return RedirectToPage("/Messages");
            }

            string? fileName = null;
            string? filePath = null;

            if (Attachment != null && Attachment.Length > 0)
            {
                fileName = Path.GetFileName(Attachment.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                filePath = Path.Combine("/uploads", fileName);
                var fullSavePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullSavePath, FileMode.Create))
                {
                    Attachment.CopyTo(stream);
                }
            }

            DBClass.SendMessage(CurrentUser, WithUser, ReplyMessage, fileName, filePath);

            Conversation = DBClass.GetConversationBetween(CurrentUser, WithUser);
            return Page();
        }

    }
}
