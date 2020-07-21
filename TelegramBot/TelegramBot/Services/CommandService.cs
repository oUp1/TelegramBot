using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private void GetDailyEmails(string[] parameters)
        {
            IEnumerable<DailyEmail> emails = null;
            try
            {
                emails = _serviceTelegram.GetDateEmails(DateTime.Now);
                foreach (var msg in emails)
                {
                    _result.Add(msg.ToString());
                }
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }
        }
        private void SendEmail(string[] parameters)
        {
            var email = new Email()
            {
                To = parameters[2],
                Subject = parameters[3],
                Text = parameters[4]
            };
            try
            {
                _serviceTelegram.SendEmail(email);
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }

        }
        private void Help(string[] parameters)
        {
            foreach (var item in _commands)
            {
                _result.Add(item.Description);
            }
        }
        private void DeleteByDate(string[] parameters)
        {
            try
            {
                int day = int.Parse(parameters[2]);
                int month = int.Parse(parameters[3]);
                int year = int.Parse(parameters[4]);
                var date = new DateTime(year, month, day);

                _serviceTelegram.DeleteEmail(date);
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }
        }
        private void DeleteDailyEmails(string[] parameters)
        {
            try
            {
                _serviceTelegram.DeleteEmail(DateTime.Now);
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }
        }
        private void DeleteDailyEmail(string[] parameters)
        {
            try
            {
                var emailId = int.Parse(parameters[2]);
                _serviceTelegram.DeleteEmail(DateTime.Now, emailId);
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }
        }
        private void deleteAllEmails(string[] parameters)
        {
            try
            {
                _serviceTelegram.DeleteAllEmails();
                _result.Add("done");
            }
            catch (Exception ex) { _result.Add($"Error - {ex.Message}"); }
        }
        private void GenerateCommands()
        {
            _commands = new List<Command>
            {
                new Command(){Pattern=@"\/help", Description="/help", Action=Help},
                new Command(){Pattern=@"\/getTodaysEmails", Description="/getTodaysEmails", Action=GetDailyEmails},
                new Command(){Pattern=@"\/deleteTodaysEmails", Description="/deleteTodaysEmails", Action=DeleteDailyEmails},
                new Command(){Pattern=@"\/delete\/[0-9]", Description="/delete/emaildId", Action=DeleteDailyEmail},
                new Command(){Pattern=@"\/sendEmail\/(.*)\/(.*)\/(.*)", Description="/sendEmail/to/subject/body", Action=SendEmail},
                new Command(){Pattern=@"\/deleteByDate\/\d\d/\d\d/\d\d\d\d", Description="/deleteByDate/day/month/year", Action=DeleteByDate},
                new Command(){Pattern=@"\/deleteAllEmails", Description="/deleteAllEmails", Action=deleteAllEmails},
            };
        }
    }
}
