﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Enums;

namespace IntelliTest.Data.Entities
{
    public class Class
    {
        [Key]
        public Guid Id { get; set; }
        public Teacher Teacher { get; set; }
        [ForeignKey(nameof(Teacher))]
        public Guid TeacherId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public Subject Subject { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<ClassTest> ClassTests { get; set; }
        public List<StudentClass> Students { get; set; }
    }
}
