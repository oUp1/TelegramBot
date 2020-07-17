using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    class CommandService
    {
        private TelegramService _serviceTelegram;
        private List<string> _result;
        private List<Command> _commands;

        public CommandService()
        {
            GenerateCommands();
            _result = new List<string>();
            _serviceTelegram = new TelegramService("bsssbhope@gmail.com");
        }

        public IEnumerable<string> ExecuteCommandAsync(string command)
        {

            var item = _commands.Select(p => p).Where(p => Regex.IsMatch(command, p.Name)).FirstOrDefault();

            var parameters = command.Split('/');

            if (parameters.Length > 2)
                item.Action.Invoke(parameters);
            else
                item.Action.Invoke();

            return _result;
        }
        private void GetDailyEmails(object parameters)
        {
            IEnumerable<DailyEmail> emails = null;
            try
            {
                 emails = _serviceTelegram.GetDailyEmails();
            }
            catch { }

            foreach (var msg in emails)
            {
                _result.Add(msg.ToString());
            }
        }
        private void DeleteDailyEmails(object parameters)
        {
            try
            {
                _serviceTelegram.DeleteAllDailyEmails();
            }
            catch { }

            _result.Add("done");
        }
        private void DeleteDailyEmail(object parameters)
        {
            var emailId = int.Parse((parameters as string[])[2]);
            try
            {
                _serviceTelegram.DeleteDailyEmail(emailId);
            }
            catch { }

            _result.Add("done");
        }
        private void SendEmail(object parameters)
        {
            var param = parameters as string[];
            var email = new Email()
            {
                To = param[2],
                Subject = param[3],
                Text = param[4]
            };
            try
            {
                _serviceTelegram.SendEmail(email);
            }
            catch { }

            _result.Add("done");
        }
        private void GenerateCommands()
        {
            _commands = new List<Command>
            {
                new Command(){Name="\\/dailyEmails", Action=GetDailyEmails},
                new Command(){Name="\\/deleteAll", Action=DeleteDailyEmails},
                new Command(){Name="\\/delete\\/[0-9]", Action=DeleteDailyEmail},
                new Command(){Name="\\/sendEmail\\/(.*)\\/(.*)\\/(.*)", Action=SendEmail}
            };
        }
    }
}
