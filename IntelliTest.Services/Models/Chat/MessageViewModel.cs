using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Chat
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string FromUserName { get; set; }
        public string FromFullName { get; set; }
        [Required]
        public string Room { get; set; }
        public string Avatar { get; set; }
    }
}
