using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TelegramBot.Models;

namespace TelegramBot
{
    class TelegramService
    {
        private GmailApiService _gmailApi;
        private EmailFactory _dailyEmailFactory;

        public TelegramService(string email)
        {
            _gmailApi = new GmailApiService(email);
            _dailyEmailFactory = new EmailFactory();
        }
        public IEnumerable<DailyEmail> GetDateEmails(DateTime date)
        {
            var allMessages = _gmailApi.GetAllMessagesAsync().Result;

            foreach (var item in allMessages.Messages)
            {
                var message = _gmailApi.GetMessageAsync(item.Id).Result;

                if (_dailyEmailFactory.GenerateDateEmail(message, date)) break;
            }
            return _dailyEmailFactory.GetEmails();
        }
        public void DeleteEmail(DateTime date, int? id = null)
        {
            var emails = GetDateEmails(date);
            if (id != null)
            {
                _gmailApi.DeleteMessageAsync(emails.Where(p => p.Id == id).FirstOrDefault().EmailId);
            }
            else
            {
                foreach (var item in emails)
                {
                    _gmailApi.DeleteMessageAsync(item.EmailId);
                }
            }
        }
        public void DeleteAllEmails()
        {
            while (true)
            {
                var allMessages = _gmailApi.GetAllMessagesAsync().Result;
                if (allMessages.Messages.Count == 0) return;
                foreach (var item in allMessages.Messages)
                {
                    var message = _gmailApi.GetMessageAsync(item.Id).Result;
                    if (message.LabelIds.Contains("IMPORTANT") || message.LabelIds.Contains("STARRED")) continue;
                    
                    _gmailApi.DeleteMessageAsync(item.Id);
                }
            }
        }
        public void SendEmail(Email email)
        {
            _gmailApi.SendEmail(email);
        }

    }
}
