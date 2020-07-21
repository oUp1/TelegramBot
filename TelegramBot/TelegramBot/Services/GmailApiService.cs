using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot
{
    class GmailApiService
    {
        private readonly string _userId;
        private readonly GmailService _service;

        public GmailApiService(string email)
        {
            _userId = email;
            _service = GmailFactory.CreateGmailService();
        }
        public async Task<ListMessagesResponse> GetAllMessagesAsync()
        {
            try
            {
                var request = _service.Users.Messages.List(_userId);

                return await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return null;
        }
        public async Task<Message> GetMessageAsync(string id)
        {
            try
            {
                var request = _service.Users.Messages.Get(_userId, id);
                request.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;

                return await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return null;
        }
        public async void DeleteMessageAsync(string id)
        {
            try
            {
                var request = _service.Users.Messages.Delete(_userId, id);

                await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
        public async void SendEmail(Email email)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.To.Add(email.To);
                mailMessage.Subject = email.Subject;
                mailMessage.Body = email.Text;

                var mimeMessage = MimeMessage.CreateFromMailMessage(mailMessage);
                var gmailMessage = new Message { Raw = Encode(mimeMessage.ToString()) };

                await _service.Users.Messages.Send(gmailMessage, email.To).ExecuteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
        private static string Encode(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            return System.Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}
