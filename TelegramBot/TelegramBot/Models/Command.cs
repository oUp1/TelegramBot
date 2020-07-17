using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
    public delegate void OptionalAction(object obj = null);
    class Command
    {
        public string Name { get; set; }
        public OptionalAction Action;
    }
}
