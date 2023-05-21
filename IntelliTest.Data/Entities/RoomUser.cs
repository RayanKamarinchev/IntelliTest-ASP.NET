using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class RoomUser
    {
        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public Room Room { get; set; }
        [ForeignKey(nameof(Room))]
        public Guid RoomId { get; set; }
    }
}
