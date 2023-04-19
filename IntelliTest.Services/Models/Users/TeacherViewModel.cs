using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Users
{
    public class TeacherViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string School { get; set; }
    }
}
