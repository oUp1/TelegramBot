using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public class DailyEmailFactory
    {
        private List<DailyEmail> _dailyEmailList;
        private int Id = 1;
        private bool IsDailyEmail = true;
        public DailyEmailFactory()
        {
            _dailyEmailList = new List<DailyEmail>();
        }

        public IEnumerable<DailyEmail> GetDailyEmails()
        {
            return _dailyEmailList;
        }
        public bool GenerateDailyEmail(Message message)
        {
            var dailyEmail = new DailyEmail();
            dailyEmail.EmailId = message.Id;
            dailyEmail.Id = Id++;

            foreach (var mParts in message.Payload.Headers)
            {
                if (mParts.Name == "Date")
                {
                    if (!Regex.IsMatch(mParts.Value, $"{GetCurrentDate()}"))
                    {
                        IsDailyEmail = false;
                        Id = 1;
                    }
                    else
                        dailyEmail.Time = Convert.ToDateTime(mParts.Value.Replace("-0500 (CDT)", string.Empty)).ToShortDateString();
                }
                else if (mParts.Name == "From")
                {
                    dailyEmail.From = mParts.Value;
                }
                else if (mParts.Name == "Subject")
                {
                    dailyEmail.Subject = mParts.Value;
                }
            }

            if(IsDailyEmail)_dailyEmailList.Add(dailyEmail);
            return IsDailyEmail;
        }
        private string GetCurrentDate()
        {
            var dateTime = DateTime.UtcNow.Date;
            var day = dateTime.ToString("dd");
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(dateTime.ToString("MM")));
            return $"\\b{day} {month.Remove(month.Length - 1)}";
        }

    }
}
