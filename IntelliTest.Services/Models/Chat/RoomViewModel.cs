using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Chat
{
    public class RoomViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} трябва да е между {2} и {1} символа.", MinimumLength = 5)]
        [RegularExpression(@"^\w+( \w+)*$", ErrorMessage = "Позволени са само букви и числа.")]
        public string Name { get; set; }

        public string Admin { get; set; }
        public string? LastMessage { get; set; }
        public string TimeStamp { get; set; }
    }
}
