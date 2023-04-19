using IntelliTest.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Classes
{
    public class ClassViewModel
    {
        public Guid Id { get; set; }
        public TeacherViewModel Teacher { get; set; }
        public string Name { get; set; }
        public Subject Subject { get; set; }
        public string Description { get; set; }
    }
}
