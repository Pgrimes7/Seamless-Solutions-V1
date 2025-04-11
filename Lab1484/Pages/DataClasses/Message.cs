namespace Lab1484.Pages.DataClasses
{
    public class Message
    {
        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Content { get; set; }

        public int IsRead { get; set; } // "0 (unread)" or "1 (read)"


    }
}