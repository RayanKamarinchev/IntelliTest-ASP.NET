using IntelliTest.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntelliTest.Core.Models
{
    public class QueryModel<T>
    {
        public QueryModel()
        {
            Filters = new Filter();
        }

        public QueryModel(string SearchTerm, int Grade, Subject Subject, Sorting Sorting, int currentPage)
        {
            Filters = new Filter();
            Filters.SearchTerm = SearchTerm;
            Filters.Grade = Grade;
            Filters.Subject = Subject;
            Filters.Sorting = Sorting;
            CurrentPage = currentPage;
        }
        public int ItemsPerPage { get; set; } = 6;
        [Display(Name = "Search Term")]
        public int CurrentPage { get; set; } = 1;
        public int TotalItemsCount { get; set; }
        public Filter Filters { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
