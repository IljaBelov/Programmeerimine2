using System;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class PagedResult<T>
    {
        public IList<T> Results { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
    }
}