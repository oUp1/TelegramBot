namespace TelegramBot.Models
{
    public class DailyEmail
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string Time { get; set; }

        public override string ToString()
        {
            return $"Id - {Id},\nFrom - {From},\nSubject - {Subject},\nTime - {Time}.\nLink - https://mail.google.com/mail/u/0/?pli=1#inbox/{EmailId} ";
        }
    }
}
