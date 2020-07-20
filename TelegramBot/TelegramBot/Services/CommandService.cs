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

        public IEnumerable<string> ExecuteCommand(string command)
        {

            var item = _commands?.FirstOrDefault(p => Regex.IsMatch(command, p.Pattern));

            var parameters = command.Split('/');
            try
            {
                if (parameters.Length > 2)
                    item.Action.Invoke(parameters);
                else
                    item.Action.Invoke();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return _result;
        }
        private void GetDailyEmails(object parameters)
        {
            IEnumerable<DailyEmail> emails = null;
            try
            {
                emails = _serviceTelegram.GetDateEmails(DateTime.Now);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            foreach (var msg in emails)
            {
                _result.Add(msg.ToString());
            }
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
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            _result.Add("done");
        }
        private void Help(object parameters)
        {
            foreach (var item in _commands)
            {
                _result.Add(item.Description);
            }
        }
        private void DeleteByDate(object parameters)
        {
            var param = parameters as string[];

            int day = int.Parse(param[2]);
            int month = int.Parse(param[3]);
            int year = int.Parse(param[4]);
            var date = new DateTime(year, month, day);

            try
            {
                _serviceTelegram.DeleteEmail(date);
                _result.Add("done");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private void DeleteDailyEmails(object parameters)
        {
            try
            {
                _serviceTelegram.DeleteEmail(DateTime.Now);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            _result.Add("done");
        }
        private void DeleteDailyEmail(object parameters)
        {
            var emailId = int.Parse((parameters as string[])[2]);
            try
            {
                _serviceTelegram.DeleteEmail(DateTime.Now, emailId);
                _result.Add("done");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private void deleteAllEmails(object parameters)
        {
            try
            {
                _serviceTelegram.DeleteAllEmails();
                _result.Add("done");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private void GenerateCommands()
        {
            _commands = new List<Command>
            {
                new Command(){Pattern=@"\/help", Description="/help", Action=Help},
                new Command(){Pattern=@"\/dailyEmails", Description="/dailyEmails", Action=GetDailyEmails},
                new Command(){Pattern=@"\/deleteTodaysEmails", Description="/deleteTodaysEmails", Action=DeleteDailyEmails},
                new Command(){Pattern=@"\/delete\/[0-9]", Description="/delete/emaildId", Action=DeleteDailyEmail},
                new Command(){Pattern=@"\/sendEmail\/(.*)\/(.*)\/(.*)", Description="/sendEmail/to/subject/body", Action=SendEmail},
                new Command(){Pattern=@"\/deleteByDate\/\d\d/\d\d/\d\d\d\d", Description="/deleteByDate/day/month/year", Action=DeleteByDate},
                new Command(){Pattern=@"\/deleteAllEmails", Description="/deleteAllEmails", Action=deleteAllEmails},
            };
        }
    }
}
