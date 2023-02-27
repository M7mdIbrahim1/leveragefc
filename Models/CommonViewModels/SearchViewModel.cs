using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Backend.Models.CommonViewModels
{
    public class SearchViewModel<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? predicate;
        public int? page;
        public int? pageSize;
        public Expression<Func<T, object>>? orderBy;

    }
}