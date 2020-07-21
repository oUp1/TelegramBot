namespace TelegramBot.Models
{
    public delegate void OptionalAction(string [] param = null);
    class Command
    {
        public string Description { get; set; }
        public string Pattern { get; set; }

        public OptionalAction Action;
    }
}
