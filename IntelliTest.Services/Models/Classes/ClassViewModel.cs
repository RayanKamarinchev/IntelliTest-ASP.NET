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
using Microsoft.AspNetCore.Http;

namespace IntelliTest.Core.Models.Classes
{
    public class ClassViewModel
    {
        public Guid Id { get; set; }
        public TeacherViewModel? Teacher { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name = "Име")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Предмет")]
        public Subject Subject { get; set; }
        [StringLength(500, MinimumLength = 5)]
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string ImageUrl { get; set; } = "";
        [Display(Name = "Изображение")]
        public IFormFile? Image { get; set; }
    }
}
