using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class QuestionViewModel
    {
        [Required]
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Id { get; set; }
        [Required]
        public bool IsEquation { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ImagePath { get; set; }

        public int Index { get; set; }
        public IFormFile? Image { get; set; }
    }
}
