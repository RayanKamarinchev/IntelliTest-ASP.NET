using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models
{
    public class Filter
    {
        [Display(Name = "Потъри")]
        public string SearchTerm { get; set; }
        [Display(Name = "Сортиране")]
        public Sorting Sorting { get; set; }
        [Display(Name = "Предмет")]
        public Subject Subject { get; set; }
        [Display(Name = "Клас")]
        public int Grade { get; set; }
    }
}
