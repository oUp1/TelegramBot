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
        private DailyEmailFactory _dailyEmailFactory;

        public TelegramService(string email)
        {
            _gmailApi = new GmailApiService(email);
            _dailyEmailFactory = new DailyEmailFactory();
        }

        public IEnumerable<DailyEmail> GetDailyEmails()
        {
            var allMessages = _gmailApi.GetAllMessagesAsync().Result;
            foreach (var item in allMessages.Messages)
            {
                var message =  _gmailApi.GetMessageAsync(item.Id).Result;

                if (!_dailyEmailFactory.GenerateDailyEmail(message)) break;
            }
            return _dailyEmailFactory.GetDailyEmails();
        }
        public void DeleteAllDailyEmails()
        {
            var emails = GetDailyEmails();
            foreach(var item in emails)
            {
                _gmailApi.DeleteMessageAsync(item.EmailId);
            }
        }
        public void DeleteDailyEmail(int id)
        {
            var emails = GetDailyEmails();

            var email = emails.Where(p => p.Id == id).FirstOrDefault();

            _gmailApi.DeleteMessageAsync(email.EmailId);
        }
        public void SendEmail(Email email)
        {
            _gmailApi.SendEmail(email);
        }
       
    }
}
