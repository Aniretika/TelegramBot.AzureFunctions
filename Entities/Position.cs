using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFunction.Entities
{
    public class Position
    {
        public Position()
        {
            this.AuthorId = default;
            this.BotId = default;
            this.Requester = default;
            this.Description = default;
        }

        public string? AuthorId { get; set; }

        public string? BotId { get; set; }

        public string? Requester { get; set; }

        public string? Description { get; set; }

    }
}

