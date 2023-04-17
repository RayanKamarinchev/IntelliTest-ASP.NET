using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User Admin { get; set; }
        public string AdminId { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<RoomUser> Users { get; set; }
        public bool IsDeleted { get; set; }
    }
}
