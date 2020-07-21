using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TelegramBot.Models
{
    public class EmailFactory
    {
        private List<DailyEmail> _dailyEmailList;
        private int Id = 1;
        private bool IsDateMath = false;
        public EmailFactory()
        {
            _dailyEmailList = new List<DailyEmail>();
        }

        public IEnumerable<DailyEmail> GetEmails()
        {
            return _dailyEmailList;
        }
        public bool GenerateDateEmail(Message message, DateTime date)
        {
            var dailyEmail = new DailyEmail();

            foreach (var mParts in message.Payload.Headers)
            {
                if (mParts.Name == "Date")
                {
                    var dt = Convert.ToDateTime(mParts.Value.Replace("GMT", string.Empty).Replace("(UTC)", string.Empty).Replace("(CDT)", string.Empty).Replace("(EEST)", string.Empty));
                    if (Convert.ToDateTime(dt).Date < date.Date)
                        return true;
                    else if (Regex.IsMatch(mParts.Value, $"{GetDate(date)}"))
                    {
                        dailyEmail.Time = dt.ToShortDateString();
                        IsDateMath = true;
                    }
                    else
                        Id = 1;
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
            if (IsDateMath)
            {
                dailyEmail.EmailId = message.Id;
                dailyEmail.Id = Id++;
                _dailyEmailList.Add(dailyEmail);
                IsDateMath = false;
            }
            return IsDateMath;
        }
        private string GetDate(DateTime date)
        {
            var dateTime = date;
            var day = dateTime.ToString("dd");
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(dateTime.ToString("MM")));
            return $"\\b{day} {month.Remove(month.Length - 1)}";
        }
    }
}
