using System;
using System.Linq.Expressions;

namespace Backend.Helper
{


    public class Paging<T>
    {
        public int Skip { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
    }
}