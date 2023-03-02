﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Student
    {
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [Required]
        public int Grade { get; set; }
        [Required]
        public string School { get; set; }
        public string? Grades { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public List<OpenQuestionAnswer> OpenAnswers { get; set; }
        public List<ClosedQuestionAnswer> ClosedAnswers { get; set; }
    }
}
